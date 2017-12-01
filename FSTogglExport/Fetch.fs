module Fetch

open FSharp.Data

let fetch baseUrl resource query headers = 
    Http.RequestString
        (baseUrl + resource,
            query = query,
            headers = headers)

let getFetchToggl auth = 
    let authFetchToggl resource query (headers: seq<string * string>) = 
        let concatHeaders = Seq.append [ "Authorization", auth ] headers
        fetch "https://www.toggl.com/api/v8/" resource query concatHeaders
    authFetchToggl