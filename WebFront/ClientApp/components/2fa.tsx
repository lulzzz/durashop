import { RouteComponentProps } from 'react-router-dom';
import PhoneNumber from './PhoneNumber';
import B2CLogin from './B2CLogin';
import * as React from 'react';


export default class Mfa extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return (
            <div style={{ width: "100%" }}>
                <div style={{ display: "flex" }}>
                <B2CLogin  />
            </div>
        </div>);
    }
}