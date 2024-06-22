import { getClientToken } from "./authentication.js"
import * as httpClient from "./httpService.js"
import oidcLogin from "./oidcLogin.js"

let inputUser = document.querySelector("#client")
let inputPass = document.querySelector("#secret")
let btnLoginClient = document.querySelector("#loginClient")
let pToken = document.querySelector("#token")
let taData = document.querySelector("#data")
let btnCallApi = document.querySelector("#callapi")
let btnLoginUser = document.querySelector("#loginUser")
let userData = document.querySelector("#user")


let idServer = "https://localhost:6001"

btnLoginClient.addEventListener("click", async () => {
    let client = inputUser.value
    let secret = inputPass.value

    try {
        let tokenRes = await getClientToken(idServer, client, secret)
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


async function setupOidc() {
    await oidcLogin.initialize()
    const user = await oidcLogin.getUser()
    if (!user) {
        btnLoginUser.addEventListener("click", () => {
            console.log("login")
            oidcLogin.login()
        })
        return
    }

    const userString = JSON.stringify(user, null, 2)
    userData.value = userString
    btnLoginUser.setAttribute("disabled")
}


