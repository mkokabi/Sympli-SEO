import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import FetchSearchResults from './components/FetchSearchResults';

import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/fetch-data/:startIndex?' component={FetchSearchResults} />
    </Layout>
);
