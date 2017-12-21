import { ApplicationState } from '../store';
import * as CartStore from '../store/Cart';
import * as React from 'react';
import { connect } from 'react-redux';

type Props = CartStore.CartState & typeof CartStore.actionCreators;

class Cart extends React.Component<Props, {}>{
    componentDidMount() {
        this.props.startCart();
    }
    componentWillReceiveProps(props: Props){
        debugger;
    }
    
    render() {
        return <div>
            <img src="../images/durashop-small.png" style={{ cursor: "pointer" }} /><div style={{ display: "inline-block", fontSize: 20 }}>({this.props.counter})</div>
        </div>
    }
}

export default connect(
    (state: ApplicationState) => state.cart,
    CartStore.actionCreators
)(Cart); 