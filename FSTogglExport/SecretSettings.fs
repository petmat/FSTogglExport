module SecretSettings

open FSharp.Data
open JsonExtensions

let getApiAuth =
    let secrets = JsonValue.Load("../secrets.json")
    secrets?apiToken.AsString()