// CAN Bus Protocol Implementation
// Controller Area Network bus communication

class CANHandler {
  constructor() {
    this.buses = new Map();
    this.messages = [];
    this.filters = [];
  }

  initializeBus(busId, baudrate = 500000) {
    this.buses.set(busId, {
      id: busId,
      baudrate,
      status: 'initialized',
      messages_received: 0,
      messages_sent: 0
    });
    return { status: 'Bus initialized', bus: busId, baudrate };
  }

  sendMessage(busId, canId, data) {
    const message = {
      timestamp: Date.now(),
      bus: busId,
      canId: `0x${canId.toString(16)}`,
      data,
      dlc: data.length
    };
    this.messages.push(message);
    
    const bus = this.buses.get(busId);
    if (bus) bus.messages_sent++;
    
    return message;
  }

  receiveMessage(busId, canId) {
    const filtered = this.messages.filter(m => 
      m.bus === busId && m.canId === `0x${canId.toString(16)}`
    );
    
    const bus = this.buses.get(busId);
    if (bus) bus.messages_received = filtered.length;
    
    return filtered;
  }

  addFilter(busId, canId, mask) {
    this.filters.push({ busId, canId, mask });
    return { status: 'Filter added', busId, canId };
  }

  getBusStatus(busId) {
    return this.buses.get(busId) || null;
  }

  getAllMessages() {
    return this.messages.slice(-100); // Last 100 messages
  }

  clearMessages() {
    this.messages = [];
    return { status: 'Messages cleared' };
  }

  simulateCANData() {
    const simulated = {
      engine: {
        rpm: Math.floor(Math.random() * 8000),
        load: Math.floor(Math.random() * 100),
        temp: 80 + Math.floor(Math.random() * 40)
      },
      transmission: {
        gear: ['P', 'R', 'N', 'D'][Math.floor(Math.random() * 4)],
        speed: Math.floor(Math.random() * 200)
      },
      sensors: {
        throttle: Math.floor(Math.random() * 100),
        brake: Math.random() > 0.7,
        clutch: Math.random() > 0.8
      }
    };
    return simulated;
  }
}

module.exports = CANHandler;
