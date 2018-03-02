import { ApplicationState } from '../store';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import * as CheckoutStore from '../store/Checkout';
import * as CartStore from '../store/Cart';
import { Button } from 'react-bootstrap';

type Props = CheckoutStore.CheckoutState & CartStore.CartState & typeof CheckoutStore.actionCreators & RouteComponentProps<{}>;

interface IState {
    checkoutDone: boolean;
}

class Checkout extends React.Component<Props, IState>{
    constructor(props: Props, state: IState) {
        super(props, state);

        this.state = {
            checkoutDone: false
        };
    }

    componentDidMount() {

    }

    checkout() {
        this.props.startCheckout();
        this.setState({
            checkoutDone: true
        });
    }

    render() {
        return <div>
            <h1>Checkout</h1>
            {this.state.checkoutDone ?
                <h3>Checked out {this.props.cartItems.length} items</h3> :
                <Button onClick={() => this.checkout()}>Checkout {this.props.cartItems.length} items</Button>
            }
        </div>
    }
}

export default connect(
    (state: ApplicationState) => state.checkout && state.cart, // Selects which state properties are merged into the component's props
    CheckoutStore.actionCreators
)(Checkout) as typeof Checkout;