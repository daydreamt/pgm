namespace pgm_test

open System
open System.Net
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra


module NlpTest = 
    1

//module MtTest =
//    open pgm.mt

    //let d = pgm.mt.d1

module GraphTest = 
    open pgm_test.graph_test
    let _ = pgm_test.graph_test.simple()

    let __ = pgm_test.markov_grid_test.simple()

module UtilsTest =
    open pgm.ml_useful
    
    let xs = [[0.;0.;]; [1.;1.]; [0.;-3.;]; [1.;-4.;];]
    let ys = [1.; 1.; -1.; -1.;]

    let p1 = Perceptron(2)
    let r = p1.classify([0.; 0.])
    printfn "Classification error %f" (p1.get_classification_error xs ys)

    let m = p1.get_misclassified xs ys
    let u = p1.train xs ys

module RbmTest = 

    open pgm.rbm

    let (vv, hh, ww) = rbm_constructed (uint32 3) (uint32 2)
    
    printfn "Visible Units:"
    for i in 0 .. vv.Length - 1 do
        printfn "%d" vv.[i]
        
    printfn "Hidden Units:"
    for i in 0 .. hh.Length - 1 do
        printfn "%d" hh.[i]
       
    printfn "Weights:"
    for i in 0 .. ww.Length - 1 do
        printfn "%f" ww.[i]      

 
    let b = sigmoid(3.);
    printfn "b: %f. The end." b
    
module Main =
    let timer = new System.Diagnostics.Stopwatch()
    
    
    timer.Start()

    //open pgm_test.graph_test

    timer.Stop()


    printfn "Elapsed Time for tests: %i ms. Also, the end." timer.ElapsedMilliseconds

    [<EntryPoint>]
    let main argv = 
        let endofapp = Console.ReadKey()
        0