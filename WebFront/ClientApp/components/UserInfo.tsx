import Spinner from './Spinner';
import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as UserStore from '../store/User';

type UserProps =
    UserStore.UserState
    & typeof UserStore.actionCreators
    & UserStore.UserState
    & RouteComponentProps<{}>;

interface IState {
    users: UserStore.User[];
}

class UserInfo extends React.Component<UserProps, IState> {
    constructor(props: UserProps, state: IState) {
        super(props, state);
    
        this.state = {
            users: [
            ]
        }
    }

    componentDidMount() {
        this.props.startUser();
    }

    public render() {
        return <div>
            {this.props.userLoading ? <Spinner /> :
                <div style={{ width: "100%" }}>
                    <div style={{ display: "flex" }}>
                        <div>
                            <h1>User Information</h1>

                            <table className="table table-hover" style={{ width: "100%" }}>
                                <thead>
                                    <tr>
                                        <th>UserId</th>
                                        <th>First Name</th>
                                        <th>Last Name</th>
                                        <th>Email</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {this.state.users.map(value =>
                                        <tr key={value.UserId}>
                                            <td>{value.FirstName}</td>
                                            <td>{value.LastName}</td>
                                            <td>{value.StreetAddress}</td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            }
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.user,
    UserStore.actionCreators
)(UserInfo) as typeof UserInfo;