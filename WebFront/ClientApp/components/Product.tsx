import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as ProductStore from '../store/ProductStore';
import * as WeatherForecasts from '../store/WeatherForecasts';

type ProductProps =
    ProductStore.ProductState
    & typeof ProductStore.actionCreators
    & RouteComponentProps<{}>;

interface IState {
    products: ProductStore.Product[];
}

class ShowProduct extends React.Component<ProductProps, IState> {
    constructor(props: ProductProps, state: IState) {
        super(props, state);

        this.state = {
            products: [{ productId: "1", productName: "Regular Function", description: "This is the 'older' Azure function" } as ProductStore.Product, { productId: "2", productName: "Durable Function", description: "This is the 'newer' and more durable Azure function" } as ProductStore.Product]
        }
    }

    handleRowClick(product: ProductStore.Product) {
        debugger;
    }

    public render() {
        return <div>
            <h1>Buy products</h1>

            <table className="table table-hover" style={{width: "50%"}}>
                <thead>
                    <tr>
                        <th>Product Id</th>
                        <th>Product Name</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.products.map(value =>
                        <tr key={value.productId} onClick={() => { this.handleRowClick(value) }}>
                            <td>{value.productId}</td>
                            <td>{value.productName}</td>
                            <td>{value.description}</td>
                            <td><button type="button" className="btn btn-primary" >Add to cart</button></td>
                        </tr>
                    )}
                </tbody>
            </table>

            {/* <button onClick={() => { this.props.increment() }}>Increment</button> */}
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.product, // Selects which state properties are merged into the component's props
    ProductStore.actionCreators                 // Selects which action creators are merged into the component's props
)(ShowProduct) as typeof ShowProduct;