
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import Cookies from 'universal-cookie';

function CreateStock() {
    const [companyName, setCompanyName] = useState("");
    const [stockTicker, setStockTicker] = useState("");
    const [volume, setVolume] = useState(0);
    const [initialPrice, setInitialPrice] = useState(0);
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const navigate = useNavigate();

    const handleSubmit = (event) => {
        event.preventDefault();
        // Send the new stock data to the backend for storage
        fetch("/admin/create-stock", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                companyName: companyName,
                stock_ticker: stockTicker,
                volume: volume,
                opening_price: initialPrice
            }),
        })
            .then((response) => {
                if (!response.ok) {
                    alert("Failed to create stock");
                } else {
                    navigate("/");
                }
                // Redirect to the homepage or show a success message
            })
            .catch((error) => {
                console.log(error);
                // Display an error message to the user
            });
    };

    if(userRole === "admin") {
    return (
        <div>
            <h1>Create a New Stock</h1>
            <form onSubmit={handleSubmit}>
                <label htmlFor="companyName">Company Name:</label>
                <input
                    type="text"
                    id="companyName"
                    value={companyName}
                    onChange={(event) => setCompanyName(event.target.value)}
                />
                <br />
                <label htmlFor="stockTicker">Stock Ticker:</label>
                <input
                    type="text"
                    id="stockTicker"
                    value={stockTicker}
                    onChange={(event) => setStockTicker(event.target.value)}
                />
                <br />
                <label htmlFor="volume">Volume:</label>
                <input
                    type="number"
                    id="volume"
                    value={volume}
                    onChange={(event) => setVolume(event.target.value)}
                />
                <br />
                <label htmlFor="initialPrice">Initial Price:</label>
                <input
                    type="number"
                    id="initialPrice"
                    value={initialPrice}
                    onChange={(event) => setInitialPrice(event.target.value)}
                />
                <br />
                <button type="submit">Create Stock</button>
            </form>
        </div>
    );
    } else {
        navigate("/login");
    }
}

export default CreateStock;