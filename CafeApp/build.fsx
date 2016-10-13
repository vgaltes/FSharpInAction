#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing

let buildDir = "./build"
let testDir = "./tests"
let nunitRunnerPath = "packages/NUnit.ConsoleRunner/tools/nunit3-console.exe"
Target "Clean" (fun _ -> CleanDir buildDir)
Target "BuildApp" (fun _ ->
    !! "src/**/*.fsproj"
    -- "src/**/*.Tests.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)
Target "BuildTests" (fun _ ->
    !! "src/**/*.Tests.fsproj"
    |> MSBuildRelease testDir "Build"
    |> Log "BuildTests-Output: "
)
Target "RunUnitTests" (fun _ ->
    !! (testDir + "/*.Tests.dll")
    |> NUnit3 (fun p ->
                {p with ToolPath = nunitRunnerPath})
)

"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"

RunTargetOrDefault "RunUnitTests"