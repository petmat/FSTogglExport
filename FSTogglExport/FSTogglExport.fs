module FSTogglExport

open SecretSettings
open Fetch
open FSharp.Data
open JsonExtensions

let apiAuth = getApiAuth

let fetchToggl: string -> (string * string) list -> seq<string * string> -> string = 
    getFetchToggl apiAuth

let dateTimeToIso (value:System.DateTime) =
    value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")

let fetchTimeEntries startDate endDate = 
    let query = [
        "start_date", dateTimeToIso startDate; 
        "end_date", dateTimeToIso endDate
    ]

    let json = fetchToggl "time_entries" query []

    JsonValue.Parse(json).AsArray()
    |> Array.filter (fun entry -> (entry?duration.AsInteger()) >= 0)

let convertTimesToFleStandard timeEntries =
    timeEntries

let run =
    System.Console.WriteLine("Fetching entries...")
    let timeEntries = fetchTimeEntries (System.DateTime(2017, 11, 1)) (System.DateTime(2017, 11, 30))

    System.Console.WriteLine(timeEntries)

[<EntryPoint>]
let main argv =
    run
    System.Console.ReadKey() |> ignore
    0