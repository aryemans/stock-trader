import React, { useState } from "react";
import Cookies from 'universal-cookie';
import { useNavigate } from "react-router-dom"

function MarketSchedule() {
    const cookies = new Cookies();
    const userRole = cookies.get('UserRole');
    const navigate = useNavigate();
    const [day, setDay] = useState("");
    const [startTime, setStartTime] = useState("");
    const [endTime, setEndTime] = useState("");
    const [isHoliday, setIsHoliday] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        const response = await fetch("/admin/change-schedule", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                dayOfWeek: day,
                startTime: startTime,
                endTime: endTime,
                isHoliday: isHoliday
            }),
        });
        const data = await response.json();
            if (response.ok) {
                alert('Successful!');
                navigate('/');
            } else {
                alert(data.message);
            }
        
    };
    if(userRole === "admin") {
    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label htmlFor="day">Day:</label>
                <input
                    type="text"
                    id="day"
                    value={day}
                    onChange={(e) => setDay(e.target.value)}
                />
            </div>
            <div>
                <label htmlFor="start-time">Start Time:</label>
                <input
                    type="text"
                    id="start-time"
                    value={startTime}
                    onChange={(e) => setStartTime(e.target.value)}
                />
            </div>
            <div>
                <label htmlFor="end-time">End Time:</label>
                <input
                    type="text"
                    id="end-time"
                    value={endTime}
                    onChange={(e) => setEndTime(e.target.value)}
                />
            </div>
            <div>
                <label htmlFor="holiday">Holiday:</label>
                <input
                    type="checkbox"
                    id="holiday"
                    checked={isHoliday}
                    onChange={(e) => setIsHoliday(e.target.checked)}
                />
            </div>
            <button type="submit">Update Schedule</button>
        </form>
    ); 
} else {
    navigate("/login");
}
}


export default MarketSchedule;
