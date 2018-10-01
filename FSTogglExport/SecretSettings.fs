module SecretSettings

open FSharp.Data
open JsonExtensions

let loadSecrets =
    JsonValue.Load("../../../../secrets.json")

let getApiAuth =
    let secrets = loadSecrets
    secrets?apiAuth.AsString()

let getBlobConnectionString =
    let secrets = loadSecrets
    secrets?blobConnectionString.AsString()