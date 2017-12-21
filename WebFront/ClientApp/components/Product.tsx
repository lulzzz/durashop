import Spinner from './Spinner';
import PhoneNumber from './PhoneNumber';
import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as ProductStore from '../store/ProductStore';
import * as CartStore from '../store/Cart';
import Cart from './Cart';

type ProductProps =
    ProductStore.ProductState
    & typeof ProductStore.actionCreators
    & typeof CartStore.actionCreators
    & CartStore.CartState
    & RouteComponentProps<{}>;

interface IState {
    products: CartStore.CartItem[];
}

class Product extends React.Component<ProductProps, IState> {
    constructor(props: ProductProps, state: IState) {
        super(props, state);

        this.state = {
            products: [
                { CartId: "", ItemId: "1", ItemName: "My cool stuff", Price: "100", UserId: "123-345" } as CartStore.CartItem,
                { CartId: "", ItemId: "2", ItemName: "My other cool stuff", Price: "200", UserId: "123-345" } as CartStore.CartItem
            ]
        }
    }

    handleRowClick(product: CartStore.CartItem) {
        this.props.addItem(product);
    }

    public render() {
        return <div>
            {this.props.cartLoading ? <Spinner /> :
                <div style={{ width: "100%" }}>
                    <div style={{ display: "flex" }}>
                        <div>
                            <h1>Buy products</h1>

                            <table className="table table-hover" style={{ width: "100%" }}>
                                <thead>
                                    <tr>
                                        <th>Product Id</th>
                                        <th>Product Name</th>
                                        <th>Price ($)</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {this.state.products.map(value =>
                                        <tr key={value.ItemId} onClick={() => { this.handleRowClick(value) }}>
                                            <td>{value.ItemId}</td>
                                            <td>{value.ItemName}</td>
                                            <td>{value.Price}</td>
                                            <td><button type="button" className="btn btn-primary" >Add to cart</button></td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                        <div>
                            <Cart />
                        </div>
                    </div>

                    <PhoneNumber />
                    {/* <button onClick={() => { this.props.increment() }}>Increment</button> */}
                </div>
            }
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.product, // Selects which state properties are merged into the component's props
    Object.assign(ProductStore.actionCreators, CartStore.actionCreators)                 // Selects which action creators are merged into the component's props
)(Product) as typeof Product;