module FSTogglExport

open SecretSettings
open Fetch
open FSharp.Data
open JsonExtensions
open System
open System.IO
open System.Text.RegularExpressions

type TimeEntry = { description: string; wid: int; pid: int; start: DateTime; stop: DateTime; duration: int }
type Project = { id: int; wid: int; name: string }
type OutputEntry = { date: DateTime; code: string; duration: double; description: string; identifier: string }

let dateTimeToIso (value: DateTime) =
    value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")

let toDateRangeQuery startDate endDate = [
    "start_date", dateTimeToIso startDate; 
    "end_date", dateTimeToIso endDate
]

let parseList json = JsonValue.Parse(json).AsArray() |> List.ofArray

let toTimeEntries values =
    values 
    |> List.map (fun entry ->
        let start = entry?start.AsDateTime() 
        {
            description = (entry?description.AsString()); 
            wid = (entry?wid.AsInteger());
            pid = (entry?pid.AsInteger());
            start = start;
            stop = (entry?stop.AsDateTime());
            duration = (entry?duration.AsInteger()) 
        }
    )

let getTimeEntries fetchToggl start stop =
    let fetchTimeEntries start stop = 
        ("time_entries", toDateRangeQuery start stop)
        ||> fetchToggl

    fetchTimeEntries start stop
    |> Async.RunSynchronously
    |> parseList
    |> List.filter (fun entry -> (entry?duration.AsInteger()) >= 0)
    |> toTimeEntries

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

let splitToIdentifierAndDescription (description:string) = 
    if Seq.isEmpty description then
        "", description
    else
        let parts = description.Split([|' '|], 2)
        if Regex.IsMatch(parts.[0], @"([A-Z]+-\d+|\d+)") && Array.length parts > 1 
            then parts.[0], parts.[1]
            else "", description

let mapToOutputEntries (timeEntries: TimeEntry list) projects =
    let getProjectName pid = projects |> List.find (fun p -> p.id = pid) |> fun p -> p.name
    let roundToHalfHours durInSecs = max (round (durInSecs / 3600.0 * 2.0) / 2.0) 0.5

    timeEntries
    |> List.groupBy (fun e -> e.start.Date, e.description, e.pid)
    |> List.sortBy (fun ((date, _, _), entries) -> date, entries |> List.map (fun e -> e.stop) |> List.max)
    |> List.map (fun ((date, description, pid), entries) -> 
        let identifier, description = splitToIdentifierAndDescription description
        {
            date = date;
            code = getProjectName pid;
            description = description;
            duration = entries |> List.sumBy (fun e -> e.duration) |> float |> roundToHalfHours;
            identifier = identifier;
        }
    )

let run = async {
    let apiAuth = getApiAuth
    let fetchToggl = (getFetchToggl apiAuth) []
    
    let now = DateTime.Now
    let start = DateTime(now.Year, now.Month, 1)
    let stop = DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month))
    
    printfn "Fetching data..."
    let timeEntries = getTimeEntries fetchToggl start stop
    let workspaceIds = timeEntries |> List.map (fun e -> e.wid) |> List.distinct
    let projects = getProjects fetchToggl workspaceIds
    let outputEntries = mapToOutputEntries timeEntries projects
    
    let entryToString entry =
        sprintf "%s\t%s\t%s\t%s\t\t%s" 
            (entry.date.ToString("dd.MM.yyyy"))
            entry.code
            (entry.duration.ToString("f1"))
            entry.description
            entry.identifier

    let lines = outputEntries |> List.map entryToString
    printfn "Writing to file..."
    File.WriteAllLines("output.txt", lines);
    printfn "All done!"
}

[<EntryPoint>]
let main argv =
    run |> Async.RunSynchronously
    System.Console.ReadKey() |> ignore
    0