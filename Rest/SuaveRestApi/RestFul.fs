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
        Create: 'a -> 'a
    }

    // 'a -> WebPart
    let JSON v =
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <-
            new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings) 
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req: HttpRequest) =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString(rawForm)

        req.rawForm |> getString |> fromJson<'a>

    // string -> RestResource<'a> -> WebPart
    let rest resourceName resource = 
        let resourcePath = "/" + resourceName
        let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
        path resourcePath >=> choose[
            GET >=> getAll
            POST >=> request (getResourceFromReq >> resource.Create >> JSON)
        ]
        

