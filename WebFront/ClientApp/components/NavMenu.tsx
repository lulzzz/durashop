import * as React from 'react';
import { NavLink, Link } from 'react-router-dom';

export class NavMenu extends React.Component<{}, {}> {
    public render() {
        return <div className='main-nav'>
                <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={ '/home' }>DuraShop</Link>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <NavLink to={ '/product' } activeClassName='active'>
                                <span className='glyphicon glyphicon-shopping-cart'></span> Products
                            </NavLink>
                            <NavLink to={ '/checkout' } activeClassName='active'>
                                <span className='glyphicon glyphicon-usd'></span> Checkout
                            </NavLink>
                            <NavLink to={ '/2fa' } activeClassName='active'>
                                <span className='glyphicon glyphicon-phone'></span> Verify Mobile
                            </NavLink>
                            <NavLink to={ '/userinfo' } activeClassName='active'>
                                <span className='glyphicon glyphicon-user'></span> User
                            </NavLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}
