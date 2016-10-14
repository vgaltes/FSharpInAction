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

let handleServeDrink drink tabId = function
    | PlacedOrder order ->
        if List.contains drink order.Drinks then
            [DrinkServed (drink,tabId)] |> ok
        else fail (CanNotServeNonOrderedDrink drink)
    | _ -> failwith "Todo"

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | PlaceOrder order -> handlePlaceOrder order state
    | ServeDrink (drink, tabId) -> handleServeDrink drink tabId state
    | _ -> failwith "Todo"

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = List.fold States.apply state events 
        (newState, events) |> ok
    | Bad err -> Bad err
