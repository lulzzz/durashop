import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Product from './components/Product';
import UserInfo from './components/UserInfo';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/product' component={ Product } />
    <Route path='/userinfo' component={ UserInfo } />
    <Route path='/fetchdata/:startDateIndex?' component={ FetchData } />
</Layout>;
