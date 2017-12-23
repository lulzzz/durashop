import { addTask } from 'domain-task';
import { AppThunkAction } from './';
import { Action, Reducer } from 'redux';
import OrchestrationResponse from 'ClientApp/commonmodels/OrchestrationResponse';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface UserState {
    userStartResponse: OrchestrationResponse;
    userItems: User[];
    userAuthenticated: boolean;
    userLoading: boolean;
}

export interface User {
    UserId: string;
    FirstName: string;
    LastName: string;
    UserEmail: string;
    UserMobile: string;
    PostalCode: string;
    City: string;
    StreetAddress: string;
    Country: string;
}

export interface GetUserItemResponse {
    runtimeStatus: string;
    input: User[];
    output: any[];
    createdTime: Date;
    lastUpdatedTime: Date;
}

interface UserStartedWasReceived { type: 'USER_STARTED_WAS_RECEIVED', userStartResponse: OrchestrationResponse }
interface UserStartWasSent { type: 'USER_START_WAS_SENT' }
interface GetUserWasSent { type: "GET_USER_WAS_SENT", userIsLoading: boolean }
interface GetUserWasRetrieved { type: "GET_USER_WAS_RETRIEVED", userItems: User[] }

type KnownAction = UserStartedWasReceived | UserStartWasSent | GetUserWasSent | GetUserWasRetrieved;

export const actionCreators = {
    startUser: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
        let fetchTask = fetch('http://localhost:7071/api/UserOrchestrator', {
            method: "post",
            headers: headers
        })
            .then(response => response.json() as Promise<OrchestrationResponse>)
            .then(data => {
                var startedUser = data as OrchestrationResponse;
                dispatch({ type: "USER_STARTED_WAS_RECEIVED", userStartResponse: startedUser });
            });

        addTask(fetchTask);
        dispatch({ type: "USER_START_WAS_SENT" });
    },
    getUser: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');

        let fetchTask = fetch(getState().user.userStartResponse.statusQueryGetUri, {
            method: "get",
            headers: headers
        })
            .then(response => response.text() as Promise<string>)
            .then((data) => {
                var userResponse = JSON.parse(data) as GetUserItemResponse;
                var userItems: User[] = [];
                userResponse.input.forEach((value, index) => {

                    if (index == 0) {
                        userItems.push(value);
                    } else {
                        var found = false;
                        userItems.forEach((v, i) => {
                            if (v.UserId == value.UserId) {
                                found = true;
                            }
                        });

                        if (!found) {
                            userItems.push(value);
                        }
                    }
                });
                dispatch({ type: "GET_USER_WAS_RETRIEVED", userItems: userItems });
            });

        addTask(fetchTask);
        dispatch({ type: "GET_USER_WAS_SENT", userIsLoading: true });
    }
};

//const unloadedState: UserState = { userStartResponse: { id: "", sendEventPostUri: "", statusQueryGetUri: "", terminatePostUri: "" } as OrchestrationResponse, userAuthenticated: false };
const unloadedState: UserState = { userStartResponse: { id: "", sendEventPostUri: "", statusQueryGetUri: "", terminatePostUri: "" } as OrchestrationResponse, userAuthenticated: false, userItems: [], userLoading: false };


export const reducer: Reducer<UserState> = (state: UserState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;

    switch (action.type) {
        case "USER_STARTED_WAS_RECEIVED":
            return { userStartResponse: action.userStartResponse, userAuthenticated: state.userAuthenticated } as UserState;
        case "GET_USER_WAS_RETRIEVED":
            return { userItems: action.userItems , userLoading: false, userStartResponse: state.userStartResponse, userAuthenticated: state.userAuthenticated } as UserState;
        case "GET_USER_WAS_SENT":
            return { userItems: state.userItems, userStartResponse: state.userStartResponse } as UserState;
        // The following line guarantees that every action in the KnownAction union has been covered by a case above
        // const exhaustiveCheck: never = action;
    }

    return state || unloadedState;
};