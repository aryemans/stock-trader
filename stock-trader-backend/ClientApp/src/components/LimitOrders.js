import React, { useState, useEffect } from "react";
import { useNavigate } from 'react-router';
import Cookies from 'universal-cookie';

function LimitOrders() {
    const [limitOrders, setLimitOrders] = useState([]);
    const navigate = useNavigate();
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');

    useEffect(() => {
        fetch("/users/get-limit-orders", {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then((res) => res.json())
            .then((data) => {
                setLimitOrders(data);
            })
            .catch((error) => console.log(error));
    }, [limitOrders]);

    const handleCancel = async (orderId) => {
        console.log(orderId);
        await fetch(`/users/delete-order/${orderId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then((res) => res.json())
            .then((data) => {
                setLimitOrders(data);
            })
            .catch((error) => console.log(error));
    };

    if(userRole === "user") {
    return (
        <div>
            <h1>Limit Orders</h1>
            {limitOrders.length > 0 ? (
             <div>   
            <table>
                <thead>
                    <tr>
                        <th>Order Id</th>
                        <th>Stock</th>
                        <th>Price</th>
                        <th>Expire Date</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {limitOrders.map((order, index) => (
                        <tr key={index}>
                            <td>{order.order_id}</td>
                            <td>{order.stock_ticker}</td>
                            <td>{order.price}</td>
                            <td>{order.expireDate}</td>
                            <td>
                                <button onClick={() => handleCancel(order.order_id)}>
                                    Cancel
                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div> ) : (
            <div> 
               <p> You currently have no limit orders </p>
            </div>
        ) }
        </div>
    );
   } else {
        navigate("/login");
   }

}

export default LimitOrders;
