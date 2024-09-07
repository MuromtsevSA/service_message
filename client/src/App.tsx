import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import SendMessage from './components/SendMessage';
import LiveMessages from './components/LiveMessages';
import MessageHistory from './components/MessageHistory';

function HomePage() {
  return (
    <div style={{display: "grid", justifyContent: 'center', alignItems: 'center', width: '100%'}}>
      <h2 style={{display: 'flex', alignItems: 'center'}}>Welcome to the Messaging App</h2>
      <div style={{ marginBottom: '20px' }}>
        <SendMessage />
      </div>
      <div style={{ marginBottom: '20px' , display: 'flex', gap: '25px'}}>
        <LiveMessages />
        <div>
        <MessageHistory />
      </div>
      </div>

    </div>
  );
}

function App() {
  return (
      <Router>
          <div style={{ display: 'flex', alignItems: 'center', width: '100%', justifyContent: 'center' }}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/send" element={<SendMessage />} />
          <Route path="/live" element={<LiveMessages />} />
          <Route path="/history" element={<MessageHistory />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
