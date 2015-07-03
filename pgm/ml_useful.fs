namespace pgm

module ml_useful =
    // but also dirty

    open MathNet.Numerics
    open MathNet.Numerics.LinearAlgebra
    open System

    type Perceptron(weights:double list) = class
        
        // oh the dirt
        let dotproduct x y = List.zip x y |> List.map (fun (w,i) -> w*i ) |>  List.sum
        let vsum (x:double list) (y:double list) = List.zip x y |> List.map (fun (w,i) -> w + i )
        let vscalar x (y:float) = List.map (fun l -> y * l) x

        let mutable w = List.append [0.] weights
        member this.weights with get() = w
        member this.weights with set(v) = w <- v

        member this.n = weights.Length
        // TODO: SCRAP: USE ARRAYS?
        new(weights:double array) = Perceptron(Vector<double>.Build.DenseOfArray(weights))
        new(weights:'T array) = Perceptron(Array.map double weights)
        new(n:int) = 
            let w = Vector<double>.Build.Random(n).ToArray()
            Perceptron(w)
        new(w:double seq) = Perceptron(List.ofSeq(w))

        member this.forward (x:'T List) =
            let inp = List.append [1.] (List.map double x)
            List.zip this.weights inp |> List.map (fun (w,i) -> w*i ) |>  List.sum

        member this.classify (x:'T List) = 
            if (this.forward x) >= 0. then 1 else -1

        member private this.is_correct inp y = 
            (int (this.classify inp)) = (int y)

        member this.update_by inp (y:double) =

            Console.WriteLine(sprintf "input before: %A" inp)
            Console.WriteLine(sprintf "Weights before: %A" this.weights)
            if not (this.is_correct inp y) then do

                let cur_output = this.classify inp
                printfn "CUrrent: %d Desired: %f" cur_output y
                let ninp = List.append [1.] inp
                //let r = dotproduct ninp y
                let new_weights =  vsum this.weights (vscalar ninp y)   
                this.weights <- new_weights
                Console.WriteLine(sprintf "Weights after: %A" this.weights)
            
        // returns a list of tuples (inp_i y_i)    
        member this.get_misclassified inps ys =
            List.zip inps ys |> List.filter (fun (x, y) -> not (this.is_correct x y))

        member this.get_classification_error inps ys = 
            let m = (this.get_misclassified inps ys).Length |> double
            let total = inps.Length |> double

            m / total

        // update weights by a single random missclassified element
        member this.update inps ys = 
            //printfn "Error before:%f" (this.get_classification_error inps ys)
            let miscl = this.get_misclassified inps ys

            // Single?
            if (miscl.Length > 0) then
                let (inp, y) = miscl.[0]
                this.update_by inp y
            
            // Batch?
            //for i in {0..miscl.Length - 1} do
            //  let (inp, y) = miscl.[i]
            //    this.update_by inp y
                
            //printfn "Error after:%f" (this.get_classification_error inps ys)

        member this.train inps ys =

            while (this.get_classification_error inps ys > 0.01) do
                this.update inps ys  
                printfn "New iteration..."
            
      end
    

        
    