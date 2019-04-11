# U
[![Build status](https://ci.appveyor.com/api/projects/status/h9yptt1gr1folq1g?svg=true)](https://ci.appveyor.com/project/ttak0422/u)
[![Build Status](https://travis-ci.org/ttak0422/U.svg?branch=master)](https://travis-ci.org/ttak0422/U)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/ff726cda5320446ca1947c1aa439b8cc)](https://www.codacy.com/app/ttak0422/U?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ttak0422/U&amp;utm_campaign=Badge_Grade)

## About

[**elm/url**](https://github.com/elm/url) for [**elmish/elmish**](https://github.com/elmish/elmish)

## Build

```sh
./fake.sh build
```

## Info

### URLs

```fsharp
type Url =
	{ Protocol : Protocol
  	  Host : string
	  Port : Option<int>
	  Path : string
	  Query : Option<string>
	  Fragment : Option<string> }
```

```fsharp
type Protocol =
	| Http
	| Https
```

```fsharp
toString // Url -> string
```

```fsharp
fromString // string -> Option<Url>

// sample
fromString "https://example.com:443/"
(*
	Some { Protocol = Https
	       Host =  "example.com"
	       Port = Some 443
	       Path =  "/"
	       Query = None
	       Fragment = None }
*)

fromString "https://example.com/hats?q=top%20hat"
(*
	Some { Protocol = Https
	       Host = "example.com"
	       Port = None
	       Path = "/hats"
	       Query = Some "q=top%20hat"
	       Fragment = None }
*)

fromString "http://example.com/core/List/#map"
(*
	Some { Protocol = Http
	       Host = "example.com"
	       Port = None
	       Path = "/core/List"
	       Query = None
	       Fragment = Some "map" }
*)

fromString "example.com:443"        = None
fromString "http://tom@example.com" = None
fromString "http://#cats"           = None
```

### Percent-Encoding

```fsharp
percentEncode // string -> string

// sample
percentEncode "hat"   = "hat"
percentEncode "to be" = "to%20be"
percentEncode "99%"   = "99%25"

percentEncode "$" = "%24"
percentEncode "¢" = "%C2%A2"
percentEncode "€" = "%E2%82%AC"
```

```fsharp
percentDecode // string -> Option<string>

// sample
percentDecode "hat"     = Some "hat"
percentDecode "to%20be" = Some "to be"
percentDecode "99%25"   = Some "99%"

percentDecode "%24"       = Some "$"
percentDecode "%C2%A2"    = Some "¢"
percentDecode "%E2%82%AC" = Some "€"

percentDecode "%"   = None
percentDecode "%XY" = None
percentDecode "%C2" = None
```

### Queries

**<font color = "Red">The following function name is different from elm/url</font>**

- **<font color = "Red">str (in elm/url is string)</font>**
- **<font color = "Red">i32 (in elm/url is int)</font>**

```fsharp
type QueryPatameter = QueryPatameter of  keyValue : string * string
```

```fsharp
str // string -> string -> QueryParameter

// sample
absolute [ "products" ] [ str "search" "hat" ]
(*
	"/products?search=hat"
*)

absolute [ "products" ] [ str "search" "coffee table" ]
(*
	"/products?search=coffee%20table"
*)
```

```fsharp
i32 // string -> int -> QueryParameter

// sample
absolute [ "products" ] [ str "search" "hat"; i32 "page" 2 ]
(*
	"/products?search=hat&page=2"
*)
```

```fsharp
toQuery // QueryParameter list -> string

// sample
toQuery [ str "search" "hat" ]
(*
	"?search=hat"
*)

toQuery [ str "search" "coffee table" ]
(*
	"?search=coffee%20table"
*)

toQuery [ str "search" "hat"; int "page" 2 ]
(*
	"?search=hat&page=2"
*)

toQuery []
(*
	""
*)
```

### Builder

```fsharp
type Root =
	| Absolute
	| Relative
	| CrossOrigin of string
```

```fsharp
abusolute // string list -> QueryParameter list -> strnig

// sample
absolute [] []
(*
	"/"
*)

absolute [ "packages"; "elmish"; "elmish" ] []
(*
	"/packages/elmish/elmish
*)

absolute [ "blog"; string 42 ] []
(*
	"/blog/42"
*)

absolute [ "products" ] [ str "search" "hat"; i32 "page" 2 ]
(*
	"/products?search=hat&page=2"
*)
```

```fsharp
relative // string list -> QueryParameter list -> string

// sample
relative [] []
(*
	""
*)

relative [ "elmish"; "elmish" ] []
(*
	"elmish/elmish"
*)

relative [ "blog"; string 42 ] []
(*
	"blog/42"
*)

relative [ "products" ] [ str "search" "hat"; i32 "page" 2 ]
(*
	"products?search=hat&page=2"
*)
```

```fsharp
crossOrigin // string -> string list -> QueryParameter list -> string

// sample
crossOrigin "https://example.com" [ "products" ] []
(*
	"https://example.com/products"
*)

crossOrigin "https://example.com" [] []
(*
	"https://example.com/"
*)

crossOrigin
  "https://example.com:8042"
  [ "over"; "there" ]
  [ str "name" "ferret" ]
(*
	"https://example.com:8042/over/there?name=ferret"
*)
```

```fsharp
custom // Root -> string list -> QueryParameter list -> Option<string> -> string

// sample
custom Absolute
  [ "packages"; "elmish"; "elmish"; "latest"; "String" ]
  []
  (Some "length")
(*
	"/packages/elmish/elmish/latest/String#length"
*)

custom Relative [ "there" ] [ str "name" "ferret" ] None
(*
	"there?name=ferret"
*)

custom
  (CrossOrigin "https://example.com:8042")
  [ "over"; "there" ]
  [ str "name" "ferret" ]
  (Some "nose")
(*
	"https://example.com:8042/over/there?name=ferret#nose"
*)
```
