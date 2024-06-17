import { getClientToken } from "./authentication.js"
import * as httpClient from "./httpService.js"

let inputUser = document.querySelector("#client")
let inputPass = document.querySelector("#secret")
let btnLogin = document.querySelector("#login")
let pToken = document.querySelector("#token")
let taData = document.querySelector("#data")
let btnCallApi = document.querySelector("#callapi")

let idServer = "https://localhost:6001"

btnLogin.addEventListener("click", async () => {
    let user = inputUser.value
    let password = inputPass.value

    try {
        let tokenRes = await getClientToken(idServer, user, password)
        pToken.innerHTML = tokenRes.access_token
    }
    catch (err) {
        pToken.value = err.message
    }
})

btnCallApi.addEventListener("click", async () => {

    try {
        let res = await httpClient.get("api/values")
        taData.value = JSON.stringify(res, null, 2)
    }
    catch (err) {
        pToken.value = err.message
    }
})