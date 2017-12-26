import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Product from './components/Product';
import UserInfo from './components/UserInfo';
import mfa from './components/2fa';

export const routes = <Layout>
    <Route path='/home' component={ Home } />
    <Route path='/product' component={ Product } />
    <Route path='/userinfo' component={ UserInfo } />
    <Route path='/2fa' component={ mfa } />
</Layout>;
