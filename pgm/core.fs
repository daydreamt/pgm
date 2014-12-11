namespace pgm

module Base =
    open Graph
    open System.Collections
    open MathNet.Numerics.Distributions

    (* Distributions take parameters, and have methods that sample given those parameters

    TODO: If I could somehow generate them automatically from MathNet.Numerics.Distributions, that would be neat.

    The thing is, to use them in the Variable type, their constructor must be the same, right?
    I also don't want to type floats everywhere where a implicit conversion from int makes sense.
    Can you do an automatic conversion in the distributions type like that?
    
    Like like by converting the keywords to the names in the MathNet.Numerics.Distributions constructors 
    for example.
    
    Anyway, for now I have total control of this little subset to think it over..
    *)


    [<AbstractClass>]
    type IDist(parameters: Map<string, double>) = class

        member x.parameters = parameters

        // really is sample
        abstract sample: unit -> double
    end

    type NormalDist(parameters: Map<string, double>) = class
        inherit IDist(parameters)
        //member x.parameters = parameters
        member x.d = Normal(parameters.["mean"], parameters.["stddev"])
        override x.sample() = x.d.Sample()

        new (μ:double, stddev:double) = NormalDist(["mean", μ; "stddev", stddev] |> Map.ofList)
        new (μ:int, stddev:int) = NormalDist(double(μ), double(stddev))
        new () = NormalDist(0.0, 1.0)
    end

    type Bernoulli(parameters: Map<string, double>) = class
        inherit IDist(parameters)

        member x.d = MathNet.Numerics.Distributions.Bernoulli(parameters.["p"])
        override x.sample() = double(x.d.Sample())

        new (p:double) = Bernoulli(["p", p] |> Map.ofList)
        new (p:int) = Bernoulli(double(p))

    end

    type Uniform(parameters: Map<string, double>)= class
        inherit IDist(parameters)

        member x.d = MathNet.Numerics.Distributions.DiscreteUniform(int(parameters.["lower"]), int(parameters.["upper"]))
        override x.sample() = double(x.d.Sample())

        new (lower:double, upper:double) = Uniform(["lower", lower; "upper", upper] |> Map.ofList)
        new (lower:int, upper:int) = Uniform(double(lower), double(upper))

    end

    type UniformCont(parameters: Map<string, double>)= class
        inherit IDist(parameters)

        member x.d = MathNet.Numerics.Distributions.ContinuousUniform(parameters.["lower"], parameters.["upper"])
        override x.sample() = x.d.Sample()

        new (lower:double, upper:double) = UniformCont(["lower", lower; "upper", upper] |> Map.ofList)
        new (lower:int, upper:int) = UniformCont(double(lower), double(upper))

    end
        
    (*
    Variables are building blocks in pgm.

    They contain current values, as well as keep references to their inputs.

    
    the domain of the variable,
    the distribution type of the variable (can be uknown)
    parameters of the distribution type                 3)Parameters

    They currently mostly wrap MathNet.Numerics.Distributions
    *)
    
    type Domain(min:Numeric, max:Numeric) = 
        member x.min = min
        member x.max = max

    type Variable(name:string, dom:Domain, value:Numeric) = 
        member x.value = value
        member x.domain = dom

    // OpenGM defines them like this
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