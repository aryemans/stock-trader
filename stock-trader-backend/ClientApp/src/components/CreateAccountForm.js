import React, { useState } from 'react';
import './CreateAccountForm.css';
import { useNavigate } from 'react-router-dom';
import Cookies from 'universal-cookie';

const CreateAccountForm = () => {
    const cookies = new Cookies();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        fullName: '',
        username: '',
        email: '',
        pwd: ''
    });

    const [errors, setErrors] = useState({
        fullName: '',
        username: '',
        email: '',
        pwd: ''
    });

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // validate form data
        const newErrors = {};
        if (!formData.fullName) newErrors.fullName = 'Full name is required';
        if (!formData.username) newErrors.username = 'Username is required';
        if (!formData.email) newErrors.email = 'Email is required';
        if (!formData.pwd) newErrors.pwd = 'Password is required';

        setErrors(newErrors);

        // if form data is valid, submit it to backend
        if (Object.keys(newErrors).length === 0) {
            try {
                const response = await fetch('/users/create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(formData)
                });

                if (response.ok) {
                    cookies.set('Name', formData.username, { path: '/' });
                    cookies.set('UserRole', "user", { path: '/' });
                    navigate("/portfolio");
                } else {
                    alert("Could not create account")
                }
            } catch (err) {
                console.error('Error submitting form:', err);
            }
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <label>
                Full Name:
        <input
                    type="text"
                    name="fullName"
                    value={formData.fullName}
                    onChange={handleChange}
                />
                {errors.fullName && <div className="error">{errors.fullName}</div>}
            </label>
            <label>
                Username:
        <input
                    type="text"
                    name="username"
                    value={formData.username}
                    onChange={handleChange}
                />
                {errors.username && <div className="error">{errors.username}</div>}
            </label>
            <label>
                Email:
        <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                />
                {errors.email && <div className="error">{errors.email}</div>}
            </label>
            <label>
                Password:
        <input
                    type="password"
                    name="pwd"
                    value={formData.pwd}
                    onChange={handleChange}
                />
                {errors.pwd && <div className="error">{errors.pwd}</div>}
            </label>
            <button type="submit">Submit</button>
        </form>
    );
}

export default CreateAccountForm;
