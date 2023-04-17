import React from 'react';
import { Container } from 'reactstrap';
import  NavMenu from './NavMenu';
import PropTypes from 'prop-types';

function Layout(props) {
  return (
    <div>
      <NavMenu />
      <Container tag="main">
        {props.children}
      </Container>
    </div>
  );
}

Layout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default Layout;