import { addTask } from 'domain-task';
import { AppThunkAction } from './';
import { Action, Reducer } from 'redux';
import OrchestrationResponse from 'ClientApp/commonmodels/OrchestrationResponse';

declare const __API__: string;

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
    input: User;
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
    startUser: (user: User): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
        let fetchTask = fetch(__API__ + 'api/UserOrchestrator', {
            method: "post",
            headers: headers,
            body: JSON.stringify(user)
        })
            .then(response => response.json() as Promise<OrchestrationResponse>)
            .then(data => {
                var startedUser = data as OrchestrationResponse;
                dispatch({ type: "USER_STARTED_WAS_RECEIVED", userStartResponse: startedUser });
                actionCreators.getUser()(dispatch, getState);
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
            .then(response => response.json() as Promise<GetUserItemResponse>)
            .then((data) => {
                dispatch({ type: "GET_USER_WAS_RETRIEVED", userItems: [data.input] });
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