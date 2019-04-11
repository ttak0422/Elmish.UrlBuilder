namespace U

[<RequireQualifiedAccess>]
module Helper =
    let indexes (sub : string) (str : string) : int list =
        let subLen = sub.Length
        if subLen < 1 then []
        else
            let mutable i = 0
            let mutable is = []
            while ((i <- str.IndexOf(sub, i)
                    i) > -1) do
                is <- i :: is
                i <- i + subLen
            is |> List.rev

    let slice (start : int) (``end`` : int) (str : string) : string =
        let len = str.Length

        let start =
            if start >= 0 then start
            else len + start

        let ``end`` =
            if ``end`` >= 0 then ``end``
            else len + ``end``

        let ``end`` =
            if ``end`` > len then len
            else ``end``

        if start >= len || start >= ``end`` then ""
        else
            let takeNum = ``end`` - start
            str.Substring(start, takeNum)

    let dropLeft (n : int) (str : string) : string =
        if n < 1 then str
        else slice n str.Length str

    let left (n : int) (str : string) =
        if n < 1 then ""
        else slice 0 n str

    let toInt (str : string) : Option<int> =
        match System.Int32.TryParse str with
        | true, num -> Some num
        | _ -> None
