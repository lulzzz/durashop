import { RouteComponentProps } from 'react-router-dom';
import * as React from 'react';
import * as Msal from 'msal';

var applicationConfig = {
    clientID: 'letauppdetsjälv',
    authority: "https://login.microsoftonline.com/te/durashopdev.onmicrosoft.com/b2c_1_signupin",
    b2cScopes: ["https://durashopdev.onmicrosoft.com/demoapi/demo.read"],
    webApi: 'https://fabrikamb2chello.azurewebsites.net/hello',
};

interface IState {
    isLoggedIn: boolean, message: string
}

export default class B2CLogin extends React.Component<{}, IState> {

    clientApplication = new Msal.UserAgentApplication(applicationConfig.clientID, applicationConfig.authority, (errorDesc, token, error, tokenType) => {
        // Called after loginRedirect or acquireTokenPopup

        //console.log(conf.apiRoot);
        //SuperAgent.get(process.env.REACT_APP_DEV_API_URL)

        if (tokenType == "id_token") {
            var userName = this.clientApplication.getUser().name;
            this.setState({ isLoggedIn: true });
            this.logMessage("User '" + userName + "' logged-in");
        } else {
            this.logMessage("Error during login:\n" + error);
        }
    });

    state = { isLoggedIn: false, message: "" }

    logMessage(message: string) { this.setState({ message: this.state.message + "\n" + message }); }

    loginRedirect() {
        this.clientApplication.loginRedirect(applicationConfig.b2cScopes);
    }

    logout() { this.clientApplication.logout(); }

    loginPopup() {
        this.clientApplication.loginPopup(applicationConfig.b2cScopes).then((idToken) => {
            this.clientApplication.acquireTokenSilent(applicationConfig.b2cScopes).then((accessToken) => {
                var userName = this.clientApplication.getUser().name;
                this.setState({ isLoggedIn: true });
                this.logMessage("User '" + userName + "' logged-in");
            }, (error) => {
                this.clientApplication.acquireTokenPopup(applicationConfig.b2cScopes).then((accessToken) => {
                    var userName = this.clientApplication.getUser().name;
                    this.setState({ isLoggedIn: true });
                    this.logMessage("User '" + userName + "' logged-in");
                }, (error) => {
                    this.logMessage("Error acquiring the popup:\n" + error);
                });
            })
        }, (error) => {
            this.logMessage("Error during login:\n" + error);
        });
    }

    callApi() {
        this.clientApplication.acquireTokenSilent(applicationConfig.b2cScopes).then((accessToken) => {
            this.callApiWithAccessToken(accessToken);
        }, (error) => {
            this.clientApplication.acquireTokenPopup(applicationConfig.b2cScopes).then((accessToken) => {
                this.callApiWithAccessToken(accessToken);
            }, (error) => {
                this.logMessage("Error acquiring the access token to call the Web api:\n" + error);
            });
        })
    }

    callApiWithAccessToken(accessToken: string) {
        // Call the Web API with the AccessToken
        fetch(applicationConfig.webApi, {
            method: "GET",
            headers: { 'Authorization': 'Bearer ' + accessToken }
        }).then(response => {
            response.text().then(text => this.logMessage("Web APi returned:\n" + JSON.stringify(text)));
        }).catch(result => {
            this.logMessage("Error calling the Web api:\n" + result);
        });
    }

    render() {
        return (
            <div style={{ width: '900px', margin: 'auto' }}>
                <h1>DuraShop B2C Auth</h1>
                <button onClick={() => this.loginPopup()} disabled={this.state.isLoggedIn}>Login Popup</button>
                <button onClick={() => this.loginRedirect()} disabled={this.state.isLoggedIn}>Login Redirect</button>
                <button onClick={() => this.logout()} disabled={!this.state.isLoggedIn}>Logout</button>
                <button onClick={() => this.callApi()} disabled={!this.state.isLoggedIn}>Call Web API</button>
                <pre>{this.state.message}</pre>
            </div>
        );
    }
}