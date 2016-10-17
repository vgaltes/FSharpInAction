module CommandHandlers
open Commands
open States
open Events
open Domain
open System
open Chessie.ErrorHandling
open Errors

let handleOpenTab tab = function
| ClosedTab _ -> [TabOpened tab] |> ok
| _ -> TabAlreadyOpened |> fail

let isOrderEmpty order =
    List.isEmpty order.Foods && List.isEmpty order.Drinks

let handlePlaceOrder (order:Order) = function
    | OpenedTab _ -> 
        if isOrderEmpty order then fail CanNotPlaceEmptyOrder
        else
            [OrderPlaced order] |> ok
    | ClosedTab _ -> fail CanNotOrderWithClosedTab
    | PlacedOrder existingOrder -> 
        if existingOrder = order then fail OrderAlreadyPlaced
        else [OrderPlaced order] |> ok
    | _ -> failwith "Todo"

let (|NonOrderedDrink|_|) order drink =
    match List.contains drink order.Drinks with
    | false -> Some drink
    | true -> None

let (|ServeDrinkCompletesOrder|_|) order drink =
    match isServingDrinkCompletesOrder order drink with
    | true -> Some drink
    | false -> None

let (|NonOrderedFood|_|) order food =
    match List.contains food order.Foods with 
    | false -> Some food
    | true -> None

let (|AlreadyPreparedFood|_|) order food =
    match List.contains food order.PreparedFoods with
    | true -> Some Food
    | false -> None

let handleServeDrink drink tabId = function
    | PlacedOrder order ->
        let event = DrinkServed (drink,tabId)
        match drink with
        | NonOrderedDrink order _ -> fail (CanNotServeNonOrderedDrink drink)
        | ServeDrinkCompletesOrder order _ ->
            let payment = {Tab = order.Tab; Amount = orderAmount order}
            event::[OrderServed (order, payment)] |> ok
        | _ -> [event] |> ok
    | OrderInProgress order ->
        [DrinkServed(drink, tabId)] |> ok
    | ServedOrder order ->
        fail OrderAlreadyServed
    | OpenedTab _ -> fail CanNotServeForNonPlacedOrder
    | ClosedTab _ -> fail CanNotServeWithClosedTab

let handlePrepareFood food tabId = function
    | PlacedOrder order ->
        match food with
        | NonOrderedFood order _ -> fail (CanNotPrepareNonOrderedFood food)
        | _ -> [FoodPrepared(food, tabId)] |> ok
    | OrderInProgress ipo ->
        let order = ipo.PlacedOrder
        match food with
        | NonOrderedFood order _ -> fail (CanNotPrepareNonOrderedFood food)
        | AlreadyPreparedFood ipo _ -> fail (CanNotPrepareAlreadyPreparedFood food)
        | _ -> failwith "Todo"
    | ServedOrder order ->
        fail OrderAlreadyServed
    | ClosedTab _ -> fail CanNotPrepareWithClosedTab
    | _ -> failwith "Todo"

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | PlaceOrder order -> handlePlaceOrder order state
    | ServeDrink (drink, tabId) -> handleServeDrink drink tabId state
    | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
    | _ -> failwith "Todo"

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = List.fold States.apply state events 
        (newState, events) |> ok
    | Bad err -> Bad err
