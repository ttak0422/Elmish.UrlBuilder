namespace U

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

    let i32 (key : string) (value : int) : QueryPatameter =
        QueryPatameter(percentEncode key, (string value))
    let str (key : string) (value : string) : QueryPatameter =
        QueryPatameter(percentEncode key, percentEncode value)

    let toQuery (parameters : QueryPatameter list) : string =
        match parameters with
        | [] -> ""
        | _ ->
            "?" + String.concat "&" (List.map Internal.toQueryPair parameters)

    let absolute (pathSegments : string list) (parameters : QueryPatameter list) : string =
        "/" + String.concat "/" pathSegments + toQuery parameters
    let relative (pathSegments : string list) (parameters : QueryPatameter list) : string =
        String.concat "/" pathSegments + toQuery parameters
    let crossOrigin (prePath : string) (pathSegments : string list)
        (parameters : QueryPatameter list) : string =
        prePath + "/" + String.concat "/" pathSegments + toQuery parameters

    let custom (root : Root) (pathSegments : string list)
        (parameters : QueryPatameter list) (maybeFragment : Option<string>) : string =
        let fragmentless =
            Internal.rootToPrePath root + String.concat "/" pathSegments
            + toQuery parameters
        match maybeFragment with
        | None -> fragmentless
        | Some fragment -> fragmentless + "#" + fragment
