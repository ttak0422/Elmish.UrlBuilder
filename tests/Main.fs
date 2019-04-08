module Tests.Main

open Fable.Core
open Util.Testing

[<Global>]
let describe (name : string) (f : unit -> unit) = jsNative

[<Global>]
let it (msg : string) (f : unit -> unit) = jsNative

let run() =
    let tests = [ Helper.tests; Url.tests; Builder.tests ] :> Test seq
    for (moduleName, moduleTests) in tests do
        describe moduleName <| fun () ->
            for (name, tests) in moduleTests do
                describe name <| fun () ->
                    for (msg, test) in tests do
                        it msg test

run()
