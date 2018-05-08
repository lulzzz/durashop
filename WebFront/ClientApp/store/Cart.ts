import { addTask } from 'domain-task';
import { AppThunkAction, ApplicationState } from './';
import { Action, Reducer } from 'redux';
import OrchestrationResponse from 'ClientApp/commonmodels/OrchestrationResponse';
import { Header } from 'react-bootstrap/lib/Modal';
import MainService from '../services/mainservice'
import ErrorHandler from '../services/errorhandler';

declare const __API__: string;
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface CartState {
    cartStartResponse: OrchestrationResponse;
    cartItems: CartItem[];
    counter: number;
    cartLoading: boolean;
}

export interface CartItem {
    CartId: string;
    ItemName: string;
    ItemId: string;
    Price: string;
    UserId: string;
    TotalCount: number;
}

export interface GetCartItemResponse {
    runtimeStatus: string;
    input: CartItem[];
    output: any[];
    createdTime: Date;
    lastUpdatedTime: Date;
}
// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

interface CartStartedWasReceived { type: 'CART_STARTED_WAS_RECEIVED', cartStartResponse: OrchestrationResponse }
interface CartStartFailed { type: "CART_START_FAILED" }
interface CartStartWasSent { type: 'CART_START_WAS_SENT' }
interface AddCartItemWasSent { type: 'ADD_CART_ITEM_WAS_SENT', cartIsLoading: boolean }
interface AddCartItemIsSent { type: "ADD_CART_ITEM_IS_SENT", counter: number, cartItems: CartItem[] }
interface DeleteCartItemWasSent { type: 'DELETE_CART_ITEM_WAS_SENT', cartIsLoading: boolean }
interface DeleteCartItemIsSent { type: "DELETE_CART_ITEM_IS_SENT", counter: number }
interface GetCartWasSent { type: "GET_CART_WAS_SENT", cartIsLoading: boolean }
interface GetCartWasRetrieved { type: "GET_CART_WAS_RETRIEVED", cartItems: CartItem[] }
// interface DecrementCountAction { type: 'DECREMENT_COUNT' }

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = CartStartedWasReceived | CartStartWasSent | AddCartItemWasSent | AddCartItemIsSent | DeleteCartItemWasSent | DeleteCartItemIsSent | GetCartWasSent | GetCartWasRetrieved | CartStartFailed;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    startCart: (counter?: number): AppThunkAction<KnownAction> => (dispatch, getState) => {
        if (localStorage["myCart"]) {
            dispatch({ type: 'CART_STARTED_WAS_RECEIVED', cartStartResponse: JSON.parse(localStorage['myCart']) as OrchestrationResponse });

            if (localStorage["cartItems"]) {
                dispatch({
                    type: "ADD_CART_ITEM_IS_SENT",
                    counter: Internals.getCartCount(JSON.parse(localStorage["cartItems"]) as CartItem[]),
                    cartItems: JSON.parse(localStorage["cartItems"])
                });
            }
            return;
        }
        if (localStorage)
            MainService.fetch(true, 'cart', 'post')
                .then(response => response.json() as Promise<OrchestrationResponse>)
                .then(data => {
                    var startedCart = data as OrchestrationResponse;
                    localStorage["myCart"] = JSON.stringify(startedCart);
                    dispatch({ type: "CART_STARTED_WAS_RECEIVED", cartStartResponse: startedCart });
                })
                .catch((reason) => {
                    ErrorHandler.Handle(dispatch, { type: "CART_START_FAILED" }, reason);
                });

        dispatch({ type: "CART_START_WAS_SENT" });
    },
    addItem: (cartItem: CartItem): AppThunkAction<KnownAction> => (dispatch, getState) => {
        cartItem.CartId = getState().cart.cartStartResponse.id;
        MainService.fetch(true, 'cart/update', "post", JSON.stringify(cartItem))
            .then(response => response.json() as Promise<OrchestrationResponse>)
            .then((response) => {
                var cartItems = Internals.mapCartItems(response, false);
                localStorage["cartItems"] = JSON.stringify(cartItems);
                dispatch({ type: "ADD_CART_ITEM_IS_SENT", counter: Internals.getCartCount(cartItems), cartItems: cartItems });
            })
            .catch(reason => {
                ErrorHandler.Handle(dispatch, { type: "CART_START_FAILED" }, reason);
            });

        dispatch({ type: "ADD_CART_ITEM_WAS_SENT", cartIsLoading: true });
    },
    deleteItem: (cartItem: CartItem): AppThunkAction<KnownAction> => (dispatch, getState) => {
        cartItem.CartId = getState().cart.cartStartResponse.id;
        MainService.fetch(true, "cart/update", "delete", JSON.stringify(cartItem))
            .then((response) => {
                actionCreators.getCartItems()(dispatch, getState);
                dispatch({ type: "DELETE_CART_ITEM_IS_SENT", counter: getState().cart.counter -= 1 });
            })
            .catch(reason => {
                ErrorHandler.Handle(dispatch, { type: "CART_START_FAILED" }, reason);
            });

        dispatch({ type: "DELETE_CART_ITEM_WAS_SENT", cartIsLoading: true });
    },
    doSomePolling(state: ApplicationState, pollingTimes: number): Promise<void> {
        pollingTimes += 1;

        return new Promise((resolve, reject) => {
            var headers = new Headers();
            headers.append('Content-Type', 'application/json');

            let fetchTask = fetch(state.cart.cartStartResponse.statusQueryGetUri, {
                method: "get",
                headers: headers
            })
                .then(response => response.text() as Promise<string>)
                .then((data) => {
                    var cartItemResponse = JSON.parse(data) as GetCartItemResponse;
                    cartItemResponse.runtimeStatus === "Completed" ? console.log("COMPLETED") : "";
                    if (cartItemResponse.output != null) {
                        console.log("polled: " + pollingTimes);
                        this.doSomePolling(state, pollingTimes);
                        return;
                    }
                    resolve();
                });
        });
    },
    getCartItems: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        MainService.fetch(false, getState().cart.cartStartResponse.statusQueryGetUri, 'get')
            .then(response => response.text() as Promise<string>)
            .then((data) => {
                var cartItems = Internals.mapCartItems(data, true);
                dispatch({ type: "GET_CART_WAS_RETRIEVED", cartItems: cartItems });
            })
            .catch(reason => {
                ErrorHandler.Handle(dispatch, { type: "CART_START_FAILED" }, reason);
            });

        dispatch({ type: "GET_CART_WAS_SENT", cartIsLoading: true });
    }
    // increment: () => <IncrementCountAction>{ type: 'INCREMENT_COUNT' },
    // decrement: () => <DecrementCountAction>{ type: 'DECREMENT_COUNT' }
};

const Internals = {
    mapCartItems(data: any, parseData: boolean): CartItem[] {
        var cartItemResponse = parseData ? (JSON.parse(data) as GetCartItemResponse).input : data as CartItem[];
        var cartItems: CartItem[] = [];
        cartItemResponse.forEach((value, index) => {
            value.TotalCount = 1;

            if (index == 0) {
                cartItems.push(value);
            } else {
                var found = false;
                cartItems.forEach((v, i) => {
                    if (v.ItemId == value.ItemId) {
                        v.TotalCount += 1;
                        found = true;
                    }
                });

                if (!found) {
                    cartItems.push(value);
                }
            }
        });

        return cartItems;
    },
    getCartCount(cartItems: CartItem[]): number {
        var count = 0;

        cartItems.forEach((value, index) => {
            count += value.TotalCount;
        });

        return count;
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
const unloadedState: CartState = { cartStartResponse: { id: "", sendEventPostUri: "", statusQueryGetUri: "", terminatePostUri: "" } as OrchestrationResponse, counter: 0, cartItems: [], cartLoading: false };

export const reducer: Reducer<CartState> = (state: CartState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;

    switch (action.type) {
        case "CART_STARTED_WAS_RECEIVED":
            return { cartStartResponse: action.cartStartResponse, counter: state.counter, cartItems: state.cartItems, cartLoading: false } as CartState;
        case "ADD_CART_ITEM_WAS_SENT":
            return { counter: state.counter, cartItems: state.cartItems, cartStartResponse: state.cartStartResponse, cartLoading: true } as CartState;
        case "ADD_CART_ITEM_IS_SENT":
            return { cartItems: action.cartItems, cartLoading: false, cartStartResponse: state.cartStartResponse, counter: action.counter } as CartState;
        case "GET_CART_WAS_RETRIEVED":
            return { cartItems: action.cartItems, cartLoading: false, cartStartResponse: state.cartStartResponse, counter: state.counter } as CartState;
        case "GET_CART_WAS_SENT":
            return { cartItems: state.cartItems, cartLoading: true, cartStartResponse: state.cartStartResponse, counter: state.counter } as CartState;
        case "CART_START_WAS_SENT":
            return { cartItems: state.cartItems, cartLoading: true, cartStartResponse: state.cartStartResponse, counter: state.counter } as CartState;
        case "CART_START_FAILED":
            return { cartItems: state.cartItems, cartLoading: false, cartStartResponse: state.cartStartResponse, counter: state.counter } as CartState;
        case "DELETE_CART_ITEM_IS_SENT":
            return { cartItems: state.cartItems, cartLoading: false, cartStartResponse: state.cartStartResponse, counter: action.counter } as CartState;
        case "DELETE_CART_ITEM_WAS_SENT":
            return { cartItems: state.cartItems, cartLoading: action.cartIsLoading, cartStartResponse: state.cartStartResponse, counter: state.counter } as CartState;
        // The following line guarantees that every action in the KnownAction union has been covered by a case above
        // const exhaustiveCheck: never = action;
    }

    return state || unloadedState;
};
