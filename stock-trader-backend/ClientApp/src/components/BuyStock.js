import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Cookies from 'universal-cookie';
import './BuyStock.css';

function BuyStock() {
    const navigate = useNavigate();
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const [quantity, setQuantity] = useState('');
    const [price, setPrice] = useState('');
    const [limitPrice, setLimitPrice] = useState('');
    const [expireDate, setExpireDate] = useState('');
    const {stock} = useParams();

    useEffect(() => {
        const fetchPriceData = async () => {
            const response = await fetch(`/market/current-price/${stock}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            const data = await response.json();
            setPrice(data.current_price);
            //console.log(stock);
        };
        fetchPriceData();
    }, []);
    
    
    const handleBuy = async () => {
        const response = await fetch(`/users/buy-stock/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                stock_ticker: stock,
                volume: parseInt(quantity)
            })
        });
        const data = await response.json();
        if (response.ok) {
            alert('Stock bought successfully!');
            console.log(data.message);
            navigate('/portfolio');
        } else {
            alert(data.message);
        }
    };

    const handleSetLimitOrder = async () => {
        const response = await fetch(`/users/limit-order-buy`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                stock_ticker: stock,
                current_price: limitPrice,
                volume: parseInt(quantity),
                expire_date: expireDate     
            })
        });
        const data = await response.json();
        // handle success or error
        if (response.ok) {
            alert('Limit order set successfully!');
            navigate('/limit-orders');
        } else {
            alert(data.message);
        }
    }; 

    if(userRole === "user") {
    return (
        <div>
        <h1> {stock} {price} </h1>
        <h2>Buy Stock</h2>
        <form>
            <label>
                Quantity:
      <input
                    type="number"
                    value={quantity}
                    onChange={(event) => setQuantity(event.target.value)}
                />
            </label>
            <br />
            <button onClick={handleBuy}>Buy</button>
        </form>
        <br />
        <hr />
        <h2>Set Limit Order</h2>
        <form>
            <br />
            <label>
                Quantity:
      <input
                    type="number"
                    value={quantity}
                    onChange={(event) => setQuantity(event.target.value)}
                />
            </label>
            <label>
                Price:
      <input
                    type="number"
                    value={limitPrice}
                    onChange={(event) => setLimitPrice(event.target.value)}
                />
            </label>
            <label>
                Expire Date:
      <input 
                    type="date"
                    value={expireDate} 
                    onChange={e => setExpireDate(e.target.value)} 
                />
            </label>
            <br />
            <button onClick={handleSetLimitOrder}>Set Limit Order</button>
        </form>
    </div>
    );
    } else {
        navigate("/login");
    }
}

export default BuyStock;