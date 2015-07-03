namespace pgm_test


open pgm.Graph
open pgm.Base
open pgm.Distributions

module graph_test =
    open MathNet.Numerics
    open MathNet.Numerics.LinearAlgebra
    open System

    let dependencies() =

        let m1 = matrix [[ 2.0; 3.0 ];
                         [ 4.0; 5.0 ]]

        let v1 = vector [ 1.0; 2.0; 3.0 ]
        let v2 = vector [1.0]

        let l1 = [1.;2.;]

        //List.concat l1 3.
        let V = Vector<double>.Build

        printfn "Vectors and matrices OK"


    let simple() = 
        
        let v1 = new Vertex "5.0"
        let v2 = new Vertex "12.0"
        let v3 = new Vertex "85.0"
        let v4 = new Vertex "7.0"
        let e1 = new Edge(v1, v2)
        let e2 = new Edge(v3, v4)
        let e3 = new Edge(v4, v1)
        let g = new DirectedGraph([v1; v2; v3; v4;], [e1; e2; e3;])

        let edges = g.getEdges()
        for e in edges do
            printfn "%s" (string(e.src.GetType()))
            printfn "%s" (string(e))


        g

    let factor() =
        let v1 = new Variable("f", 1.0)
        let v2 = new Variable("b", 0.0)
        let v3 = new Variable("a", 0.5)

        let v4 = new Variable("e", 1.0)

        v4.value <- 0.0

        let v5 = new Variable("e1", 0.25)

        let s = [v1;v2;v3]

        let f1 = new Factor(s, Adder)
        let f2 = new Factor([v4;v5], Adder)

        let fg = FactorGraph([f1;f2])
        fg

module variable_test = 
    let distributions() =
        let u = NormalDist()
        let u2 = NormalDist(2, 3)
        let u3 = Uniform(0,5)
        let u4 = UniformCont(0, 5)

        u4.sample()

module markov_grid_test = 
    let simple() =
        printfn "MAKING THE GRID"
        let g = new MarkovGrid(3,3)
        let u = g.nodes

        printfn "WE MADE THE GRID"
        