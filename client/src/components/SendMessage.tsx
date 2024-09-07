/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useState } from 'react';
import axios from 'axios';

function SendMessage() {
    const [text, setText] = useState<string>('');
    const [order, setOrder] = useState<number>(0);

    const handleSubmit = async (e: any) => {
        e.preventDefault();
        try {
            await axios.post('http://localhost:5000/api/v1/Messages/CreateMessage', { text, order });
            setText('');
            setOrder(order + 1);
            console.log(text, order)
        } catch (error) {
            console.error('Error sending message:', error);
        }
    };

    return (
        <div >
            <h2 style={{display: 'flex', alignItems: 'center'}}>Send Message</h2>
            <form style={{display: 'flex', gap: '10px'}} onSubmit={handleSubmit}>
                <input
                    type="text"
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    placeholder="Message"
                    required
                />
                <button style={{width: '200px', height: '25px', background: 'gray', fontSize: '12px'}} type="submit">Send</button>
            </form>
        </div>
    );
}

export default SendMessage;
