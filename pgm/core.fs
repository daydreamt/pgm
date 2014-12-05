namespace pgm

module Base =

    open Graph
    open System.Collections

    let Adder (l: seq<Vertex>) =
        Seq.reduce (fun x y -> x + y) (Seq.map (fun (vv:Vertex) -> vv.v) l)

    // Factors consist of a seq of nodes and a factor function
    type Factor(nodes: seq<Vertex>, f:(seq<Vertex> -> double)) =
        member x.nodes = nodes
        member x.f = f

    type FactorGraph(s:seq<Factor>) =
        // all nodes?
        member x.nodes =  List.ofSeq (new Set<Vertex>(Seq.collect (fun (f:Factor) -> f.nodes) s))
        // reconstruct the whole graph?

