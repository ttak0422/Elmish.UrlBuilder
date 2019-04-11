namespace U

open System
open Fable.Import

type Protocol =
    | Http
    | Https

type Url =
    { Protocol : Protocol
      Host : string
      Port : Option<int>
      Path : string
      Query : Option<string>
      Fragment : Option<string> }

module Url =
    module Internal =
        let addPort (maybePort : Option<int>) (starter : string) : string =
            match maybePort with
            | None -> starter
            | Some port_ -> starter + ":" + (string port_)

        let addPrefixed (prefix : string) (maybeSegment : Option<string>)
            (starter : string) : string =
            match maybeSegment with
            | None -> starter
            | Some segment -> starter + prefix + segment

        let chompBeforePath (protocol : Protocol) (path : string)
            (``params`` : Option<string>) (frag : Option<string>) (str : string) : Option<Url> =
            if String.IsNullOrEmpty str || str.Contains "@" then None
            else
                match Helper.indexes ":" str with
                | [] ->
                    Some { Protocol = protocol
                           Host = str
                           Port = None
                           Path = path
                           Query = ``params``
                           Fragment = frag }
                | [ i ] ->
                    match Helper.toInt (Helper.dropLeft (i + 1) str) with
                    | None -> None
                    | port ->
                        Some { Protocol = protocol
                               Host = Helper.left i str
                               Port = port
                               Path = path
                               Query = ``params``
                               Fragment = frag }
                | _ -> None

        let chompBeforeQuery (protocol : Protocol) (``params`` : Option<string>)
            (frag : Option<string>) (str : string) : Option<Url> =
            if String.IsNullOrEmpty str then None
            else
                match Helper.indexes "/" str with
                | [] -> chompBeforePath protocol "/" ``params`` frag str
                | i :: _ ->
                    chompBeforePath protocol (Helper.dropLeft i str) ``params``
                        frag (Helper.left i str)

        let chompBeforeFragment (protocol : Protocol) (frag : Option<string>)
            (str : string) : Option<Url> =
            if String.IsNullOrEmpty str then None
            else
                match Helper.indexes "?" str with
                | [] -> chompBeforeQuery protocol None frag str
                | i :: _ ->
                    chompBeforeQuery protocol
                        (Some(Helper.dropLeft (i + 1) str)) frag
                        (Helper.left i str)

        let chompAfterProtocol (protocol : Protocol) (str : string) : Option<Url> =
            if String.IsNullOrEmpty str then None
            else
                match Helper.indexes "#" str with
                | [] -> chompBeforeFragment protocol None str
                | i :: _ ->
                    chompBeforeFragment protocol
                        (Some(Helper.dropLeft (i + 1) str)) (Helper.left i str)

    let toString (url : Url) : string =
        let http =
            match url.Protocol with
            | Http -> "http://"
            | Https -> "https://"
        Internal.addPort url.Port (http + url.Host) + url.Path
        |> Internal.addPrefixed "?" url.Query
        |> Internal.addPrefixed "#" url.Fragment

    let fromString (str : string) : Option<Url> =
        if str.StartsWith "http://" then
            Internal.chompAfterProtocol Http (Helper.dropLeft 7 str)
        elif str.StartsWith "https://" then
            Internal.chompAfterProtocol Https (Helper.dropLeft 8 str)
        else None

    let percentEncode (str : string) : string = JS.encodeURIComponent str

    let percentDecode (str : string) : Option<string> =
        try
            Some(JS.decodeURIComponent str)
        with _ -> None
