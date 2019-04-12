#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.JavaScript

let srcFiles = !!"./src/Elmish.UrlBuilder.fsproj"
let fableTestsGlob = "tests/fable/**/*.fsproj"

Target.create "Clean"
<| fun _ ->
    !!"src/**/bin" ++ "src/**/obj" ++ "tests/**/bin" ++ "tests/**/obj"
    |> Shell.cleanDirs
Target.create "YarnInstall" <| fun _ -> Yarn.install id
Target.create "DotnetRestore"
<| fun _ ->
    srcFiles ++ fableTestsGlob |> Seq.iter (fun proj -> DotNet.restore id proj)
Target.create "MochaTest" <| fun _ ->
    !!fableTestsGlob
    |> Seq.iter (fun proj ->
           let projDir = Path.getDirectory proj
           let configFile = projDir </> "splitter.config.js"
           Yarn.exec ("fable-splitter -c " + configFile) id
           let projDirOutput = projDir </> "bin/tests"
           Yarn.exec ("mocha " + projDirOutput) id)
Target.create "All" ignore
"Clean" ==> "YarnInstall" ==> "DotnetRestore" ==> "MochaTest" ==> "All"
Target.runOrDefault "All"
