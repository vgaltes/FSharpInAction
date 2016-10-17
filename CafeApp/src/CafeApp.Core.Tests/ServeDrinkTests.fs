module ServeDrinkTests

open Domain
open TestData
open CafeAppTestsDSL
open Events
open States
open Commands
open Errors
open NUnit.Framework

[<Test>]
let ``Can Server Drink`` () =
    let order = {emptyOrder with Drinks = [coke; lemonade]}
    let expected = {
        PlacedOrder = order
        ServedDrinks = [coke]
        PreparedFoods = []
        ServedFoods = []}

    Given(PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [DrinkServed (coke, order.Tab.Id)]

[<Test>]
let ``Can not serve a non ordered drink`` () =
    let order = {emptyOrder with Drinks = [coke]}
    Given (PlacedOrder order)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ShouldFailWith (CanNotServeNonOrderedDrink lemonade)

[<Test>]
let ``Can not serve drink for already served order`` () =
    Given(ServedOrder emptyOrder)
    |> When (ServeDrink (lemonade, emptyOrder.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Test>]
let ``Can not serve drink for non place order`` () =
    Given(OpenedTab tab)
    |> When (ServeDrink (lemonade, emptyOrder.Tab.Id))
    |> ShouldFailWith CanNotServeForNonPlacedOrder

[<Test>]
let ``Can not serve with closed tab`` () =
    Given(ClosedTab (Some tab.Id))
    |> When (ServeDrink (lemonade, emptyOrder.Tab.Id))
    |> ShouldFailWith CanNotServeWithClosedTab

[<Test>]
let ``Can serve drink for order containing one drink`` () =
    let order = {emptyOrder with Drinks=[coke]}
    let payment = {Tab = order.Tab; Amount = drinkPrice coke}

    Given(PlacedOrder order)
    |> When (ServeDrink(coke, order.Tab.Id))
    |> ThenStateShouldBe (ServedOrder order)
    |> WithEvents [
        DrinkServed (coke, order.Tab.Id)
        OrderServed (order, payment)
    ]

[<Test>]
let ``Remain in order in progress while serving a drink`` () =
    let order = {emptyOrder with Drinks=[coke; lemonade; appleJuice]}
    let orderInProgress = {
        PlacedOrder = order
        ServedDrinks = [coke]
        PreparedFoods = []
        ServedFoods = []
    }
    let expected = {orderInProgress with ServedDrinks = lemonade::orderInProgress.ServedDrinks}

    Given (OrderInProgress orderInProgress)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ThenStateShouldBe(OrderInProgress expected)
    |> WithEvents [DrinkServed (lemonade, order.Tab.Id)]

    