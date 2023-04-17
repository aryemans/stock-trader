import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import Cookies from 'universal-cookie';
import './Portfolio.css';

const Portfolio = () => {
    const [userStockData, setUserStockData] = useState([]);
    const [userCashData, setUserCashData] = useState();
    const [length, setLength] = useState(0);
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const navigate = useNavigate();

    useEffect(() => {
        // Fetch market data from backend API
        const fetchAssetsData = async () => {
            const response = await fetch("/users/view-assets", {
                method: 'GET',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            const data = await response.json();
            if (response.ok) {
                setUserStockData(data.stocks);
                setUserCashData(data.cash);
                setLength(Object.keys(userStockData).length);
            } else {
                navigate("/login");
                //alert(data.message);
            }
        };
        fetchAssetsData();
        
    },[userStockData]);
    
    if(userRole === "user") {
    return (
        <div>
            <h2>My Portfolio</h2>
                    { length > 0  ? (
                    <div> 
                        <div>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>Ticker</th>
                                            <th>Quantity</th>
                                            <th>Sell</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    {Object.entries(userStockData).map(([key, value]) => (
                                        <tr key={key}>
                                            <td>{key}</td>
                                            <td>{value}</td>
                                            <td><Link className = "link-class" to={{ pathname: `/sell-stock/${key}`}}>Sell</Link></td>
                                        </tr>
                                        ))}
                                    </tbody>
                                </table>
                            
                            </div>
                            <table>
                                    <thead>
                                        <tr>
                                            <th>Cash</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>{userCashData}</td>
                                        </tr>
                                    </tbody>
                            </table>
                        </div>
                           ) : (
                            <div>
                                <p> You currently have no stocks! </p>
                                </div>
                           ) }
        </div>
    );
    } else {
        navigate("/login");
    }

}

export default Portfolio;
