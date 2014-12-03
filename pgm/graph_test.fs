namespace pgm_test

open pgm.Graph


module graph_test =

    let simple() = 
        
        let v1 = new Vertex 5
        let v2 = new Vertex 12
        let e1 = new Edge(v1, v2)
        let g = new Graph([v1; v2], [e1])

        let edges = g.getEdges()
        for e in edges do
            printfn "%s" (string(e.src.GetType()))
            printfn "%s" (string(e))


        g