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

let log twoTrackInput =
    let success(x, msgs) = printfn "Correct state"
    let failure msgs = printf "ERROR. %A" msgs
    eitherTee

let ThenStateShouldBe expectedState (command, state) =
    match evolve state command with
    | Ok((actualState,events),_) ->
          actualState |> should equal expectedState
          events |> Some
    | Bad errs ->
        sprintf "Expected : %A, But Actual : %A" expectedState errs.Head
        |> Assert.Fail
        None

let WithEvents expectedEvents actualEvents =
  match actualEvents with
  | Some (actualEvents) ->
    actualEvents |> should equal expectedEvents
  | None -> None |> should equal expectedEvents

let ShouldFailWith (expectedError: Error) (command, state) =
    match evolve state command with
    | Ok(r, _) ->
        sprintf "Expected : %A, But Actual : %A" expectedError raise
        |> Assert.Fail
    | Bad errs -> errs.Head |> should equal expectedError
