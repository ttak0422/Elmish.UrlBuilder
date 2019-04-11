module Tests.Builder

open U
open U.Helper
open Util.Testing
open U.Builder
open Tests

let tests : Test =
    testList "Builder" [ testList "toQueryPair" [ testCase "search=hat" <| fun _ ->
                                                      let expected =
                                                          "search=hat"
                                                      let actual =
                                                          Internal.toQueryPair
                                                              (QueryPatameter
                                                                   ("search",
                                                                    "hat"))
                                                      equal expected actual ]
                         testList "rootToPrePath" [ testCase "abusolute" <| fun _ ->
                                                        let expected = "/"
                                                        let actual =
                                                            Internal.rootToPrePath
                                                                Absolute
                                                        equal expected actual
                                                    testCase "ralative" <| fun _ ->
                                                        let expected = ""
                                                        let actual =
                                                            Internal.rootToPrePath
                                                                Relative
                                                        equal expected actual
                                                    testCase "crossOrigin" <| fun _ ->
                                                        let expected =
                                                            "https://example.com:8042/"
                                                        let actual =
                                                            Internal.rootToPrePath
                                                                (CrossOrigin
                                                                     "https://example.com:8042")
                                                        equal expected actual ]
                         testList "int" [ testCase "simple" <| fun _ ->
                                              let expected =
                                                  QueryPatameter("page", "2")
                                              let actual = num "page" 2
                                              equal expected actual ]
                         testList "string" [ testCase "simple" <| fun _ ->
                                                 let expected =
                                                     QueryPatameter
                                                         ("search", "hat")
                                                 let actual = str "search" "hat"
                                                 equal expected actual ]
                         testList "toQuery" [ testCase "one int query" <| fun _ ->
                                                  let expected = "?page=2"
                                                  let actual =
                                                      toQuery [ num "page" 2 ]
                                                  equal expected actual
                                              testCase "one string query" <| fun _ ->
                                                  let expected = "?search=hat"
                                                  let actual =
                                                      toQuery
                                                          [ str "search" "hat" ]
                                                  equal expected actual
                                              testCase
                                                  "one string query : percentEncode" <| fun _ ->
                                                  let expected =
                                                      "?search=coffee%20table"
                                                  let actual =
                                                      toQuery
                                                          [ str "search"
                                                                "coffee table" ]
                                                  equal expected actual
                                              testCase "multiple queries" <| fun _ ->
                                                  let expected =
                                                      "?search=hat&page=2"

                                                  let actual =
                                                      toQuery [ str "search"
                                                                    "hat"
                                                                num "page" 2 ]
                                                  equal expected actual ]
                         testList "abusolute" [ testCase "simple case 1" <| fun _ ->
                                                    let expected = "/"
                                                    let actual = absolute [] []
                                                    equal expected actual
                                                testCase "simple case 2" <| fun _ ->
                                                    let expected =
                                                        "/packages/elmish/elmish"
                                                    let actual =
                                                        absolute
                                                            [ "packages";
                                                              "elmish"; "elmish" ]
                                                            []
                                                    equal expected actual
                                                testCase "simple case 3" <| fun _ ->
                                                    let expected = "/blog/42"

                                                    let actual =
                                                        absolute [ "blog"
                                                                   string 42 ]
                                                            []
                                                    equal expected actual
                                                testCase "simple case 4" <| fun _ ->
                                                    let expected =
                                                        "/products?search=hat&page=2"

                                                    let actual =
                                                        absolute [ "products" ]
                                                            [ str "search" "hat"
                                                              num "page" 2 ]
                                                    equal expected actual ]
                         testList "relative" [ testCase "simple case 1" <| fun _ ->
                                                   let expected = ""
                                                   let actual = relative [] []
                                                   equal expected actual
                                               testCase "simple case 2" <| fun _ ->
                                                   let expected =
                                                       "elmish/elmish"
                                                   let actual =
                                                       relative
                                                           [ "elmish"; "elmish" ]
                                                           []
                                                   equal expected actual
                                               testCase "simple case 3" <| fun _ ->
                                                   let expected = "blog/42"

                                                   let actual =
                                                       relative [ "blog"
                                                                  string 42 ] []
                                                   equal expected actual
                                               testCase "simple case 4" <| fun _ ->
                                                   let expected =
                                                       "products?search=hat&page=2"

                                                   let actual =
                                                       relative [ "products" ]
                                                           [ str "search" "hat"
                                                             num "page" 2 ]
                                                   equal expected actual ]
                         testList "crossOrigin" [ testCase "simple case 1" <| fun _ ->
                                                      let expected =
                                                          "https://example.com/products"
                                                      let actual =
                                                          crossOrigin
                                                              "https://example.com"
                                                              [ "products" ] []
                                                      equal expected actual
                                                  testCase "simple case 2" <| fun _ ->
                                                      let expected =
                                                          "https://example.com/"
                                                      let actual =
                                                          crossOrigin
                                                              "https://example.com"
                                                              [] []
                                                      equal expected actual
                                                  testCase "simple case 3" <| fun _ ->
                                                      let expected =
                                                          "https://example.com:8042/over/there?name=ferret"
                                                      let actual =
                                                          crossOrigin
                                                              "https://example.com:8042"
                                                              [ "over"; "there" ]
                                                              [ str "name"
                                                                    "ferret" ]
                                                      equal expected actual ]
                         testList "custom" [ testCase "simple case abusolute" <| fun _ ->
                                                 let expected =
                                                     "/packages/elm/core/latest/String#length"
                                                 let actual =
                                                     custom Absolute
                                                         [ "packages"; "elm";
                                                           "core"; "latest";
                                                           "String" ] []
                                                         (Some "length")
                                                 equal expected actual
                                             testCase "simple case relative" <| fun _ ->
                                                 let expected =
                                                     "there?name=ferret"
                                                 let actual =
                                                     custom Relative [ "there" ]
                                                         [ str "name" "ferret" ]
                                                         None
                                                 equal expected actual
                                             testCase "simple case crossOrigin" <| fun _ ->
                                                 let expected =
                                                     "https://example.com:8042/over/there?name=ferret#nose"
                                                 let actual =
                                                     custom
                                                         (CrossOrigin
                                                              "https://example.com:8042")
                                                         [ "over"; "there" ]
                                                         [ str "name" "ferret" ]
                                                         (Some "nose")
                                                 equal expected actual ] ]
