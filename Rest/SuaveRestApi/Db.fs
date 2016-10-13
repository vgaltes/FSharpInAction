namespace SuaveRestApi.Db

open System.Collections.Generic

type Person = {
    Id: int
    Name: string
    Age: int
    Email: string
}

module Db =
    let private peopleStorage = new Dictionary<int, Person>()
    let getPeople () =
        peopleStorage.Values :> seq<Person>

    let createPerson person =
        let id = peopleStorage.Values.Count + 1
        let newPerson = {person with Id = id}
        peopleStorage.Add(id, newPerson)
        newPerson
