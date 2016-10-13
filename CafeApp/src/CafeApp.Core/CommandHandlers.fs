module CommandHandlers
open Commands
open States
open Events
open Domain
open System

let execute state command =
    match command with
    | OpenTab tab -> [TabOpened tab]
    | _ -> failwith "Todo"

let evolve state command =
    let events = execute state command
    let newState = List.fold apply state events
    (newState, events)