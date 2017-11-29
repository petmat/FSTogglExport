#r "../packages/FSharp.Data/lib/net45/FSharp.Data.dll"

open FSharp.Data

let dateTimeToIso (value:System.DateTime) =
    value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")
        

let fetch baseUrl resource query headers = 
    Http.RequestString
        (baseUrl + resource,
            query = query,
            headers = headers)

let fetchToggl = fetch "https://www.toggl.com/api/v8/"

let query = [
    "start_date", dateTimeToIso (System.DateTime(2017, 11, 1)); 
    "end_date", dateTimeToIso (System.DateTime(2017, 11, 30))
]


type SecretSettings = JsonProvider<"../secrets.json">

let secrets = SecretSettings.GetSample()

secrets.ApiAuth

let basicAuth = secrets.ApiAuth

fetchToggl "time_entries" query [ "Authorization", basicAuth ] 
