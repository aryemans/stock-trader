import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'universal-cookie';

function History() {
    const navigate = useNavigate();
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const [history, setHistory] = useState([]);

    useEffect(() => {
        // Fetch market data from backend API
        const fetchHistoryData = async () => {
            const response = await fetch("/users/view-history", {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            const data = await response.json();
            if (response.ok) {
                //alert('Successful!');
                setHistory(data);
            } else {
                alert(data.message);
            }
        };
        fetchHistoryData();
    }, []);
    if(userRole === "user") {
    return (
        <div>
            <h1>Purchase History</h1>
            <table>
                <thead>
                    <tr>
                        <th>Transaction</th>
                        <th>Stock Ticker</th>
                        <th>Quantity</th>
                        <th>Price</th>
                        <th>Quantity</th>
                        <th>Date Purchased</th>
                    </tr>
                </thead>
                <tbody>
                    {history.map((purchase, index) => (
                        <tr key={index}>
                            <td>{purchase.transaction_type}</td>
                            <td>{purchase.stock_ticker}</td>
                            <td>{purchase.purchasedPrice}</td>
                            <td>{purchase.quantity}</td>
                            <td>{purchase.date}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
    } else {
        navigate("/login");
    }
}

export default History;
