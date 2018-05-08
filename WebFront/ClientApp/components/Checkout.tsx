import { ApplicationState } from '../store';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { connect } from 'react-redux';
import * as CheckoutStore from '../store/Checkout';
import * as CartStore from '../store/Cart';
import { Button } from 'react-bootstrap';

type Props = CheckoutStore.CheckoutState & CartStore.CartState & typeof CheckoutStore.actionCreators & typeof CartStore.actionCreators & RouteComponentProps<{}>;

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
        this.props.startCart();
    }

    getTotalPrice(): number {
        var totalPrice = 0;

        this.props.cartItems.forEach((value, index) => {
            totalPrice += (Number.parseInt(value.Price) * value.TotalCount);
        });

        return totalPrice;
    }

    checkout() {
        this.props.startCheckout();
        this.props.clearCartItems();
        this.setState({
            checkoutDone: true
        });

        localStorage.removeItem("myCart");
        localStorage.removeItem("cartItems");
    }

    render() {
        return <div>
            <h1>Checkout</h1>
            <table className="table table-hover" style={{ width: "100%" }}>
                <thead>
                    <tr>
                        <th>Product Id</th>
                        <th>Product Name</th>
                        <th>Price ($)</th>
                        <th>Total items</th>
                    </tr>
                </thead>
                <tbody>
                    {this.props.cartItems.map((value, index) =>
                        <tr key={index}>
                            <td>{value.ItemId}</td>
                            <td>{value.ItemName}</td>
                            <td>{value.Price}</td>
                            <td>{value.TotalCount}</td>
                        </tr>
                    )}
                </tbody>
            </table>
            <h4 style={{ marginTop: "-15px" }}>Total Price: ${this.getTotalPrice()}</h4>
            {this.state.checkoutDone ?
                <h3>Checked out items</h3> :
                <Button style={{ marginTop: "10px" }} onClick={() => this.checkout()}>Buy</Button>
            }
        </div>
    }
}

export default connect(
    (state: ApplicationState) => state.checkout && state.cart, // Selects which state properties are merged into the component's props
    Object.assign(CheckoutStore.actionCreators, CartStore.actionCreators)
)(Checkout) as typeof Checkout;