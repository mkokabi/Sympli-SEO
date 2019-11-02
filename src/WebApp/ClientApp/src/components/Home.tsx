import * as React from 'react';
import { connect } from 'react-redux';
import Search from './Search';

const Home = () => (
  <div>
    <h1>Sympli SEO</h1>
    <Search></Search>
  </div>
);

export default connect()(Home);
