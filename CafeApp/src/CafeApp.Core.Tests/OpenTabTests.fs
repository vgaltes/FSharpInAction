module OpenTabTests

open NUnit.Framework
open CafeAppTestsDSL
open Domain
open Events
open Commands
open States
open System

[<Test>]
let ``Can open a new Tab`` () =
    let tab = {Id = Guid.NewGuid(); TableNumber = 1}

    Given(ClosedTab None)
    |> When (OpenTab tab)
    |> ThenStateShouldBe (OpenedTab tab)
    //|> WithEvents [TabOpened tab]