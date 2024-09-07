import React, { useEffect, useState } from 'react';
import axios from 'axios';

function MessageHistory() {
    const [messages, setMessages] = useState([]);

    useEffect(() => {
        const fetchMessages = async () => {
            try {
                const response = await axios.get('http://localhost:5000/api/v1/Messages/GetRecentMessages');
                console.log('Fetched messages:', response.data);
                setMessages(response.data);
            } catch (error) {
                console.error('Error fetching recent messages:', error);
            }
        };

        fetchMessages();
        const intervalId = setInterval(fetchMessages, 60000);

        return () => clearInterval(intervalId);
    }, []);


    return (
        <div>
            <h2>Recent Message History (Last 10 Minutes)</h2>
            <ul>
                {messages && messages.length > 0 ? (
                    messages.map(message => (
                        <li key={message.id}>
                            <strong>{new Date(message.timestamp).toLocaleString()}</strong>: {message.text}
                        </li>
                    ))
                ) : (
                    <p>No messages available</p>
                )}
            </ul>
        </div>
    );
}

export default MessageHistory;
