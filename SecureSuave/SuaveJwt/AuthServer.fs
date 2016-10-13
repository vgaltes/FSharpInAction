namespace SuaveJwt.AuthServer

type AudienceCreateRequest = {
    Name: string
}

type AudienceCreateResponse = {
    ClientId: string
    Base64Secret: string
    Name: string
}

type Config = {
    AddAudienceUrlPath: string
    SaveAudience: Audience -> Async<Audience>
}
