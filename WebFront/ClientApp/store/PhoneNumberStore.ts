import { fetch } from 'domain-task';
import { Action, Reducer } from 'redux';
import { AppThunkAction } from 'ClientApp/store';
import { addTask } from 'domain-task/main';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface PhoneNumberState {
    phoneNumber: string;
    isLoading: boolean;
    phoneNumberVerification: PhoneNumberVerification;
}

export interface PhoneNumberVerification {
    id: string;
    statusQueryGetUri: string;
    sendEventPostUri: string;
    terminatePostUri: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface PhoneNumberWasChanged { type: 'PHONE_NUMBER_WAS_CHANGED', phoneNumber: string }
interface SubmitPhoneNumberVerification { type: 'VERIFICATION_WAS_SENT' }
interface PhoneNumberVerificationWasRetrieved{type: 'PHONE_NUMBER_VERIFICATION_WAS_RETRIEVED', phoneNumberVerification: PhoneNumberVerification}
// interface DecrementCountAction { type: 'DECREMENT_COUNT' }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = PhoneNumberWasChanged | SubmitPhoneNumberVerification | PhoneNumberVerificationWasRetrieved;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    phoneNumberWasChanged: (phoneNumber: string) => <PhoneNumberWasChanged>{ type: "PHONE_NUMBER_WAS_CHANGED", phoneNumber: phoneNumber },
    submitPhoneNumberVerification: (phoneNumber: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
        let fetchTask = fetch('http://localhost:7071/api/orchestrators/SMSPhoneVerification', {
            method: "post",
            headers: headers,
            body: JSON.stringify(phoneNumber)
        })
        .then(response => response.json() as Promise<PhoneNumberVerification>)
        .then(data => {
            dispatch({type: "PHONE_NUMBER_VERIFICATION_WAS_RETRIEVED", phoneNumberVerification: data});
        });

        addTask(fetchTask);
        dispatch({type: "VERIFICATION_WAS_SENT"});
    }
    // increment: () => <IncrementCountAction>{ type: 'INCREMENT_COUNT' },
    // decrement: () => <DecrementCountAction>{ type: 'DECREMENT_COUNT' }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: PhoneNumberState = { phoneNumber: "", isLoading: false, phoneNumberVerification: { id: "", sendEventPostUri: "", statusQueryGetUri: "", terminatePostUri: "" } };

export const reducer: Reducer<PhoneNumberState> = (state: PhoneNumberState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;

    switch (action.type) {
        case "PHONE_NUMBER_WAS_CHANGED":
            return {
                phoneNumber: action.phoneNumber,
                isLoading: state.isLoading,
                phoneNumberVerification: state.phoneNumberVerification
            };
        case "PHONE_NUMBER_VERIFICATION_WAS_RETRIEVED":
            return {
                phoneNumber: state.phoneNumber,
                phoneNumberVerification: action.phoneNumberVerification,
                isLoading: state.isLoading
            };
        // The following line guarantees that every action in the KnownAction union has been covered by a case above
        // const exhaustiveCheck: never = action;
    }

    return state || unloadedState;
};
