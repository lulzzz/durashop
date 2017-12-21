import { ApplicationState } from '../store';
import * as CartStore from '../store/Cart';
import * as React from 'react';
import { connect } from 'react-redux';
import { Modal, Button } from 'react-bootstrap';

type Props = CartStore.CartState & typeof CartStore.actionCreators;

interface State {
    showDialog: boolean;
}

class Cart extends React.Component<Props, State>{
    /**
     *
     */
    constructor(props: Props, state: State) {
        super(props, state);

        this.state = {
            showDialog: false
        };
    }
    componentDidMount() {
        this.props.startCart();
    }

    componentWillReceiveProps(props: Props) {
        
    }

    async imageClick() {
        await this.props.getCartItems();
        this.setState({
            showDialog: true
        });
    }

    close(){
        this.setState({showDialog: false});
    }

    render() {
        return <div>
            <img onClick={this.imageClick.bind(this)} src="../images/durashop-small.png" style={{ cursor: "pointer" }} /><div style={{ display: "inline-block", fontSize: 20 }}>({this.props.counter})</div>

            <Modal show={this.state.showDialog} onHide={this.close.bind(this)}>
                <Modal.Header closeButton>
                    <Modal.Title>Cart items</Modal.Title>
                </Modal.Header>
                <Modal.Body>
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
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.close.bind(this)}>Close</Button>
                </Modal.Footer>
            </Modal>
        </div>
    }
}

export default connect(
    (state: ApplicationState) => state.cart,
    CartStore.actionCreators
)(Cart); 