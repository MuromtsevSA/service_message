import React, { useEffect, useState } from 'react';

const LiveMessages = () => {
    const [messages, setMessages] = useState([]);
    const [socket, setSocket] = useState(null);

    useEffect(() => {
        const ws = new WebSocket('ws://localhost:5000/ws');

        ws.onopen = () => {
            console.log('WebSocket connected');
        };

        ws.onmessage = (event) => {
            try {
                const message = JSON.parse(event.data);
                setMessages((prevMessages) => [
                    ...prevMessages,
                    {
                        text: message.text,
                        order: message.order,
                        time: new Date(message.time),
                    },
                ]);
            } catch (e) {
                console.error('Failed to parse WebSocket message:', e);
            }
        };

        ws.onerror = (error) => {
            console.error('WebSocket error:', error);
        };

        ws.onclose = (event) => {
            console.log('WebSocket closed:', event.reason);
        };

        setSocket(ws);

        return () => {
            ws.close();
        };
    }, []);

    return (
        <div>
            <h2>Live Messages</h2>
            <ul>
                {messages.map((msg, index) => (
                    <li key={index}>
                        <strong>Order: {msg.order}</strong><br />
                        <span>{msg.text}</span><br />
                        <small>{msg.time.toLocaleTimeString()}</small>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default LiveMessages;
