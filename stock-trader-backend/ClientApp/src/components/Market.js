import React, { useState, useEffect } from "react";
import {Link} from "react-router-dom";
import Cookies from 'universal-cookie';

const Market = () => {
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');

    const [marketData, setMarketData] = useState([]);
    useEffect(() => {
        // Fetch market data from backend API
        const fetchMarketData = async () => {
            const response = await fetch("/market/stock-market");
            const data = await response.json();
            if (response.ok) {
                setMarketData(data);
            } else {
                alert(data.message);
            }
        };
        fetchMarketData();
    }, []);

    return (
        <div>
            <h2>Market Page</h2>
            <table>
                <thead>
                    <tr>

                        <th>Stock Ticker</th>
                        <th>Company Name</th>
                        <th>Price</th>
                        <th>Volume</th>
                        <th>Market Capitalization</th>
                        <th>Opening Price</th>
                        <th>High</th>
                        <th>Low</th>
                    </tr>
                </thead>
                <tbody>
                    {marketData.map((stock) => (
                        <tr key={stock.stock_ticker}>
                          {userRole === "user" && <td><Link to={{ pathname: `/buy-stock/${stock.stock_ticker}`}}>{stock.stock_ticker}</Link></td> }
                          {userRole === "admin" && <td>{stock.stock_ticker}</td> }
                          <td>{stock.companyName}</td>
                            <td>{stock.current_price}</td>
                            <td>{stock.volume}</td>
                            <td>{stock.market_capitalization}</td>
                            <td>{stock.opening_price}</td>
                            <td>{stock.high}</td>
                            <td>{stock.low}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default Market;
