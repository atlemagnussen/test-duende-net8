import { getTokenRes } from "/js/authentication.js"

const baseUrl = window.location.origin

const jsonContentType = "application/json"

export async function get(url) {
    const req = createRequest(url, "get", jsonContentType)
    return await httpRequest(req)
}
export async function post(url, data, contentType) {
    const req = createRequest(url, "post", contentType ?? jsonContentType, data)
    return await httpRequest(req)
}

export async function put(url, data) {
    const req = createRequest(url, "put", jsonContentType, data)
    return await httpRequest(req)
}

function createRequest(url, method, contentType, data) {
    const args = {
        method,
        headers: {}
    }
    if (contentType) {
        if (args.headers)
            args.headers["Content-Type"] = contentType
    }
    const tokenRes = getTokenRes()
    if (tokenRes?.access_token) {
        if (args.headers)
            args.headers["Authorization"] = "Bearer " + tokenRes.access_token
    }
    
    if (data) {
        if (contentType === jsonContentType)
            args.body = JSON.stringify(data)
        else
            args.body = data
    }
    
    const fullUrl = url.startsWith("http") ? url : `${baseUrl}/${url}`
    return new Request(fullUrl, args)
}

async function httpRequest(request) {
    let errorFetchMsg
    const res = await fetch(request)
    .catch((error) => {
        errorFetchMsg = "Error fetching"
        console.error(error.message)
        throw new Error(errorFetchMsg)
    })
    return resHandler(res)
}

async function resHandler(res) {
    let errorFetchMsg
    if (res.ok) {
        const contentType = res.headers.get("content-type")
        if (res.status === 200 || res.status === 201) {
            
            if (contentType && contentType.includes("application/json")) {
                const json = await res.json()
                return json
            }
            const text = await res.text()
            return text
        }
        else {
            return ""
        }
    } else {
        console.error(`${res.statusText} (${res.status})`)
        
        errorFetchMsg = "Error fetching"
        if (res.status == 401) {
            errorFetchMsg = "Status 401, Unauthorized"
            throw new Error(errorFetchMsg)
        }
        
        if (res.status >= 400 && res.status < 500) {
            try {
                const pd = await res.json()
                console.log(pd)
                const errors = handleErrorsAspNet(pd)
                errorFetchMsg = errors.join(",")
            }
            catch (ex) {
                console.debug(ex);
            }
            
        } else {
            const message = await res.text()
            console.log(message)
        }
        
        throw new Error(errorFetchMsg)
    }
}

function handleErrorsAspNet(res) {
    const allErrorMsgs = []
    if (res.errors) {
        for (let key in res.errors) {
            let errors = res.errors[key]
            if (errors && Array.isArray(errors))
                allErrorMsgs.push(...errors)
        }
    }
    return allErrorMsgs
}