#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing

let buildDir = "./build"
let testDir = "./tests"
let nunitRunnerPath = "packages/NUnit.ConsoleRunner/tools/nunit3-console.exe"

let buildApp () =
    !! "src/**/*.fsproj"
    -- "src/**/*.Tests.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "

let buildTests () =
    !! "src/**/*.Tests.fsproj"
    |> MSBuildRelease testDir "Build"
    |> Log "BuildTests-Output: "

let runTests () =
    !! (testDir + "/*.Tests.dll")
    |> NUnit3 (fun p ->
                {p with ToolPath = nunitRunnerPath})

let clean () =
    CleanDir buildDir

Target "Clean" (fun _ -> clean ())
Target "BuildApp" (fun _ ->
    buildApp ()
)
Target "BuildTests" (fun _ ->
    buildTests ()
)
Target "RunUnitTests" (fun _ ->
    runTests ()
)

Target "Watch" (fun _ ->
    use watcher = !! "src/**/*.*" |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        clean ()
        buildApp ()
        buildTests ()
        runTests ()
    )

    System.Console.ReadLine() |> ignore //Needed to keep FAKE from exiting

    watcher.Dispose() // Use to stop the watch from elsewhere, ie another task.
)

"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"

RunTargetOrDefault "RunUnitTests"