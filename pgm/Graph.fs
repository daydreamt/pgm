namespace pgm
(*
contains the Graph data structures, handles the wrappers with the underlying
 (hopefully blazing fast) graph library. 

Desired API:

let g = new Graph [List of Vertices] [List of Edges]
let g = new Graph [List of Edges]
let g = new Graph [[1; 2]; [4;1], [5;2] ... ]
let g = new Geaph [[1; 2; 0.1]; [2; 1; 0.3;] ... ]

*)

open System
open QuickGraph
open System.Collections.Generic
open MathNet.Numerics.Distributions

//QuickGraph.Edge
module Graph =

    // TODO? 
    type Numeric = 
        | Int of int
        | Float of float
        | Double of double

    type Vertex(name: string, v:double) = 
        member x.name = name
        member x.v = v
        member x.toQuickVertex() = x.v
        override x.ToString() = name + " : " + string(x.v)

        override x.Equals(other) =
            match other with
            | :? Vertex as vv -> (x.name = vv.name)
            | _ -> false

        interface System.IComparable with
            // lexicographical ordering by name
            member x.CompareTo(other) =
                match other with 
                | :? Vertex as sc -> compare (string(x)) (string(sc))
                | _ -> invalidArg "Vertex" "cannot compare Vertex with that type"

        override x.GetHashCode() =
            x.toQuickVertex().GetHashCode()

        //new(n:string, v:string) = Vertex(n,v, IDistribution)
        new(n:string) = Vertex(name=n, v=0.0)

    type Vertex<'T> = Vertex of 'T

    type Edge(src: Vertex, dst: Vertex) = class
        member x.src = src
        member x.dst = dst
        member x.directed = false
        member x.weighted = false
        override x.ToString() = string(x.src) + (if x.directed then "->" else "-") + string(x.dst)
        member x.toQDEdge() = QuickGraph.Edge(src.toQuickVertex(), dst.toQuickVertex())
        member x.toUDEdge() = QuickGraph.UndirectedEdge(src.toQuickVertex(), dst.toQuickVertex())
    end

    type WeightedEdge(src: Vertex, dst: Vertex, value: double) =
        inherit Edge(src, dst)
        member x.value = value
        member x.weighted = true

    //TODO: Only keep edges and vertices once, namely in the graph and read from there
    type IGraph(vertices: seq<Vertex>, edges: seq<Edge>) =

        member this.vertices = vertices
        member this.edges = edges

        member this.directed = false
        member this.raw = null
                
        member this.getEdges() = 
            seq { for e in this.edges do yield e }

        member this.getVertices() = 
            seq { for v in this.vertices do yield v }

    type DirectedGraph(vertices: seq<Vertex>, edges: seq<Edge>) =
        inherit IGraph(vertices, edges)
        member this.directed = true

        member this.raw = 

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

