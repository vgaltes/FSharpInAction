module PlaceOrderTests

open NUnit.Framework
open CafeAppTestsDSL
open Domain
open System
open States
open Commands
open Events
open Errors
open TestData

[<Test>]
let ``Can place only drinks order`` () =
    let order = {emptyOrder with Drinks = [coke]}
    Given (OpenedTab tab)
    |> When (PlaceOrder order)
    |> ThenStateShouldBe (PlacedOrder order)
    |> WithEvents [OrderPlaced order]

[<Test>]
let ``Can not place an empty order`` () =
    Given (OpenedTab tab)
    |> When (PlaceOrder emptyOrder)
    |> ShouldFailWith CanNotPlaceEmptyOrder

[<Test>]
let ``Can not place order on a closed tab`` () = 
    let order = {emptyOrder with Drinks = [coke]}
    Given (ClosedTab (Some tab.Id))
    |> When (PlaceOrder order)
    |> ShouldFailWith CanNotOrderWithClosedTab

[<Test>]
let ``Can not place order multiple times`` () =
    let order = {emptyOrder with Drinks = [coke]}
    Given (PlacedOrder order)
    |> When (PlaceOrder order)
    |> ShouldFailWith OrderAlreadyPlaced