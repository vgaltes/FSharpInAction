namespace SuaveRestApi.Rest

[<AutoOpen>]
module RestFul =
    open Suave
    open Suave.Successful
    open Suave.Operators
    open Suave.Filters
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization


    type RestResource<'a> = {
        GetAll : unit -> 'a seq
    }

    // 'a -> WebPart
    let JSON v =
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <-
            new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings) 
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    // string -> RestResource<'a> -> WebPart
    let rest resourceName resource = 
        let resourcePath = "/" + resourceName
        let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
        path resourcePath >=> GET >=> getAll

