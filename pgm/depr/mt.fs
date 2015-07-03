namespace pgm


module mt = 

    open System.Collections.Generic
    open pgm.nlp_utils

    let english_corpus = "C:\Users\dd\Dropbox\pgm\pgm\data\mt\europarl-v7.de-en.en"
    let german_corpus = "C:\Users\dd\Dropbox\pgm\pgm\data\mt\europarl-v7.de-en.de"
    let SENTENCES = 1000

    // corpora
    let ec = Seq.take SENTENCES (readLines english_corpus) |> List.ofSeq |> List.map clean_corpus
    let gc = Seq.take SENTENCES (readLines german_corpus) |> List.ofSeq |> List.map clean_corpus

    let get_words (s:System.String) = s.Split([|' '|])

    let e_vocab = get_vocabulary' ec
    let f_vocab = get_vocabulary' gc

    //let dd = get_frequencies (String.concat " " ec)
    
    

    printfn "English vocab size: %d" e_vocab.Count


    // IBM Model 1
    // TODO: use mappings of integers internally to speed things up
    let IBM1 (sp_e:string list) (sp_f:string list) = 

        let e_vocab = get_vocabulary' sp_e
        let f_vocab = get_vocabulary' sp_f

        let t = new Dictionary<string, Dictionary<string, double>>() //Map.empty

        // initialize t(e|f) uniformly
        let initial_prob = 1. / double(e_vocab.Count)
        for e in e_vocab do
            t.Add(e, new Dictionary<string, double>())
            for f in f_vocab do
                t.[e].Add(f, initial_prob)

        // do while not converged
        let iteration = ref 0
        let argmax_sum = ref 0.
        let converged = ref false
        while(not !converged && !iteration < 30) do

            printfn "iteration number %d" !iteration
            //initialize //TODO: More functional
            let count = new Dictionary<string, Dictionary<string, double>>()
            for e in e_vocab do
                count.Add(e, new Dictionary<string, double>())
                for f in f_vocab do
                    count.[e].Add(f, 0.)
            let total = new Dictionary<string, double>()
            for f in f_vocab do
                total.Add(f, 0.)

            // for all sentence pairs 
            for (sent_e, sent_f) in Seq.zip sp_e sp_f do
                let ew = get_words sent_e
                let fw = get_words sent_f
                // total probability of an english word in this sentence
                let s_total = new Dictionary<string, double>()
           

                // compute normalization
                for e in Seq.distinct ew |> List.ofSeq  do //initialize each word once
                    s_total.Add(e, 0.) // see if success?
                    for f in fw do
                        s_total.[e] <- s_total.[e] + t.[e].[f] 

                // collect counts
                for e in ew do
                    for f in fw do
                        count.[e].[f] <- count.[e].[f] + t.[e].[f] / s_total.[e] 
                        //printfn "total before: %f" total.[f] 
                        total.[f] <- total.[f] + t.[e].[f] / s_total.[e]
                        //printfn "total after: %f" total.[f] 

                // estimate probabilities
                for f in fw do
                    for e in ew do
                        t.[e].[f] <- count.[e].[f] / total.[f]

            iteration := !iteration + 1

            // print sum of largest
            let new_amsum = (List.reduce (+) (List.max (List.map (fun e -> t.[e].Values |> List.ofSeq) (List.ofSeq e_vocab))))
            printfn "Old argmax sum: %f vs new %f" !argmax_sum new_amsum

            if (abs(!argmax_sum - new_amsum) < 0.01) then do
                converged := true
            else
                argmax_sum := new_amsum
        
        // return translation probabilities t(e|f)
        t

    let d1 = IBM1 ec gc

    let most_likely_t f = 

        if f_vocab.Contains(f) then
            
            let am x y = if d1.[x].[f] > d1.[y].[f] then x else y
            List.reduce (am) (List.ofSeq(e_vocab))
        else
            "OOV"

    let most_likely_s s =
        List.map most_likely_t s