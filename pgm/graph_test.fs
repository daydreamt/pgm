namespace pgm_test

open pgm.Graph


module graph_test =

    let simple() = 
        
        
        let v1 = new Vertex 5
        let v2 = new Vertex 12
        let v3 = new Vertex 85
        let v4 = new Vertex 7
        let e1 = new Edge(v1, v2)
        let e2 = new Edge(v3, v4)
        let e3 = new Edge(v4, v1)
        let g = new DirectedGraph([v1; v2; v3; v4;], [e1; e2; e3;])

        let edges = g.getEdges()
        for e in edges do
            printfn "%s" (string(e.src.GetType()))
            printfn "%s" (string(e))


        g