// Automotive Block - ECU Diagnostic Tool
// Main entry point

const express = require('express');
const path = require('path');

const app = express();
const PORT = 3000;

// Middleware
app.use(express.json());
app.use(express.static('public'));

// Routes
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'public/index.html'));
});

app.get('/api/status', (req, res) => {
  res.json({
    status: 'running',
    version: '1.0.0',
    name: 'Automotive Block'
  });
});

// Start server
app.listen(PORT, () => {
  console.log(`Automotive Block running on http://localhost:${PORT}`);
});
