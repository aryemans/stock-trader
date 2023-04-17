import React, { useState, useEffect } from "react";
import { useNavigate, useParams} from 'react-router-dom';
import Cookies from 'universal-cookie';

function SellStock() {
    const navigate = useNavigate();
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const [quantity, setQuantity] = useState(0);
    const [price, setPrice] = useState(0);
    const [limitPrice, setLimitPrice] = useState(0);
    const [limitQuantity, setLimitQuantity] = useState(0);
    const [expireDate, setExpireDate] = useState('');
    const {stock} = useParams();
    //const stock_ticker = location.state.stock;

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
            console.log(stock);
        };
        fetchPriceData();
    }, []);

    const handleSellStocks = () => {
        const sellStockUrl = "/users/sell-stock";
        const body = {
            stock_ticker: stock,
            volume: parseInt(quantity),  
        };

        const headers = {
            "Content-Type": "application/json",
        };

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(body),
        };

        fetch(sellStockUrl, requestOptions)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Failed to sell stocks");
                }
                if (response.ok) {
                    alert('Stock sold successfully!');
                    navigate('/assets');
                }
                // handle successful response here
            })
            .catch((error) => {
                alert(error.message);
            });
    };

    const handleSetLimitOrder = () => {
        const setLimitOrderUrl = "/users/limit-order-sell";
        const body = {
            stock_ticker: stock,
            volume: parseInt(quantity),
            current_price: limitPrice,
            expire_date: expireDate   
        };

        const headers = {
            "Content-Type": "application/json",
        };

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(body),
        };

        fetch(setLimitOrderUrl, requestOptions)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Failed to set limit order");
                }
                if (response.ok) {
                    alert('Limit order set successfully!');
                    navigate('/assets');
                }
                // handle successful response here
            })
            .catch((error) => {
                alert(error.message);
            });
    };

    if(userRole === "user") {
    return (
        <div>
            <h1> {stock} {price} </h1>
            <h2>Sell Stock</h2>
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
                <button onClick={handleSellStocks}>Sell</button>
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
                        value={limitQuantity}
                        onChange={(event) => setLimitQuantity(event.target.value)}
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
        </div> );
    } else {
        navigate("/login");
    }
}

export default SellStock;