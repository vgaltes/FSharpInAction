module States

open Domain
open System

type State =
    | ClosedTab of Guid option
    | OpenedTab of Tab
    | PlaceOrder of Order
    | OrderInProgress of InProgressOrder
    | ServedOrder of Order

let apply state event =
    match state, event with
    | _ -> ClosedTab None