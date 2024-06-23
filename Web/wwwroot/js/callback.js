/// <script src="https://cdnjs.cloudflare.com/ajax/libs/oidc-client-ts/2.0.2/browser/oidc-client-ts.js"></script>

oidc.Log.setLogger(console)

const settings = {
    authority: "https://localhost:6001",
    client_id: "webclient",
    redirect_uri: window.location.origin,
    loadUserInfo: true,
    response_mode: "query",
    response_type: "code",
    userStore: new oidc.WebStorageStateStore({ store: window.sessionStorage }),
}
const mgr = new oidc.UserManager(settings)

async function callbackFromLogin() {
    console.log("DigiLeanOIDC: CallbackLogin")
    try {
        const user = await mgr.signinRedirectCallback()
        console.log("callback-oidc: signed in user", user.profile.sub)
        console.log("callback-oidc: user.state: ", user.state)
        if (user.state)
            window.location.replace(user.state) //state is url the user wants to get back to after login
        else
            window.location.replace("/")
    }
    catch(err) {
        console.error(err)
        const message = err.message
        const main = document.querySelector("main")
        const errorMessage = main?.querySelector("section#error-message")
        const errorHtml = `<div class="error">
            <h2>Error occured while logging in</h2>
            <p class="msg"><i>${message}</i></p>
            <p><a href="${window.location.origin}">Try to log in again</a></p>
            </div>`
        if (errorMessage)
            errorMessage.innerHTML = errorHtml
        else {
            document.body.innerHTML = errorHtml
        }
    }
}
callbackFromLogin()