open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Web
open Suave.Successful

[<EntryPoint>]
let main argv = 
    let personWebPart = rest "people" {
        GetAll = Db.getPeople
    }

    startWebServer defaultConfig personWebPart
    0
