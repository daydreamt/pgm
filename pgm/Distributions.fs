namespace pgm
    
    (* Distributions take parameters, and have a method that sample given those parameters.
    
    They are guaranteed to have a standard constructor parameters: Map<string, double>, as well as nicer ones.

    TODO: If I could somehow generate them automatically from MathNet.Numerics.Distributions, that would be neat.

    The thing is, to use them in the Variable type, their constructor must be the same, right?
    I also don't want to type floats everywhere where a implicit conversion from int makes sense.
    Can you do an automatic conversion in the distributions type like that?
    
    Like by converting the keywords to the names in the MathNet.Numerics.Distributions constructors 
    for example.
    
    Anyway, for now I have total control of this little subset to think it over.
    *)


module Distributions = 
    
    open MathNet.Numerics.Distributions


    [<AbstractClass>]
    type IDist(parameters: Map<string, double>) = class

        member x.parameters = parameters

        // really is sample
        abstract sample: unit -> double
    end

    type NormalDist(parameters: Map<string, double>) = class
        inherit IDist(parameters)
        //member x.parameters = parameters
        member x.d = MathNet.Numerics.Distributions.Normal(parameters.["mean"], parameters.["stddev"])
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
