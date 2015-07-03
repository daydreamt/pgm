namespace pgm

open pgm.ml_useful

module nlp_utils =
    open System.Collections.Generic

    let readLines filePath = System.IO.File.ReadLines(filePath)
    // poor man's tokenization, poor man has not trained a tokenizer
    let stripchars chars str =
        Seq.fold (fun (str: string) chr -> str.Replace(chr |> System.Char.ToUpper |> string, "").Replace(chr |> System.Char.ToLower |> string, "")) str chars
    let punc = ['('; ')'; '.'; ','; ';'; '['; ']'; '{'; '}'; '''; '"';]
    let clean_corpus (s:string) = stripchars punc (s.ToLower())

    let get_vocabulary (s:string):Set<string> = s |> (fun (w:System.String) -> w.Split([|' '|]))
                                                       |> List.ofArray |> Set.ofList

    let get_vocabulary' (ll:string list):Set<string> = ll |> String.concat " " |> get_vocabulary
    let get_frequencies (s:string) =  s |> (fun (w:System.String) -> w.Split([|' '|]))
                                                       |> Seq.countBy id

    //given a list of key, val pairs returns the top n
    let get_top_frequencies (n:int) (s:string) = 
        let sor = s |> get_frequencies |> Seq.sortBy (fun (k,v) -> v)
        // return them in descending order
        Seq.skip ((Seq.length sor) - n) sor |> Seq.sortBy (fun (k,v) -> -v)

    // encode a vocabulary to integers: uint16 takes values from 0 (for OOV) to 65535.
    let get_encoding freqs = 
        assert ((Seq.length freqs) < 65535)
        Seq.mapi (fun i x -> (x, i + 1)) freqs |> Map.ofSeq

    type Vocabulary (sz:int, s:string) =
        
        let freq = get_top_frequencies sz s

        // words are sorted by frequency, most frequent first
        // An array allows O(1) word encoding to string translation
        // and O(n) string to array. Good for pxm, possibly bad for training.
        member this.words = Seq.map fst freq |> Array.ofSeq
        member this.Length = uint32(this.words.Length)

        member this.get_vocabulary() = this.words
        member this.get_length() = this.Length

        member this.decode(n:uint32) = if (0u < n && n < this.Length) then  this.words.[int(n)] else "OOV"
        member this.decode(n:uint32 list) = List.map (fun (i:uint32) -> this.decode i) n
        member this.decode(n:int) = this.decode(uint32(n))
        member this.decode(n:int list) = this.decode(List.map uint32 n)
        
        member this.encode(s:string) = 
            Array.findIndex (fun x -> x = s) this.words

        // Lazy!
        member this.encode(s:string list) = 
            lazy (List.map (fun (x:string) -> this.encode x) s)

        new(s:string) = Vocabulary(1000, s)
        // TODO: Make lazy too, or for file handles
        new(sl:string list, n:int) = Vocabulary (n, (String.concat " " sl))
