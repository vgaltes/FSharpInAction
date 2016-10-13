namespace SuaveRestApi.Rest

[<AutoOpen>]
module RestFul =
    open Suave
    open Suave.Successful
    open Suave.Operators
    open Suave.Filters
    open Suave.RequestErrors
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization


    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        Create: 'a -> 'a
        Update: 'a -> 'a option
        Delete: int -> unit
        GetById: int -> 'a option
        UpdateById: int -> 'a -> 'a option
        Exists: int -> bool
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
        let badRequest = BAD_REQUEST "Resource not found"
        let notFound = NOT_FOUND "Resource not found"
        let handleRequest requestError = function
            | Some r -> r |> JSON
            | None -> requestError
        let resourceIdPath =
            let path = resourcePath + "/%d"
            new PrintfFormat<(int -> string), unit, string, string, int>(path)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let getResourceById =
            resource.GetById >> handleRequest notFound

        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleRequest notFound)
            
        let resourceExists id =
            if resource.Exists id then OK "" else notFound

        choose [
            path resourcePath >=> choose[
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> JSON)
                PUT >=> request (getResourceFromReq >> resource.Update >> handleRequest badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath resourceExists
        ]
        

