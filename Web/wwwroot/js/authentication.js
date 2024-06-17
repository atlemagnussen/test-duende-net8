
import * as httpService from "/js/httpService.js"

const disco = "/.well-known/openid-configuration"

export async function getDiscoveryDocument(authServerUrl) {
    const doc = await httpService.get(authServerUrl + disco)
    return doc
}

let tokenRes
export function getTokenRes() {
    return tokenRes
}


export async function getClientToken(authServerUrl, clientId, clientSecret) {
    const doc = await getDiscoveryDocument(authServerUrl)
    const xFormBody = `client_id=${encodeURI(clientId)}&client_secret=${encodeURI(clientSecret)}&grant_type=client_credentials`
    console.log(xFormBody)
    tokenRes = await httpService.post(doc.token_endpoint, xFormBody, "application/x-www-form-urlencoded")
    return tokenRes
}

