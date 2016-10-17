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

[<Test>]
let ``Can not prepare a non ordered food`` () =
    let order = {emptyOrder with Foods=[pizza]}
    Given (PlacedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith (CanNotPrepareNonOrderedFood salad)

[<Test>]
let ``Can not prepare food for served order`` () =
    Given(ServedOrder emptyOrder)
    |> When (PrepareFood (salad, emptyOrder.Tab.Id))
    |> ShouldFailWith (OrderAlreadyServed)

[<Test>]
let ``Can not prepare with closed tab`` () =
    Given(ClosedTab None)
    |> When (PrepareFood (salad, emptyOrder.Tab.Id))
    |> ShouldFailWith CanNotPrepareWithClosedTab

[<Test>]
let ``Can not prepare a non ordered food during order in progress`` () =
    let order = {emptyOrder with Foods=[salad]}
    let orderInProgress = {
        PlacedOrder = order
        ServedFoods = []
        ServedDrinks = []
        PreparedFoods = []
    }

    Given(OrderInProgress orderInProgress)
    |> When (PrepareFood (pizza, order.Tab.Id))
    |> ShouldFailWith (CanNotPrepareNonOrderedFood pizza)
    
[<Test>]
let ``Can not prepare already prepared food during order in progress`` () =
    let order = {emptyOrder with Foods=[salad; pizza]}
    let orderInProgress = {
        PlacedOrder = order
        ServedFoods = []
        ServedDrinks = []
        PreparedFoods = [salad]
    }

    Given(OrderInProgress orderInProgress)
    |> When (PrepareFood (salad, order.Tab.Id))