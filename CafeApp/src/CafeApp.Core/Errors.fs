module Errors
open Domain

type Error =
    | TabAlreadyOpened
    | CanNotPlaceEmptyOrder
    | CanNotOrderWithClosedTab
    | OrderAlreadyPlaced
    | CanNotServeNonOrderedDrink of Drink
    | OrderAlreadyServed