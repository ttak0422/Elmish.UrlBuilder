module Tests.Url

open U
open Util.Testing
open U.Url

let tests : Test =
    testList "Url"
        [ testList "toString test" [ testCase "valid case 1" <| fun _ ->
                                         let url =
                                             { Protocol = Https
                                               Host = "example.com"
                                               Port = Some 443
                                               Path = "/"
                                               Query = None
                                               Fragment = None }

                                         let expected =
                                             "https://example.com:443/"
                                         let actual = Url.toString url
                                         equal expected actual
                                     testCase "valid case 2" <| fun _ ->
                                         let url =
                                             { Protocol = Https
                                               Host = "example.com"
                                               Port = None
                                               Path = "/hats"
                                               Query = Some "q=top%20hat"
                                               Fragment = None }

                                         let expected =
                                             "https://example.com/hats?q=top%20hat"
                                         let actual = Url.toString url
                                         equal expected actual
                                     testCase "valid case 3" <| fun _ ->
                                         let url =
                                             { Protocol = Http
                                               Host = "example.com"
                                               Port = None
                                               Path = "/core/List/"
                                               Query = None
                                               Fragment = Some "map" }

                                         let expected =
                                             "http://example.com/core/List/#map"
                                         let actual = Url.toString url
                                         equal expected actual ]
          testList "addPort test" [ testCase "Some port" <| fun _ ->
                                        let starter = "https://foo.com"
                                        let port = Some 443
                                        let expected =
                                            starter + ":"
                                            + (string (Option.get port))
                                        let actual =
                                            Internal.addPort port starter
                                        equal expected actual
                                    testCase "None port" <| fun _ ->
                                        let starter = "https://foo.com"
                                        let port = None
                                        let expected = starter
                                        let actual =
                                            Internal.addPort port starter
                                        equal expected actual ]
          testList "addPrefixed test" [ testCase "None" <| fun _ ->
                                            let starter = "http://foo.com/"
                                            let prefix = ""
                                            let segment = None
                                            let expected = starter + prefix
                                            let actual =
                                                Internal.addPrefixed prefix
                                                    segment starter
                                            equal expected actual
                                        testCase "? Query" <| fun _ ->
                                            let starter = "http://foo.com/"
                                            let prefix = "?"
                                            let segment = Some "isTest=yes"
                                            let expected =
                                                starter + prefix
                                                + (Option.get segment)
                                            let actual =
                                                Internal.addPrefixed prefix
                                                    segment starter
                                            equal expected actual
                                        testCase "# segment" <| fun _ ->
                                            let starter = "http://foo.com/"
                                            let prefix = "#"
                                            let segment = Some "1"
                                            let expected =
                                                starter + prefix
                                                + (Option.get segment)
                                            let actual =
                                                Internal.addPrefixed prefix
                                                    segment starter
                                            equal expected actual ]
          testList "chompBeforePath test" [ testCase "contains ':'" <| fun _ ->
                                                let protocol = Http
                                                let path = "/core/List"
                                                let ``params`` =
                                                    Some "q=top%20hat"
                                                let frag = Some "map"
                                                let str = "example.com:443"

                                                let expected =
                                                    Some { Protocol = protocol
                                                           Host = "example.com"
                                                           Port = Some 443
                                                           Path = path
                                                           Query = ``params``
                                                           Fragment = frag }

                                                let actual =
                                                    Internal.chompBeforePath
                                                        protocol path ``params``
                                                        frag str
                                                equal expected actual
                                            testCase "contains ':' * 2 " <| fun _ ->
                                                let protocol = Http
                                                let path = "/core/List"
                                                let ``params`` =
                                                    Some "q=top%20hat"
                                                let frag = Some "map"
                                                let str = "example.com:443:80"
                                                let expected = None
                                                let actual =
                                                    Internal.chompBeforePath
                                                        protocol path ``params``
                                                        frag str
                                                equal expected actual
                                            testCase "not contains :" <| fun _ ->
                                                let protocol = Http
                                                let path = "/core/List"
                                                let ``params`` =
                                                    Some "q=top%20hat"
                                                let frag = Some "map"
                                                let str = "example.com"

                                                let expected =
                                                    Some { Protocol = protocol
                                                           Host = "example.com"
                                                           Port = None
                                                           Path = path
                                                           Query = ``params``
                                                           Fragment = frag }

                                                let actual =
                                                    Internal.chompBeforePath
                                                        protocol path ``params``
                                                        frag str
                                                equal expected actual
                                            testCase "contains @" <| fun _ ->
                                                let protocol = Http
                                                let path = "/core/List"
                                                let ``params`` =
                                                    Some "q=top%20hat"
                                                let frag = Some "map"
                                                let str = "tom@example.com"
                                                let expected = None
                                                let actual =
                                                    Internal.chompBeforePath
                                                        protocol path ``params``
                                                        frag str
                                                equal expected actual ]
          testList "chompBeforeQuery test" [ testCase "contains '/'" <| fun _ ->
                                                 let protocol = Http
                                                 let ``params`` =
                                                     Some "q=top%20hat"
                                                 let frag = Some "map"
                                                 let str =
                                                     "example.com:443/core/List"

                                                 let expected =
                                                     Some { Protocol = protocol
                                                            Host = "example.com"
                                                            Port = Some 443
                                                            Path = "/core/List"
                                                            Query = ``params``
                                                            Fragment = frag }

                                                 let actual =
                                                     Internal.chompBeforeQuery
                                                         protocol ``params``
                                                         frag str
                                                 equal expected actual
                                             testCase "not contains '/'" <| fun _ ->
                                                 let protocol = Http
                                                 let ``params`` = None
                                                 let frag = None
                                                 let str = "example.com:443"

                                                 let expected =
                                                     Some { Protocol = protocol
                                                            Host = "example.com"
                                                            Port = Some 443
                                                            Path = "/"
                                                            Query = ``params``
                                                            Fragment = frag }

                                                 let actual =
                                                     Internal.chompBeforeQuery
                                                         protocol ``params``
                                                         frag str
                                                 equal expected actual ]
          testList "chompBeforeFragment test" [ testCase "contains '?'" <| fun _ ->
                                                    let protocol = Http
                                                    let frag = Some "map"
                                                    let str =
                                                        "example.com:443/core/List?q=top%20hat"

                                                    let expected =
                                                        Some { Protocol =
                                                                   protocol
                                                               Host =
                                                                   "example.com"
                                                               Port = Some 443
                                                               Path =
                                                                   "/core/List"
                                                               Query =
                                                                   Some
                                                                       "q=top%20hat"
                                                               Fragment = frag }

                                                    let actual =
                                                        Internal.chompBeforeFragment
                                                            protocol frag str
                                                    equal expected actual
                                                testCase "not contains '?'" <| fun _ ->
                                                    let protocol = Http
                                                    let frag = Some "map"
                                                    let str =
                                                        "example.com:443/core/List"

                                                    let expected =
                                                        Some { Protocol =
                                                                   protocol
                                                               Host =
                                                                   "example.com"
                                                               Port = Some 443
                                                               Path =
                                                                   "/core/List"
                                                               Query = None
                                                               Fragment = frag }

                                                    let actual =
                                                        Internal.chompBeforeFragment
                                                            protocol frag str
                                                    equal expected actual ]
          testList "chompAfterProtocol test" [ testCase "contains '#'" <| fun _ ->
                                                   let protocol = Http
                                                   let str =
                                                       "example.com:443/core/List?q=top%20hat#map"

                                                   let expected =
                                                       Some
                                                           { Protocol = protocol
                                                             Host =
                                                                 "example.com"
                                                             Port = Some 443
                                                             Path = "/core/List"
                                                             Query =
                                                                 Some
                                                                     "q=top%20hat"
                                                             Fragment =
                                                                 Some "map" }

                                                   let actual =
                                                       Internal.chompAfterProtocol
                                                           protocol str
                                                   equal expected actual
                                               testCase "not contains '#'" <| fun _ ->
                                                   let protocol = Http
                                                   let str =
                                                       "example.com:443/core/List?q=top%20hat"

                                                   let expected =
                                                       Some { Protocol =
                                                                  protocol
                                                              Host =
                                                                  "example.com"
                                                              Port = Some 443
                                                              Path =
                                                                  "/core/List"
                                                              Query =
                                                                  Some
                                                                      "q=top%20hat"
                                                              Fragment = None }

                                                   let actual =
                                                       Internal.chompAfterProtocol
                                                           protocol str
                                                   equal expected actual ]
          testList "fromString test" [ testCase "valid case 1" <| fun _ ->
                                           let url = "https://example.com:443/"

                                           let expected =
                                               Some { Protocol = Https
                                                      Host = "example.com"
                                                      Port = Some 443
                                                      Path = "/"
                                                      Query = None
                                                      Fragment = None }

                                           let actual = Url.fromString url
                                           equal expected actual
                                       testCase "valid case 1'" <| fun _ ->
                                           let url = "https://example.com:443"

                                           let expected =
                                               Some { Protocol = Https
                                                      Host = "example.com"
                                                      Port = Some 443
                                                      Path = "/"
                                                      Query = None
                                                      Fragment = None }

                                           let actual = Url.fromString url
                                           equal expected actual
                                       testCase "valid case 2" <| fun _ ->
                                           let url =
                                               "https://example.com/hats?q=top%20hat"

                                           let expected =
                                               Some { Protocol = Https
                                                      Host = "example.com"
                                                      Port = None
                                                      Path = "/hats"
                                                      Query = Some "q=top%20hat"
                                                      Fragment = None }

                                           let actual = Url.fromString url
                                           equal expected actual
                                       testCase "valid case 3" <| fun _ ->
                                           let url =
                                               "http://example.com/core/List/#map"

                                           let expected =
                                               Some { Protocol = Http
                                                      Host = "example.com"
                                                      Port = None
                                                      Path = "/core/List/"
                                                      Query = None
                                                      Fragment = Some "map" }

                                           let actual = Url.fromString url
                                           equal expected actual
                                       testCase "invalid case 1 : no protocol" <| fun _ ->
                                           let url = "example.com:443"
                                           let actual = Url.fromString url
                                           let expected = None
                                           equal expected actual
                                       testCase
                                           "invalid case 2 : userinfo disallowed" <| fun _ ->
                                           let url = "http://tom@example.com"
                                           let expected = None
                                           let actual = Url.fromString url
                                           equal expected actual
                                       testCase "invalid case 3 : no host" <| fun _ ->
                                           let url = "http://#cats"
                                           let expected = None
                                           let actual = Url.fromString url
                                           equal expected actual ]
          testList "percentEncode test" [ testCase "ascii : nochange" <| fun _ ->
                                              let str = "hat"
                                              let expected = str
                                              let actual = percentEncode str
                                              equal expected actual
                                          testCase "asciii : space" <| fun _ ->
                                              let str = "to be"
                                              let expected = "to%20be"
                                              let actual = percentEncode str
                                              equal expected actual
                                          testCase "ascii : %" <| fun _ ->
                                              let str = "99%"
                                              let expected = "99%25"
                                              let actual = percentEncode str
                                              equal expected actual
                                          testCase "utf-8 : $" <| fun _ ->
                                              let str = "$"
                                              let expected = "%24"
                                              let actual = percentEncode str
                                              equal expected actual
                                          testCase "utf-8 : ¢" <| fun _ ->
                                              let str = "¢"
                                              let expected = "%C2%A2"
                                              let actual = percentEncode str
                                              equal expected actual
                                          testCase "utf-8 : €" <| fun _ ->
                                              let str = "€"
                                              let expected = "%E2%82%AC"
                                              let actual = percentEncode str
                                              equal expected actual ]
          testList "percentDecode test" [ testCase "ascii : nochange" <| fun _ ->
                                              let str = "hat"
                                              let expected = Some str
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "asciii : space" <| fun _ ->
                                              let str = "to%20be"
                                              let expected = Some "to be"
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "ascii : %" <| fun _ ->
                                              let str = "99%25"
                                              let expected = Some "99%"
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "utf-8 : $" <| fun _ ->
                                              let str = "%24"
                                              let expected = Some "$"
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "utf-8 : ¢" <| fun _ ->
                                              let str = "%C2%A2"
                                              let expected = Some "¢"
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "utf-8 : €" <| fun _ ->
                                              let str = "%E2%82%AC"
                                              let expected = Some "€"
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "invalid decode %" <| fun _ ->
                                              let str = "%"
                                              let expected = None
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "invalid decode %XY" <| fun _ ->
                                              let str = "%XY"
                                              let expected = None
                                              let actual = percentDecode str
                                              equal expected actual
                                          testCase "invalid decode %C2" <| fun _ ->
                                              let str = "%C2"
                                              let expected = None
                                              let actual = percentDecode str
                                              equal expected actual ] ]
