module PrepareFoodTests

open Domain
open States
open Commands
open Events
open CafeAppTestsDSL
open TestData
open Errors
open NUnit.Framework

[<Test>]
let ``Can prepare food`` () =
    let order = {emptyOrder with Foods = [salad]}
    let expected = {
        PlacedOrder = order
        ServedDrinks = []
        PreparedFoods = [salad]
        ServedFoods = []
    }

    Given (PlacedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ThenStateShouldBe(OrderInProgress expected)
    |> WithEvents [FoodPrepared (salad, order.Tab.Id)]