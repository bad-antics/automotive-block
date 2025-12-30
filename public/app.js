// Automotive Block - Client Application

document.addEventListener('DOMContentLoaded', initApp);

async function initApp() {
  console.log('🚗 Automotive Block initializing...');
  await fetchStatus();
}

async function fetchStatus() {
  try {
    const response = await fetch('/api/status');
    const data = await response.json();
    
    const statusDiv = document.getElementById('status');
    if (statusDiv) {
      statusDiv.innerHTML = `
        <strong>Status:</strong> ${data.status}<br>
        <strong>Name:</strong> ${data.name}<br>
        <strong>Version:</strong> ${data.version}
      `;
    }
    
    console.log('✅ Connected to server');
  } catch (error) {
    console.error('❌ Connection error:', error);
    document.getElementById('status').innerHTML = 'Connection failed';
  }
}
