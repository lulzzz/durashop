import * as WeatherForecasts from './WeatherForecasts';
import * as ProductStore from './ProductStore';
import * as PhoneNumberStore from './PhoneNumberStore';
import * as CartStore from './Cart';
import * as UserStore from './User'

// The top-level state object
export interface ApplicationState {
    product: ProductStore.ProductState;
    weatherForecasts: WeatherForecasts.WeatherForecastsState;
    phoneNumberStore: PhoneNumberStore.PhoneNumberState;
    cart: CartStore.CartState;
    user: UserStore.UserState;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    product: ProductStore.reducer,
    weatherForecasts: WeatherForecasts.reducer,
    phoneNumberStore: PhoneNumberStore.reducer,
    cart: CartStore.reducer,
    user: UserStore.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
