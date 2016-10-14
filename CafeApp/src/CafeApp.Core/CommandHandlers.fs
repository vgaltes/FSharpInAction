module CommandHandlers
open Commands
open States
open Events
open Domain
open System
open Chessie.ErrorHandling
open Errors

let execute state command =
    match command with
    | OpenTab tab -> 
        match state with 
        | ClosedTab _ -> [TabOpened tab] |> ok
        | _ -> TabAlreadyOpened |> fail
    | _ -> failwith "Todo"

let evolve state command =
    match execute state command with
    | Ok (events, _) ->
        let newState = List.fold States.apply state events 
        (newState, events) |> ok
    | Bad err -> Bad err
