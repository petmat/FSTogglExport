module Fetch

open FSharp.Data

let fetch baseUrl resource query headers = 
    async {
        return! Http.AsyncRequestString
            (baseUrl + resource,
                query = query,
                headers = headers)
    }

let getFetchToggl auth = 
    let authFetchToggl headers resource query = async {
        let headersWithAuth = [ "Authorization", auth ] |> List.append headers
        return! fetch "https://www.toggl.com/api/v8/" resource query headersWithAuth
    }
    authFetchToggl