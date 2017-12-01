// #load "./SecretSettings.fs"
// #load "./Fetch.fs"

// open SecretSettings
// open Fetch
// let apiAuth = getApiAuth
// let fetchToggl: string -> (string * string) list -> seq<string * string> -> string = 
//     getFetchToggl apiAuth

// let dateTimeToIso (value:System.DateTime) =
//     value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")

// let fetchTimeEntries startDate endDate = 
//     let query = [
//         "start_date", dateTimeToIso startDate; 
//         "end_date", dateTimeToIso endDate
//     ]

//     fetchToggl "time_entries" query

// let convertTimesToFleStandard timeEntries =
//     timeEntries

// let run =
//     System.Console.WriteLine("Fetching entries...")
//     let timeEntries = fetchTimeEntries |> convertTimesToFleStandard
//     System.Console.Write(timeEntries)

// [<EntryPoint>]
// let main argv =
//     run
//     System.Console.ReadKey() |> ignore
//     0