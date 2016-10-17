module ServeFoodTests

open Domain
open States
open Commands
open Events
open CafeAppTestsDSL
open TestData
open Errors
open NUnit.Framework

[<Test>]
let ``Can maintain the order in progress state by serving food`` () =
    let order = {emptyOrder with Foods=[salad; pizza]}
    let orderInProgress = {
        PlacedOrder = order
        ServedFoods = []
        ServedDrinks = []
        PreparedFoods = [salad; pizza]
    }

    let expected = {orderInProgress with ServedFoods = [salad]}
    Given(OrderInProgress orderInProgress)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ThenStateShouldBe(OrderInProgress expected)
    |> WithEvents([FoodServed (salad, order.Tab.Id)])