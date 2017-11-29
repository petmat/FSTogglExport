module FSTogglExport

open FSharp.Data
open SecretSettings
open Fetch

let fetchTimeEntries =
    

let convertTimesToFleStandard timeEntries =
    timeEntries

let run =
    System.Console.WriteLine("Fetching entries...")
    let timeEntries = fetchTimeEntries |> convertTimesToFleStandard
    System.Console.Write(timeEntries)

[<EntryPoint>]
let main argv =
    run
    System.Console.ReadKey() |> ignore
    0