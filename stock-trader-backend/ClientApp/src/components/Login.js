import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import Cookies from 'universal-cookie';
import './Login.css';

const LoginPage = () => {
   const cookies = new Cookies();
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [pwd, setPwd] = useState("");
  const [selectedRole, setSelectedRole] = useState('user');

  const handleRoleChange = (event) => {
    setSelectedRole(event.target.value);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    // Handle form submission based on selected role
      if (selectedRole === 'admin') {
          console.log(username);
          console.log(pwd);
        const response = await fetch(`/auth/admin`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username: username, pwd: pwd })
        });
        const data = await response.json();
        if (response.ok) {
            //alert('Login successful!');
            cookies.set('Name', username, { path: '/' });
            cookies.set('UserRole', "admin", { path: '/' });
            navigate('/');
        } else {
            alert(data.message);
        } 
    } else {
      // Handle user login
          //console.log(JSON.stringify({ username: username, pwd: pwd });
      const response = await fetch(`/auth/user`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
          body: JSON.stringify({ username: username, pwd: pwd })
        });
       const data = await response.json();
        if (response.ok) {
            //alert('Log in successful!');
            cookies.set('Name', username, { path: '/' });
            cookies.set('UserRole', "user", { path: '/' });
            navigate('/portfolio');
        } else {
            alert(data.message);
        } 
    }
  };

  return (
    <div>
      <h1>Login Page</h1>
      <div>
        <h2>Select Role</h2>
        <label className="radio-label">
          <input className="radio" type="radio" value="admin" checked={selectedRole === 'admin'} onChange={handleRoleChange} />
          Admin
        </label>
        <label className="radio-label">
          <input className="radio" type="radio" value="user" checked={selectedRole === 'user'} onChange={handleRoleChange} />
          User
        </label>
      </div>
      <form onSubmit={handleSubmit}>
        <label>
                  Username:
          <input type="text" name="username" onChange={(event) => setUsername(event.target.value)} />

        </label>
        <br />
        <label>
          Password:
          <input type="password" name="password" onChange={(event) => setPwd(event.target.value)} />
        </label>
        <br />
        <button type="submit">Login</button>
      </form>
      <p>
                Don&apos;t have an account? <Link to='/signup'>Sign up</Link> now!
      </p>
    </div>
  );
};

export default LoginPage;


