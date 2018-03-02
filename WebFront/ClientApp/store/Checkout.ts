import MainService from '../services/mainservice';
import { fetch } from 'domain-task';
import { Action, Reducer } from 'redux';
import { AppThunkAction } from 'ClientApp/store';
import { addTask } from 'domain-task/main';

declare const __API__: string;

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface CheckoutState {

}

interface CheckoutModel {
    CartId: string;
    CartUrl: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface DecrementCountAction { type: 'DECREMENT_COUNT' }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = DecrementCountAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    startCheckout: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var obj = {
            CartId: getState().cart.cartStartResponse.id,
            CartUrl: getState().cart.cartStartResponse.statusQueryGetUri
        } as CheckoutModel;

        MainService.fetch(true, "checkout", "post", JSON.stringify(obj))
            .then((results) => {
                debugger;
            });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: CheckoutState = {};

export const reducer: Reducer<CheckoutState> = (state: CheckoutState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;

    switch (action.type) {

        // The following line guarantees that every action in the KnownAction union has been covered by a case above
        // const exhaustiveCheck: never = action;
    }

    return state || unloadedState;
};
