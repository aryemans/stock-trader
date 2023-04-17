import React from 'react';
import { Link } from 'react-router-dom';
import CreateAccountForm  from './CreateAccountForm';

const Signup = () => {
  return (
        <div>
            <h1>Sign up</h1>
            <CreateAccountForm />
            <p>
                Already have an account? <Link to="/login">Login</Link> now!
      </p>
        </div>
    );
};

export default Signup;