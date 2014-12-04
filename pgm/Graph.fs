namespace pgm

(* contains the Graph data structures, handles the wrappers
Desired API:

Graph g = Graph [List of Vertices] [List of Edges]

*)

open QuickGraph
open System.Collections.Generic
//QuickGraph.Edge
module Graph =

    type Vertex(v:'a) = class
        member x.v = v
        member x.toQuickVertex() = x.v
        override x.ToString() = string(x.v)
    end

    type Edge(src: Vertex, dst: Vertex) = class
        member x.src = src
        member x.dst = dst
        member x.directed = false
        member x.weighted = false
        override x.ToString() = string(x.src) + (if x.directed then "->" else "-") + string(x.dst)
        member x.toQDEdge() = QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        member x.toUDEdge() = QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
    end

    type WeightedEdge(src: Vertex, dst: Vertex, value: 'a) =
        inherit Edge(src, dst)
        member x.value = value
        member x.weighted = true


    //TODO: Only keep edges and vertices once, namely in the graph and read from there
    type IGraph(vertices: seq<Vertex>, edges: seq<Edge>) =
        //abstract member vertices: seq<obj>
        //abstract member edges: seq<obj>

        member this.vertices = vertices
        member this.edges = edges

        member this.directed = false
        //abstract raw: obj
        member this.raw = null
                
        member this.getEdges() = 
            seq { for e in this.edges do yield e }

        member this.getVertices() = 
            seq { for v in this.vertices do yield v }

    type DirectedGraph(vertices: seq<Vertex>, edges: seq<Edge>) =
        inherit IGraph(vertices, edges)
        member this.directed = true

        member this.raw = 
            
            //TODO: Weighted graphs
            (*
            let edgeType = match (Seq.head this.edges).weighted with
                | true -> QuickGraph.TaggedEdge
                | false -> QuickGraph.Edge<
            *)
            let tmp = new QuickGraph.EdgeListGraph<_,QuickGraph.Edge<_>>()
            do tmp.AddVerticesAndEdgeRange( seq { for e in this.edges do yield new QuickGraph.Edge<_>(e.src, e.dst) }) |> ignore
            tmp

    type UndirectedGraph(vertices: seq<Vertex>, edges: seq<Edge>) =
        inherit IGraph(vertices, edges)
        member this.directed = false
        member this.raw = 
            let tmp = new QuickGraph.UndirectedGraph<_,QuickGraph.Edge<_>>()
            do tmp.AddVerticesAndEdgeRange( seq { for e in this.edges do yield new QuickGraph.Edge<_>(e.src, e.dst) }) |> ignore
            tmp

    //TODO: Kickass API
    type Graph = UndirectedGraph


    (*
    // Quickgraph edges
    type private QEdge = 
        | DirectedEdge of src: obj * dst : obj
        | UndirectedEdge of src: obj * dst : obj


    type myEdge(src: Vertex, dst: Vertex) = class
        member x.src = src
        member x.dst = dst
        member x.directed = false
        override x.ToString() = string(x.src) + (if x.directed then "->" else "-") + string(x.dst)
        //member x.toQEdge(): QEdge<'a> = match x.directed with
        //| true -> QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        //| false -> QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
        member x.toQDEdge() = QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        member x.toUDEdge() = QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
        //member x.toQEdge(): QEdge<'a> = match x.directed with
        //| true -> QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        //| false -> QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
            
    end

    let makeQEdge (e:myEdge) =
        match e.directed with
        | true -> QuickGraph.Edge(e.src.toQuickVertex(), e.dst.toQuickVertex())
        | false -> QuickGraph.UndirectedEdge(e.src.toQuickVertex(), e.dst.toQuickVertex())
    
    // Quickgraph graphs
    type private QGraph<'a> =
        | UndirectedGraph of UndirectedGraph<'a, IEdge<'a>>
        | EdgeListGraph of EdgeListGraph<'a, IEdge<'a>>

    *)


    (*
    type newGraph<'a> =
        | Undirected
        | Directed
        with
            member x.fromQ = function
                | UndirectedGraph<'a, IEdge> -> Undirected
                | EdgeListGraph<a,QuickGraph.Edge<_>> -> Directed
                |  _ -> failwith "no such skateboard model.."



    type private TMPGraph (v: seq<Vertex>, e: seq<myEdge>) = class

        member this.vertices = v
        member this.edges = e

        //member allDirected? = Seq.isEmpty (Seq.filter ())

        member this.isAllDirected = Seq.isEmpty (Seq.filter (fun (edge: myEdge) -> edge.directed) e)
    end


    let private mkGraph (g:TMPGraph ) = 
        let raw = new QuickGraph.UndirectedGraph<_,QuickGraph.Edge<_>>()
        //match g.isAllDirected with
        //    | false -> new QuickGraph.UndirectedGraph<_,QuickGraph.Edge<_>>()
        //    | true -> new QuickGraph.EdgeListGraph<_,QuickGraph.Edge<_>>()
        do raw.AddVerticesAndEdgeRange( seq { for e in g.edges do yield new QuickGraph.Edge<_>(e.src, e.dst) }) |> ignore
        raw

    type Graph(v: seq<Vertex>, e: seq<myEdge>) = class
        let tmp = new TMPGraph(v, e)

        member this.vertices = v
        member this.edges = e

        member this.getEdges() = 
            seq { for e in this.edges do yield e }

        member this.getVertices() = 
            seq { for v in this.vertices do yield v }

        //member this.raw:QGraph<_> = mkGraph(tmp)

        
    end

    type Graph = class
        //TODO: Different types of storage for graph objects
        val vertices: seq<Vertex>
        val edges: seq<Edge>

        val directed: bool

        member x.raw = new QuickGraph.UndirectedGraph<_,QuickGraph.Edge<_>>()

        new (v: seq<Vertex>, e: seq<Edge>) = {
            vertices = v
            edges = e

            directed =  Seq.isEmpty (Seq.filter (fun edge -> edge.directed) e)
            //g = null//new QuickGraph.UndirectedGraph<(Seq.head v).GetType(), string>()
        }

        member x.getEdges() = 
            seq { for e in x.edges do
                    yield e }

        member x.getVertices() = 
            seq { for v in x.vertices do
                    yield v }
    end
    *)
