import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'universal-cookie';
import './WithdrawDeposit.css';

const WithdrawDeposit = () => {
    const [amount, setAmount] = useState(0);
    const navigate = useNavigate();
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const handleAmountChange = (event) => {
        setAmount(event.target.value);
    };

    const handleWithdraw = () => {
        fetch('/users/withdraw-funds', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ amount: amount })
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
                navigate("/portfolio")
            })
            .catch(error => {
                console.error('Error:', error);
                // Handle error here
            });
    };

    const handleDeposit = () => {
        fetch('/users/deposit-funds', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ amount: amount })
        })
            .then(response => response.json())
            .then(data => {
                console.log(data);
                navigate("/portfolio");
                // Handle response data here
            })
            .catch(error => {
                console.error('Error:', error);
                // Handle error here
            });
    };

    if(userRole === "user") {
    return (
        <div>
            <h1>Withdraw/Deposit</h1>
            <label>
                Amount:
        <input type="number" value={amount} onChange={handleAmountChange} />
            </label>
            <button onClick={handleWithdraw}>Withdraw</button>
            <button onClick={handleDeposit}>Deposit</button>
        </div>
    );
    } else {
        navigate("/login")
    }
}

export default WithdrawDeposit;
