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

    // Quickgraph edges
    type QEdge<'a> = 
        | IEdge of IEdge<'a>
        | UndirectedEdge of UndirectedEdge<'a>

    type Edge(src: Vertex, dst: Vertex) = class
        member x.src = src
        member x.dst = dst
        member x.directed = false
        override x.ToString() = string(x.src) + (if x.directed then "->" else "-") + string(x.dst)
        //member x.toQEdge(): QEdge<'a> = match x.directed with
        //| true -> QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        //| false -> QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
        member x.toQDEdge() = QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        member x.toUDEdge() = QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
            
    end
    
    type Graph = class
        //TODO: Different types of storage for graph objects
        val vertices: seq<Vertex>
        val edges: seq<Edge>
        val g : obj
        // member x.raw = QuickGraph

        new (v: seq<Vertex>, e: seq<Edge>) = {
            vertices = v
            edges = e
            g = null//new QuickGraph.UndirectedGraph<(Seq.head v).GetType()), string>()
        }

        member x.getEdges() = 
            seq { for e in x.edges do
                    yield e }

        member x.getVertices() = 
            seq { for v in x.vertices do
                    yield v }
    end
