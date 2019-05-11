namespace Elmish.UrlBuilder

[<RequireQualifiedAccess>]
module internal Helper =
    /// **Description**
    ///
    /// Get all of the indexes for a substring in another string.
    ///
    /// ```
    /// Helper.indexes "i" "Mississippi" = [ 1; 4; 7; 10 ]
    /// Helper.indexes "ss" "Mississippi" = [ 2; 5 ]
    /// Helper.indexes "needle" "haystack" = []
    /// ```
    ///
    /// **Parameters**
    ///   * `sub` - parameter of type `string`
    ///   * `str` - parameter of type `string`
    ///
    /// **Output Type**
    ///   * `int list`
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

    /// **Description**
    ///
    /// Take a substring given a start and end index.
    /// Negative indexes are taken starting from the `end` of the list.
    ///
    /// ```
    /// Helper.slice 7 9 "snakes on a plane!" = "on"
    /// Helper.slice 0 6 "snakes on a plane!" = "snakes"
    /// Helper.slice 0 -7 "snakes on a plane!" = "snakes on a"
    /// Helper.slice -6 -1 "snakes on a plane!" = "plane"
    /// ```
    ///
    /// **Parameters**
    ///   * `start` - parameter of type `int`
    ///   * `end` - parameter of type `int`
    ///   * `str` - parameter of type `string`
    ///
    /// **Output Type**
    ///   * `string`
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

    /// **Description**
    ///
    /// Drop `n` characters from the left side of a string.
    ///
    /// ```
    /// Helper.dropLeft 2 "The Lone Gunmen" = "e Lone Gunmen"
    /// Helper.dropLeft 0 "The Lone Gunmen" = "The Lone Gunme"
    /// Helper.dropLeft -1 "The Lone Gunmen" = "The Lone Gunmen"
    /// ```
    /// **Parameters**
    ///   * `n` - parameter of type `int`
    ///   * `str` - parameter of type `string`
    ///
    /// **Output Type**
    ///   * `string`
    let dropLeft (n : int) (str : string) : string =
        if n < 1 then str
        else slice n str.Length str

    /// **Description**
    ///
    /// Reduce a string from the left.
    ///
    /// ```
    /// Helper.left 2 "Mulder" = "Mu"
    /// Helper.left 0 "Mulder" = ""
    /// Helper.left -1 "Mulder" = ""
    /// ```
    ///
    /// **Parameters**
    ///   * `n` - parameter of type `int`
    ///   * `str` - parameter of type `string`
    ///
    /// **Output Type**
    ///   * `string`
    let left (n : int) (str : string) =
        if n < 1 then ""
        else slice 0 n str

    /// **Description**
    ///
    /// Try cast string to int.
    ///
    /// ```
    /// Helper.toInt "1" = Some 1
    /// Helper.toInt "a" = None
    /// ```
    ///
    /// **Parameters**
    ///   * `str` - parameter of type `string`
    ///
    /// **Output Type**
    ///   * `Option<int>`
    let toInt (str : string) : Option<int> =
        match System.Int32.TryParse str with
        | true, num -> Some num
        | _ -> None
