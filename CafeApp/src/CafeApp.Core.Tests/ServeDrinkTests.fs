module ServeDrinkTests

open Domain
open TestData
open CafeAppTestsDSL
open Events
open States
open Commands
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