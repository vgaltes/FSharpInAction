module OpenTabTests

open NUnit.Framework
open CafeAppTestsDSL
open Domain
open Events
open Commands
open States
open Errors
open TestData
open System

[<Test>]
let ``Can open a new Tab`` () =
    Given(ClosedTab None)
    |> When (OpenTab tab)
    |> ThenStateShouldBe (OpenedTab tab)
    |> WithEvents [TabOpened tab]

[<Test>]
let ``Cannot open an already opened tab`` () =
    Given (OpenedTab tab)
    |> When (OpenTab tab)
    |> ShouldFailWith TabAlreadyOpened