import { ApplicationState } from '../store';
import * as React from 'react';
import { connect } from 'react-redux';
import * as PhoneNumberStore from '../store/PhoneNumberStore';

type Props = PhoneNumberStore.PhoneNumberState & typeof PhoneNumberStore.actionCreators;

class PhoneNumber extends React.Component<Props, {}>{
    constructor(props: Props) {
        super(props);

        
    }
    handleChange(event: any) {
        this.props.phoneNumberWasChanged(event.target.value);
    }
    render() {
        return <div>
            <h2>Add authentication</h2>
            <div className="form-group">
                <label htmlFor="phoneNumber">Phone Number</label>
                <input style={{width: "50%"}} className="form-control" type="text" id="phoneNumber" value={this.props.phoneNumber} onChange={this.handleChange.bind(this)} />
                <button onClick={() => {this.props.submitPhoneNumberVerification(this.props.phoneNumber)}}>Submit</button>
            </div>
        </div>;
    }
}

export default connect(
    (state: ApplicationState) => state.phoneNumberStore,
    PhoneNumberStore.actionCreators
)(PhoneNumber);