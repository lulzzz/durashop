import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { Image } from 'react-bootstrap';

export default class Home extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>DuraShop</h1>
            <p>Sample project to learn about Azure Durable Functions. DuraShop is Micro Service oriented and contains some basic functionality for a serverless e-commerce site, currently these components are included:</p>
            <ul>
            <li><strong>DuraShoppingCart:</strong> Orchestrator to store a customers Shopping Cart. Implements <em>External Events.</em></li>
            <li><strong>DuraShopMFA:</strong> Used for 2 factor authentication with codes in SMS. Implements <em>Human Interaction.</em></li>
            <li><strong>DuraShopUser:</strong> Orchestrator to handle Checkout (payment/order/notification). Implements <em>Function Chaining.</em></li>
            <li><strong>Duracommunication:</strong> Listens to Event Grid and sends mail/SMS. Regular Function.</li>
            <li><strong>SPA Front End:</strong> Simple e-shop. Implements <em>React+Redux.</em></li>
            </ul>
            <img src="../images/durashop.png"/><div style={{ display: "inline-block", fontSize: 20 }}></div>
        </div>;
    }
}
