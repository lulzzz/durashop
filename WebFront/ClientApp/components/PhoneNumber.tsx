import { Modal } from 'react-bootstrap';
import Spinner from './Spinner';
import { ApplicationState } from '../store';
import * as React from 'react';
import { connect } from 'react-redux';
import * as PhoneNumberStore from '../store/PhoneNumberStore';
import { Button } from 'react-bootstrap';
import { RouteComponentProps } from 'react-router';

type Props = PhoneNumberStore.PhoneNumberState & typeof PhoneNumberStore.actionCreators & RouteComponentProps<{}>;

interface IState {
    verificationCode?: string;
    showDialog?: boolean;
}

class PhoneNumber extends React.Component<Props, IState>{
    constructor(props: Props, state: IState) {
        super(props, state);

        this.state = {
            verificationCode: "",
            showDialog: false
        };
    }
    componentWillReceiveProps(props: Props) {
        if (props.isLoading === false && this.props.phoneNumberVerification.id && this.props.verificationCodeAccepted === false)
            this.setState({ showDialog: true });
    }
    handleVerificationCodeChange(event: any) {
        this.setState({ verificationCode: event.target.value });
    }
    handlePhoneNumberChange(event: any) {
        this.props.phoneNumberWasChanged(event.target.value);
    }
    close() {
        this.setState({ showDialog: false });
    }
    render() {
        return <div>
            <h2>Add authentication</h2>
            {this.props.phoneNumberVerification.id && this.props.verificationCodeAccepted ?
                <div>
                    <h1>VERIFICATION ACCEPTED</h1>
                </div> :
                this.props.phoneNumberVerification.id ?
                    <Modal show={this.props.phoneNumberVerification.id !== "" || this.state.showDialog} onHide={this.close.bind(this)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Verification</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <label htmlFor="verificationCode">Verification code</label>
                            <input type="text" id="verificationCode" onChange={this.handleVerificationCodeChange.bind(this)} />
                        </Modal.Body>
                        <Modal.Footer>
                            <Button onClick={this.close.bind(this)}>Cancel</Button>
                            <Button bsStyle="primary" onClick={() => { this.props.submitVerificationCode(this.state.verificationCode); this.close(); }}>Verify</Button>
                        </Modal.Footer>
                    </Modal> :
                    <div className="form-group">
                        <label htmlFor="phoneNumber">Phone Number</label>
                        <div>
                            <input style={{ width: "20%", display: "inline-block" }} className="form-control" type="text" id="phoneNumber" value={this.props.phoneNumber} onChange={this.handlePhoneNumberChange.bind(this)} />
                            <Button bsStyle="primary" style={{ display: "inline-block", marginLeft: 10 }} onClick={() => { this.props.submitPhoneNumberVerification(this.props.phoneNumber) }}>Submit</Button>
                        </div>
                    </div>
            }
        </div>;
    }
}

export default connect(
    (state: ApplicationState) => state.phoneNumberStore,
    PhoneNumberStore.actionCreators
)(PhoneNumber) as typeof PhoneNumber;