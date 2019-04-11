module Tests.Helper

open Util.Testing
open U

let tests : Test =
    testList "Url.Helper"
        [ testList "indexes test" [ testCase "indexes : sub length is 1" <| fun _ ->
                                        let expected = [ 1; 4; 7; 10 ]
                                        let actual =
                                            Helper.indexes "i" "Mississippi"
                                        equal expected actual
                                    testCase "indexes : sub length is 2" <| fun _ ->
                                        let expected = [ 2; 5 ]
                                        let actual =
                                            Helper.indexes "ss" "Mississippi"
                                        equal expected actual
                                    testCase "indexes : return no value" <| fun _ ->
                                        let expected = []
                                        let actual =
                                            Helper.indexes "needle" "haystack"
                                        equal expected actual ]
          testList "slice test" [ testCase "case 1" <| fun _ ->
                                      let str = "0123456789"
                                      let expected = "3456789"
                                      let actual = Helper.slice 3 str.Length str
                                      equal expected actual
                                  testCase "case 2" <| fun _ ->
                                      let str = "0123456789"
                                      let expected = "89"
                                      let actual =
                                          Helper.slice -2 str.Length str
                                      equal expected actual
                                  testCase "case 3" <| fun _ ->
                                      let str = "0123456789"
                                      let expected = "345678"
                                      let actual = Helper.slice 3 -1 str
                                      equal expected actual
                                  testCase "elm test 1" <| fun _ ->
                                      let str = "snakes on a plane!"
                                      let expected = "on"
                                      let actual = Helper.slice 7 9 str
                                      equal expected actual
                                  testCase "elm test 2" <| fun _ ->
                                      let str = "snakes on a plane!"
                                      let expected = "snakes"
                                      let actual = Helper.slice 0 6 str
                                      equal expected actual
                                  testCase "elm test 3" <| fun _ ->
                                      let str = "snakes on a plane!"
                                      let expected = "snakes on a"
                                      let actual = Helper.slice 0 -7 str
                                      equal expected actual
                                  testCase "elm test 4" <| fun _ ->
                                      let str = "snakes on a plane!"
                                      let expected = "plane"
                                      let actual = Helper.slice -6 -1 str
                                      equal expected actual
                                  testCase "0 0" <| fun _ ->
                                      let str = "0123456789"
                                      let expected = ""
                                      let actual = Helper.slice 0 0 str
                                      equal expected actual
                                  testCase "0 n" <| fun _ ->
                                      let str = "0123456789"
                                      let expected = "0123456789"
                                      let actual = Helper.slice 0 str.Length str
                                      equal expected actual ]
          testList "dropLeft test" [ testCase "drop 2 character" <| fun _ ->
                                         let expected = "e Lone Gunmen"
                                         let actual =
                                             Helper.dropLeft 2 "The Lone Gunmen"
                                         equal expected actual
                                     testCase "n = 0" <| fun _ ->
                                         let expected = "The Lone Gunmen"
                                         let actual =
                                             Helper.dropLeft 0 "The Lone Gunmen"
                                         equal expected actual
                                     testCase "n < 0" <| fun _ ->
                                         let expected = "The Lone Gunmen"
                                         let actual =
                                             Helper.dropLeft -1
                                                 "The Lone Gunmen"
                                         equal expected actual
                                     testCase "drop http" <| fun _ ->
                                         let url = "http://foo.com"
                                         let expected = "foo.com"
                                         let actual = Helper.dropLeft 7 url
                                         equal expected actual
                                     testCase "drop https" <| fun _ ->
                                         let url = "https://foo.com"
                                         let expected = "foo.com"
                                         let actual = Helper.dropLeft 8 url
                                         equal expected actual ]
          testList "left test" [ testCase "left 2 character" <| fun _ ->
                                     let expected = "Mu"
                                     let actual = Helper.left 2 "Mulder"
                                     equal expected actual
                                 testCase "n = 0" <| fun _ ->
                                     let expected = ""
                                     let actual = Helper.left 0 "Mulder"
                                     equal expected actual
                                 testCase "n < 0" <| fun _ ->
                                     let expected = ""
                                     let actual = Helper.left -1 "Mulder"
                                     equal expected actual ]
          testList "toInt test" [ testCase "\"1\" to Some 1" <| fun _ ->
                                      let expected = Some 1
                                      let actual = Helper.toInt "1"
                                      equal expected actual
                                  testCase "invalid cast return None" <| fun _ ->
                                      let expected = None
                                      let actual = Helper.toInt "a"
                                      equal expected actual ] ]
