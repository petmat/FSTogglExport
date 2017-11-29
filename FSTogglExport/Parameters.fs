module Parameters

type Parameters = { isValid: bool; apiKey: string; }

let invalidParams = { isValid = false; apiKey = "" }

let validateArgs (args:string[]) =
    args.Length = 2 && args.[0] = "--api-key"

let parseArgs (args:string[]) =
    { isValid = true; apiKey = args.[1] }

let toParams args =
    match validateArgs args with 
    | true -> parseArgs args
    | false -> invalidParams