import { addTask } from 'domain-task';
import { AppThunkAction } from './';
import { Action, Reducer } from 'redux';
import OrchestrationResponse from 'ClientApp/commonmodels/OrchestrationResponse';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface CartState {
    cartStartResponse: OrchestrationResponse;
    cartItems: CartItem[];
    counter: number;
}

export interface CartItem {
    CartId: string;
    ItemName: string;
    ItemId: string;
    Price: string;
    UserId: string;
}
// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface CartStartedWasReceived { type: 'CART_STARTED_WAS_RECEIVED', cartStartResponse: OrchestrationResponse }
interface CartStartWasSent { type: 'CART_START_WAS_SENT' }
interface AddCartItemWasSent { type: 'ADD_CART_ITEM_WAS_SENT', counter: number }
// interface DecrementCountAction { type: 'DECREMENT_COUNT' }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = CartStartedWasReceived | CartStartWasSent | AddCartItemWasSent;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    startCart: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
        let fetchTask = fetch('http://localhost:7071/api/CartOrchestrator', {
            method: "post",
            headers: headers
        })
            .then(response => response.json() as Promise<OrchestrationResponse>)
            .then(data => {
                var startedCart = data as OrchestrationResponse;
                dispatch({ type: "CART_STARTED_WAS_RECEIVED", cartStartResponse: startedCart });
            });

        addTask(fetchTask);
        dispatch({ type: "CART_START_WAS_SENT" });
    },
    addItem: (cartItem: CartItem): AppThunkAction<KnownAction> => (dispatch, getState) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');
        cartItem.CartId = getState().cart.cartStartResponse.id;

        let fetchTask = fetch('http://localhost:7071/api/Cart/Update', {
            method: "post",
            headers: headers,
            body: JSON.stringify(cartItem),
            mode: 'no-cors'
        })
            .then(() => { dispatch({ type: "ADD_CART_ITEM_WAS_SENT", counter: getState().cart.counter += 1 }) });

        addTask(fetchTask);
    }
    // increment: () => <IncrementCountAction>{ type: 'INCREMENT_COUNT' },
    // decrement: () => <DecrementCountAction>{ type: 'DECREMENT_COUNT' }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: CartState = { cartStartResponse: { id: "", sendEventPostUri: "", statusQueryGetUri: "", terminatePostUri: "" } as OrchestrationResponse, counter: 0, cartItems: [] };

export const reducer: Reducer<CartState> = (state: CartState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;

    switch (action.type) {
        case "CART_STARTED_WAS_RECEIVED":
            return { cartStartResponse: action.cartStartResponse, counter: state.counter, cartItems: state.cartItems } as CartState;
        case "ADD_CART_ITEM_WAS_SENT":
            return { counter: action.counter, cartItems: state.cartItems, cartStartResponse: state.cartStartResponse } as CartState;
        // The following line guarantees that every action in the KnownAction union has been covered by a case above
        // const exhaustiveCheck: never = action;
    }

    return state || unloadedState;
};
