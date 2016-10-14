module CafeAppTestsDSL
open FsUnit
open CommandHandlers
open Chessie.ErrorHandling
open NUnit.Framework
open Errors
open States
open Events

let Given (state : State) = state

let When command state = (command, state)

let ThenStateShouldBe (expectedState:State) (command, state) : (Option<Event list>) =
    let a = evolve state command
    match a with
    // Original pattern match.
    //| Ok _ ->
          //actualState |> should equal expectedState
          //events |> Some
    //| Bad errs ->
    //    sprintf "Expected : %A, But Actual : %A" expectedState errs.Head
    //    |> Assert.Fail
    //    None
    | Ok(_) -> None
    | _ -> None

let WithEvents expectedEvents actualEvents =
  match actualEvents with
  | Some (actualEvents) ->
    actualEvents |> should equal expectedEvents
  | None -> None |> should equal expectedEvents