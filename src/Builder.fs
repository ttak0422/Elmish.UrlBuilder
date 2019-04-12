namespace Elmish.UrlBuilder

open Url

type Root =
    | Absolute
    | Relative
    | CrossOrigin of string

type QueryPatameter = QueryPatameter of keyValue : string * string

module Builder =
    module Internal =
        let toQueryPair (queryParameter : QueryPatameter) : string =
            match queryParameter with
            | QueryPatameter(key, value) -> key + "=" + value

        let rootToPrePath (root : Root) : string =
            match root with
            | Absolute -> "/"
            | Relative -> ""
            | CrossOrigin prePath -> prePath + "/"

    /// **Description**
    ///
    /// Create a percent-encoded query parameter.
    ///
    /// ```
    /// absolute [ "products" ] [ str "search" "hat" ]
    /// (* "/products?search=hat" *)
    ///
    /// absolute [ "products" ] [ str "search" "coffee table" ]
    /// (*  "/products?search=coffee%20table" *)
    /// ```
    ///
    /// **Parameters**
    ///   * `key` - parameter of type `string`
    ///   * `value` - parameter of type `int`
    ///
    /// **Output Type**
    ///   * `QueryPatameter`
    let str (key : string) (value : string) : QueryPatameter =
        QueryPatameter(percentEncode key, percentEncode value)

    /// **Description**
    ///
    /// Create a percent-encoded query parameter.
    ///
    /// ```
    /// absolute ["products"] [ str "search" "hat"; int "page" 2 ]
    /// (*  "/products?search=hat&page=2" *)
    /// ```
    ///
    /// **Parameters**
    ///   * `key` - parameter of type `string`
    ///   * `value` - parameter of type `int`
    ///
    /// **Output Type**
    ///   * `QueryPatameter`
    let i32 (key : string) (value : int) : QueryPatameter =
        QueryPatameter(percentEncode key, (string value))

    /// **Description**
    ///
    /// Convert a list of query parameters to a percent-encoded query.
    /// This function is used by `absolute` , `relative`, etc.
    ///
    /// ```
    /// toQuery [ str "search" "hat" ]
    /// (* "?search=hat" *)
    ///
    /// toQuery [ str "search" "coffee table" ]
    /// (* "?search=coffee%20table" *)
    ///
    /// toQuery [ str "search" "hat"; i32 "page" 2 ]
    /// (* "?search=hat&page=2" *)
    ///
    /// toQuery []
    /// (* "" *)
    /// ```
    ///
    /// **Parameters**
    ///   * `parameters` - parameter of type `QueryPatameter list`
    ///
    /// **Output Type**
    ///   * `string`
    let toQuery (parameters : QueryPatameter list) : string =
        match parameters with
        | [] -> ""
        | _ ->
            "?" + String.concat "&" (List.map Internal.toQueryPair parameters)

    /// **Description**
    ///
    /// Create an absolute URL
    ///
    /// ```
    /// absolute [] []
    /// (* "/" *)
    ///
    /// absolute [ "packages"; "elmish"; "elmish" ] []
    /// (* "/packages/elmish/elmish" *)
    ///
    /// absolute [ "blog"; string 42 ] []
    /// (* "/blog/42" *)
    ///
    /// absolute [ "products" ] [ str "search" "hat"; i32 "page" 2 ]
    /// (* "/products?search=hat&page=2" *)
    /// ```
    ///
    /// **Parameters**
    ///   * `pathSegments` - parameter of type `string list`
    ///   * `parameters` - parameter of type `QueryPatameter list`
    ///
    /// **Output Type**
    ///   * `string`
    let absolute (pathSegments : string list) (parameters : QueryPatameter list) : string =
        "/" + String.concat "/" pathSegments + toQuery parameters

    /// **Description**
    ///
    /// Create a relative URL
    ///
    /// ```
    /// relative [] []
    /// (* "" *)
    ///
    /// relative [ "elmish"; "elmish" ] []
    /// (* "elmish/elmish" *)
    ///
    /// relative [ "blog"; string 42 ] []
    /// (* "blog/42" *)
    ///
    /// relative [ "products" ] [ str "search" "hat"; i32 "page" 2 ]
    /// (* "products?search=hat&page=2" *)
    ///
    /// ```
    ///
    /// **Parameters**
    ///   * `pathSegments` - parameter of type `string list`
    ///   * `parameters` - parameter of type `QueryPatameter list`
    ///
    /// **Output Type**
    ///   * `string`
    let relative (pathSegments : string list) (parameters : QueryPatameter list) : string =
        String.concat "/" pathSegments + toQuery parameters

    /// **Description**
    ///
    /// Crate a cross-origin URL
    ///
    /// ```
    /// crossOrigin "https://example.com" [ "products" ] []
    /// (* "https://example.com/products" *)
    ///
    /// crossOrigin "https://example.com" [] []
    /// (* "https://example.com/" *)
    ///
    /// crossOrigin "https://example.com:8042" [ "over"; "there" ] [ str "name" "ferret" ]
    /// (* "https://example.com:8042/over/there?name=ferret" *)
    /// ```
    ///
    /// **Parameters**
    ///   * `prePath` - parameter of type `string`
    ///   * `pathSegments` - parameter of type `string list`
    ///   * `parameters` - parameter of type `QueryPatameter list`
    ///
    /// **Output Type**
    ///   * `string`
    let crossOrigin (prePath : string) (pathSegments : string list)
        (parameters : QueryPatameter list) : string =
        prePath + "/" + String.concat "/" pathSegments + toQuery parameters

    /// **Description**
    ///
    /// Create custom URLs that may have a hash on the end
    ///
    /// ```
    /// custom Absolute
    ///     [ "packages"; "elm"; "core"; "latest"; "String" ]
    ///     []
    ///     (Some "length")
    /// (* "/packages/elm/core/latest/String#length" *)
    ///
    /// custom Relative
    ///     [ "there" ]
    ///     [ str "name" "ferret" ]
    ///     None
    /// (* "there?name=ferret" *)
    ///
    /// custom
    ///     (CrossOrigin "https://example.com:8042")
    ///     [ "over"; "there" ]
    ///     [ str "name" "ferret" ]
    ///     (Some "nose")
    /// (* "https://example.com:8042/over/there?name=ferret#nose" *)
    /// ```
    ///
    /// **Parameters**
    ///   * `root` - parameter of type `Root`
    ///   * `pathSegments` - parameter of type `string list`
    ///   * `parameters` - parameter of type `QueryPatameter list`
    ///   * `maybeFragment` - parameter of type `Option<string>`
    ///
    /// **Output Type**
    ///   * `string`
    let custom (root : Root) (pathSegments : string list)
        (parameters : QueryPatameter list) (maybeFragment : Option<string>) : string =
        let fragmentless =
            Internal.rootToPrePath root + String.concat "/" pathSegments
            + toQuery parameters
        match maybeFragment with
        | None -> fragmentless
        | Some fragment -> fragmentless + "#" + fragment
