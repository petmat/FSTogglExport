module SecretSettings

open FSharp.Data
open JsonExtensions

let getApiAuth =
    printfn "We're getting the api auth!"
    let secrets = JsonValue.Load("../../../../secrets.json")
    secrets?apiAuth.AsString()