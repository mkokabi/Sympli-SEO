import * as React from 'react';
import { connect } from 'react-redux';
import Button from 'reactstrap/lib/Button';

const Home = () => (
  <div>
    <h1>Sympli SEO</h1>
    <ul>
      <li>Enter the URL you are interested:</li>
      <li>Enter the keywords:</li>
      <li><Button type="button" className="btn btn-success">Search</Button></li>
    </ul>
  </div>
);

export default connect()(Home);
