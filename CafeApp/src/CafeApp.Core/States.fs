module States

open Domain
open Events
open System

type State =
    | ClosedTab of Guid option
    | OpenedTab of Tab
    | PlacedOrder of Order
    | OrderInProgress of InProgressOrder
    | ServedOrder of Order

let apply state event =
    match state, event with
    | ClosedTab _, TabOpened tab -> OpenedTab tab
    | OpenedTab _, OrderPlaced order -> PlacedOrder order
    | PlacedOrder order, DrinkServed (drink, _) ->  
        {
            PlacedOrder = order
            ServedDrinks = [drink]
            PreparedFoods = []
            ServedFoods = []
        } |> OrderInProgress
    | PlacedOrder order, FoodPrepared (food, _) ->
        {
            PlacedOrder = order
            ServedDrinks = []
            PreparedFoods = [food]
            ServedFoods = []
        } |> OrderInProgress
    | OrderInProgress ipo, OrderServed (order, _) ->
        ServedOrder order
    | OrderInProgress ipo, DrinkServed (drink, _) ->
        {ipo with ServedDrinks = drink::ipo.ServedDrinks} |> OrderInProgress
    | OrderInProgress ipo, FoodServed (food, _) ->
        {ipo with ServedFoods = food::ipo.ServedFoods} |> OrderInProgress
    | _ -> state