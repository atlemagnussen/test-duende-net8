/// <script src="https://cdnjs.cloudflare.com/ajax/libs/oidc-client-ts/2.0.2/browser/oidc-client-ts.js" integrity="sha512-+Yjhp8AofYSElFHSIbFMv+FHYfq1q4oyG/5z8tFnLGVgk7qLQim/JWdpPDDLvnMJ21AkLab8Rl8Rp8ShybgWlA==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
const rootPath = window.location.origin
let oicdConfig = {
    authority: "https://localhost:6001",
    client_id: "webclient",
    redirect_uri: `${rootPath}/callback.html`,
    response_mode: "query",
    response_type: "code",
    scope:"openid profile api.read",
    loadUserInfo: true,
    post_logout_redirect_uri: rootPath,
    userStore: new oidc.WebStorageStateStore({ store: window.sessionStorage }),
    accessTokenExpiringNotificationTimeInSeconds: 60,
    silentRequestTimeoutInSeconds: 20000,
    automaticSilentRenew: false,
    silent_redirect_uri: `${rootPath}/silent-renew.html`,
    monitorSession: true
}

oidc.Log.setLogger(console)
let manager = new oidc.UserManager(oicdConfig)

function setupEvents() {
    manager.events.addUserLoaded((user) => {
        const loadedMsg = `user ${user.profile.sub} loaded`
        logDebug(loadedMsg)
    })
    manager.events.addUserUnloaded(() => {
        logDebug("user unloaded. Session terminated. user: ")
    })
    manager.events.addUserSignedIn(() => {
        logDebug(`user signed in`)
    })
    manager.events.addUserSignedOut(() => {
        logDebug("user signed OUT, if it was another tab, we need to do it here as well")
    })
    manager.events.addAccessTokenExpiring(() => {
        logDebug("access token soon expiring")
    })
    manager.events.addAccessTokenExpired(() => {
        logDebug("access token expired. Renew hopefully")
    })
    manager.events.addSilentRenewError(error => {
        logDebug(`error silent renew ${error.message}`)
    })
}

async function getUser() {
    const user = await manager.getUser()
    return user
}

async function getLoggedInUser() {
    const user = await manager.getUser()
    if (!user || user.expired) {
        log("DigiLEAN auth:: no user means not logged in")
        return login()
    }
    else {
        return user
    }
}
function login() {
    manager.signinRedirect({ 
        state: window.location.href
    })
}

// Must be called initially
async function initialize() {
    const user = await oidc.manager.getUser()
    if (!user || user.expired) {
        log("Not logged in")
        if (!user)
            log("No user")
        else
            log(`User expired=${user.expired}`)
        return null
    }
    else {
        return user
    }
}
function signOut() {
    logDebug("sign out of digilean")
    manager.signoutRedirect()
}

function logDebug(msg) {
    console.debug(`${logPrefix} ${msg}`)
}


setupEvents()

export default { 
    initialize,
    getLoggedInUser,
    getUser,
    login,
    signOut
}