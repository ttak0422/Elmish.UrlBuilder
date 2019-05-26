open Fake.Core
open System.Text.RegularExpressions
open System

#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"
#if !FAKE
#r "./packages/netcorebuild/NETStandard.Library.NETFramework/build/net461/lib/netstandard.dll"
#endif


open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.JavaScript
open System
open System.Text
open System.IO

let srcFiles = !!"./src/Fable.Elmish.UrlBuilder.fsproj"
let fableTestsGlob = "tests/fable/**/*.fsproj"

module Util =
    let visitFile (visitor : string -> string) (fileName : string) =
        File.ReadAllLines(fileName)
        |> Array.map (visitor)
        |> fun lines -> File.WriteAllLines(fileName, lines)

    let replaceLines (replacer : string -> Match -> string option) (reg : Regex)
        (fileName : string) =
        fileName
        |> visitFile (fun line ->
               let m = reg.Match(line)
               if not m.Success then line
               else
                   match replacer line m with
                   | None -> line
                   | Some newLine -> newLine)

module Logger =
    let consoleColor (fc : ConsoleColor) =
        let current = Console.ForegroundColor
        Console.ForegroundColor <- fc
        { new IDisposable with
              member x.Dispose() = Console.ForegroundColor <- current }

    let warn str =
        Printf.kprintf (fun s ->
            use c = consoleColor ConsoleColor.DarkYellow in printf "%s" s) str

    let warnfn str =
        Printf.kprintf (fun s ->
            use c = consoleColor ConsoleColor.DarkYellow in printfn "%s" s) str

    let error str =
        Printf.kprintf (fun s ->
            use c = consoleColor ConsoleColor.Red in printf "%s" s) str

    let errorfn str =
        Printf.kprintf (fun s ->
            use c = consoleColor ConsoleColor.Red in printfn "%s" s) str

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

let needsPublishing (versionRegex : Regex)
    (releaseNotes : ReleaseNotes.ReleaseNotes) (projFile : string) =
    printfn "Project: %s" projFile
    if releaseNotes.NugetVersion.ToUpper().EndsWith("NEXT") then
        Logger.warnfn
            "Version in Release Notes ends with NEXT, don't publish yet."
        false
    else
        File.ReadLines(projFile)
        |> Seq.tryPick (fun line ->
               let m = versionRegex.Match(line)
               if m.Success then Some m
               else None)
        |> function
        | None -> failwith "Couldn't find version in project file"
        | Some m ->
            let sameVersion = m.Groups.[1].Value = releaseNotes.NugetVersion
            if sameVersion then
                Logger.warnfn "Already version %s, no need to publish."
                    releaseNotes.NugetVersion
            not sameVersion

let pushNuget (releaseNotes : ReleaseNotes.ReleaseNotes) (projFile : string) =
    let versionRegex =
        Regex("<Version>(.*?)</Version>", RegexOptions.IgnoreCase)
    if needsPublishing versionRegex releaseNotes projFile then
        let projDir = Path.GetDirectoryName(projFile)

        let nugetKey =
            match Environment.environVarOrNone "NUGET_KEY" with
            | Some nugetKey -> nugetKey
            | None ->
                failwith
                    "The Nuget API key must be set in a NUGET_KEY environmental variable"
        (versionRegex, projFile)
        ||> Util.replaceLines
                (fun line _ ->
                versionRegex.Replace
                    (line,
                     "<Version>" + releaseNotes.NugetVersion + "</Version>")
                |> Some)
        DotNet.pack (fun p ->
            { p with Configuration = DotNet.Release
                     Common = { p.Common with DotNetCliPath = "dotnet" }}) projFile
        let files =
            Directory.GetFiles(projDir </> "bin" </> "Release", "*.nupkg")
            |> Array.find
                   (fun nupkg -> nupkg.Contains(releaseNotes.NugetVersion))
            |> fun x -> [ x ]
        Paket.pushFiles (fun o ->
            { o with ApiKey = nugetKey
                     PublishUrl = "https://www.nuget.org/api/v2/package" })
            files

Target.create "Publish" (fun _ ->
    srcFiles
    |> Seq.iter (fun s ->
           let projFile = s
           let projDir = IO.Path.GetDirectoryName(projFile)
           let release = projDir </> "RELEASE_NOTES.md" |> ReleaseNotes.load
           pushNuget release projFile))
Target.create "Test" (fun _ -> printfn "Test")
"Clean" ==> "YarnInstall" ==> "DotnetRestore" ==> "MochaTest" ==> "Publish"
Target.runOrDefault "Publish"
