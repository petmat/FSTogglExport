module FSTogglExport

open FSharp.Data
open Parameters

let fetchTimeEntries apiKey =
    []    

let convertTimesToFleStandard timeEntries =
    []

let proceed (parameters:Parameters) =
    System.Console.WriteLine("Fetching entries...")
    let timeEntries = fetchTimeEntries parameters.apiKey |> convertTimesToFleStandard
    ()

let run args =
    let parameters = toParams args
    match parameters.isValid with
    | true -> proceed parameters
    | false -> System.Console.WriteLine("Example usage: toggl-export --api-key xxxx");

[<EntryPoint>]
let main argv =
    run argv
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code

