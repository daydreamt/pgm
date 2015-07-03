namespace pgm

    (*
    Variables are building blocks in pgm.

    They contain current values, as well as keep references to their inputs.

    In PYMC they contain ancestors. NOT THAT IT MATTERS

    
    the domain of the variable,
    the distribution type of the variable (can be uknown)
    parameters of the distribution type                 3)Parameters

    They currently mostly wrap MathNet.Numerics.Distributions
    *)

module Base =
    open System.Collections
    open Distributions
    open System.Collections.Generic
        
    
    type Domain(min:double, max:double) = 
        member x.min = min
        member x.max = max

        new(min:int, max:int) = Domain(double(min), double(max))

        override x.ToString() = "(" + string(x.min) + "," + string(x.max) + ")"


    // variables are vertices
    type Variable(name:string, dom:Domain, value:double) = 
    //type Variable(name:string, value:double) = 

        // the current value of the variable
        let mutable value_internal:double = value
        // TODO: suboptimal
        let mutable fixed_inputs_internal = new Dictionary<string, Variable>()
        let mutable inputs_internal = new Dictionary<string, Variable>()
        let mutable connections_internal = List<Variable>()

        member x.name = name
        member x.value
            with get() = value_internal
            and set (v:double) =
                if v <> value then
                    value_internal <- v

        member x.fixedInputs with get() = fixed_inputs_internal
        member x.inputs with get() = inputs_internal
        member x.connections with get() = connections_internal
        member x.domain = dom

        // eg: mu or sigma^2
        member x.addFixedInput(name:string, value:Variable) =
            fixed_inputs_internal.Add(name, value)

        member x.addInput(other:Variable, desc:string) =
            inputs_internal.Add(desc, other)

        member x.addConnection(other: Variable) =
            connections_internal.Add(other)
        
        override x.Equals(other) =
            match other with
            | :? Variable as vv -> (x.name = vv.name)
            | _ -> false

        interface System.IComparable with
            // lexicographical ordering by name
            member x.CompareTo(other) =
                match other with 
                | :? Variable as sc -> compare (string(x)) (string(sc))
                | _ -> invalidArg "Variable" "cannot compare Variable with that type"

        override x.GetHashCode() =
            x.value.GetHashCode()
        override x.ToString() = x.name + ":" + string(x.value) + "∈" + string(x.domain) 

        
        new(name:string, value:double) = new Variable(name, new Domain(-1, 1), value)
        new(name:string) = new Variable(name, new Domain(0, 1), 0.0)
        
        new(name:string, value:int) = new Variable(name, new Domain(-1, 1), double(value))
        
    
         
    //type Variable<'T> = Variable of 'T

    ///////////////////////////////////////////////////////////////////
    // Factor Graphs
    //////////////////////////////////////////////////////////////////
    // OpenGM defines them like this
    let Adder (l: seq<Variable>) =
        Seq.reduce (fun x y -> x + y) (Seq.map (fun (vv:Variable) -> vv.value) l)

    // Factors consist of a seq of nodes and a factor function
    type Factor(nodes: seq<Variable>, f:(seq<Variable> -> double)) =
        member x.nodes = nodes
        member x.f = f

    type FactorGraph(s:seq<Factor>) =
        // all nodes?
        member x.nodes =  List.ofSeq (new Set<Variable>(Seq.collect (fun (f:Factor) -> f.nodes) s))
        // reconstruct the whole graph?

    //////////////////////////////////////////////////////////////////
    // Markov networks
    //////////////////////////////////////////////////////////////////
    // "Standard evaluation examples of BP algorithms is a markov grid"
    // a pairwise connected markov grid like on the residual belief propagation paper
    // with unary potential factors from [0,1] and ψ_{i,j}(x_i, x_j) = e^(λC) or e^(-λC)
    // depending on whether x_i == x_j.
    //type MarkovGrid(width:int, height:int, pairwise_potential_fn, unary_potential_fn) = class
    //type MarkovGrid(width:int, height:int, lambda, C) = class
    type MarkovGrid(width:int, height:int) = class

        let dom = new Domain(-1., 1.)
        member x.nodes =
            let nodes = new List<List<Variable>>()
            // initialize nodes
            for i in {0..(width - 1)} do
            
                nodes.Add(new List<Variable>())
                for j in {0 .. (height - 1)} do
                    let name = "n" + string(i) + string(j)
                    let curVal = double(j) - 1.;
                    nodes.[i].Add(Variable(name, dom, curVal))

            // add connections between neighbouring nodes
            for i in {0..(width - 1)} do
                for j in {0 .. (height - 1)} do
                    let curNode = nodes.[i].[j]
                    let offsets = [(0,-1); (-1,0)] //(0,1); (1,0);
                    for off in offsets do
                        let coords = (i + (fst off), j + (snd off))
                        let (x,y) = coords
                        if (x > 0 && y > 0) then do
                            let otherNode = nodes.[x].[y]
                            curNode.addConnection(otherNode)
                            otherNode.addConnection(curNode)
            nodes
        // save the unary potentials for every node
        member x.unaries = 
            let u = new List<double>()
            for i in {0.. (height * width)} do
                u.Add(MathNet.Numerics.Distributions.ContinuousUniform.Sample(0., 1.))
            u
        // pairwise potentials: What is C?

        member x.Energy() = 0
    end

    let u = MarkovGrid(3,3);
