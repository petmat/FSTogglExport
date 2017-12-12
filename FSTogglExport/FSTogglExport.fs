module FSTogglExport

open SecretSettings
open Fetch
open FSharp.Data
open JsonExtensions
open System

type TimeEntry = { description: string; wid: int; pid: int; start: DateTime; stop: DateTime; duration: int }
type Project = { id: int; wid: int; name: string }

let dateTimeToIso (value: DateTime) =
    value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")

let toDateRangeQuery startDate endDate = [
    "start_date", dateTimeToIso startDate; 
    "end_date", dateTimeToIso endDate
]
let fetchTimeEntries fetchToggl startDate endDate = async {
    let query = [
        "start_date", dateTimeToIso startDate; 
        "end_date", dateTimeToIso endDate
    ]

    let! json = fetchToggl "time_entries" query

    return JsonValue.Parse(json).AsArray()
    |> List.ofArray
    |> List.filter (fun entry -> (entry?duration.AsInteger()) >= 0)
}

let parseList json = JsonValue.Parse(json).AsArray() |> List.ofArray

let toTimeEntries values =
    values 
    |> List.map (fun entry -> 
        {
            description = (entry?description.AsString()); 
            wid = (entry?wid.AsInteger());
            pid = (entry?pid.AsInteger());
            start = (entry?start.AsDateTime());
            stop = (entry?stop.AsDateTime());
            duration = (entry?duration.AsInteger()) 
        }
    )

let convertTimesToFleStandard timeEntries =
    let fleZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time")
    let convert time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(time, DateTimeKind.Utc), fleZone)
    timeEntries
    |> List.map (fun entry -> {entry with start = convert entry.start; stop = convert entry.stop})

let getTimeEntries fetchToggl start stop =
    let fetchTimeEntries start stop = 
        ("time_entries", toDateRangeQuery start stop)
        ||> fetchToggl

    fetchTimeEntries start stop
    |> Async.RunSynchronously
    |> parseList
    |> List.filter (fun entry -> (entry?duration.AsInteger()) >= 0)
    |> toTimeEntries
    |> convertTimesToFleStandard

let toProjects values =
    values 
    |> List.map (fun project -> 
        {
            id = (project?id.AsInteger()); 
            wid = (project?wid.AsInteger());
            name = (project?name.AsString());
        }
    )

let getProjects fetchToggl workspaceIds =
    let fetchProjectsForWorkspace workspaceId =
        let url = sprintf "workspaces/%i/projects" workspaceId
        (url, []) ||> fetchToggl
    
    workspaceIds
    |> List.map fetchProjectsForWorkspace
    |> Async.Parallel
    |> Async.RunSynchronously
    |> List.ofArray
    |> List.map parseList
    |> List.collect toProjects

let run = async {
    let apiAuth = getApiAuth
    let fetchToggl = (getFetchToggl apiAuth) []
    
    let now = DateTime.Now
    let start = DateTime(now.Year, now.Month, 1)
    let stop = DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month))
    let timeEntries = getTimeEntries fetchToggl start stop

    let workspaceIds = timeEntries |> List.map (fun e -> e.wid) |> List.distinct
    let projects = getProjects fetchToggl workspaceIds

    printfn "%A" timeEntries
    printfn "%A" projects
}

[<EntryPoint>]
let main argv =
    run |> Async.RunSynchronously
    System.Console.ReadKey() |> ignore
    0