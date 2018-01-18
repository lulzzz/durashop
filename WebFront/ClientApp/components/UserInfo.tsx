import { User } from '../store/User';
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
        this.props.startUser({
            "UserId" : "72ff573d0fce48a2b150089cadca824a",
            "FirstName" : "Johan Jed",
            "LastName" : "Eriksson",
            "UserEmail" : "jed.johan@gmail.com",
            "UserMobile" : "+46765269844",
            "PostalCode" : "23442",
            "City" : "Lomma",
            "StreetAddress" : "Matrosgatan 6",
            "Country" : "jed.johan@gmail.com"
          } as User);
    }
    // Här skulle va nice med bootstrap-form för att mata in en ny user ?
    // Men jag vet inte hur man sen visar usern när den är sparad
    public render() {
        return <div>
            {this.props.userLoading || !this.props.userItems || this.props.userItems.length <= 0 ? <Spinner /> :
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
                                    {this.props.userItems.map(value =>
                                        <tr key={value.UserId}>
                                            <td>{value.FirstName}</td>
                                            <td>{value.LastName}</td>
                                            <td>{value.StreetAddress}</td>
                                            <td>{value.UserEmail}</td>
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