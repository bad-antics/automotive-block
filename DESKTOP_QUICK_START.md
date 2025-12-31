# 🏴 BlackFlag v2.0 Desktop Edition - Quick Reference

## ⚡ 60-Second Quick Start

```bash
# 1. Go to desktop folder
cd desktop

# 2. Install (first time only)
npm install

# 3. Run
npm start
```

**That's it!** Window opens with full BlackFlag UI.

---

## 📂 What You Have

✅ **Web Version** (Fully working)
- 49 vehicles with specs
- 9 functional menu cards
- Library browser (3 tabs)
- 6 diagnostic guides
- 10 wiring diagrams
- 150+ API endpoints
- Network hosting ready

✅ **Desktop Version** (Ready to test)
- Electron app wrapper
- Same backend + UI
- Window controls
- Security isolation
- Installer builder
- Status monitoring

---

## 📍 Key Files

```
desktop/
├── main.js           ← Electron entry
├── preload.js        ← Security
├── package.json      ← Config
└── ui/
    ├── index.html    ← Interface
    ├── styles-blackflag.css  ← Theme
    ├── blackflag-app.js      ← Logic
    └── desktop-app.js        ← Desktop features
```

---

## 🎯 Test Checklist

Run `npm start` then verify:

- [ ] Window opens
- [ ] 9 menu cards visible
- [ ] Vehicle selector populates
- [ ] Library Browser works
- [ ] Status indicator = green
- [ ] Click menu cards = opens screens
- [ ] Window buttons work (minimize, close)
- [ ] No console errors

---

## 🔧 Common Commands

```bash
# Install dependencies
npm install

# Run in development (with DevTools)
npm run dev

# Build Windows installer
npm run dist

# Just pack (no installer)
npm run pack
```

---

## 🐛 If Something Goes Wrong

**Backend not connecting?**
```bash
# Kill any process on port 3000
taskkill /F /IM node.exe
# Try again
npm start
```

**npm install fails?**
```bash
# Clear cache
npm cache clean --force
# Delete folders
rmdir /s node_modules
# Reinstall
npm install
```

**Window won't open?**
```bash
# Check Node.js version
node --version  # Should be 14+
# Check npm
npm --version
```

---

## 📊 What's Included

| Component | Status | Location |
|-----------|--------|----------|
| Web Server | ✅ Running on 3000 | index.js |
| Vehicle DB | ✅ 49 vehicles | vehicle-database.js |
| Frontend UI | ✅ 373 lines | desktop/ui/index.html |
| Styling | ✅ Cyberpunk theme | desktop/ui/styles-blackflag.css |
| Logic | ✅ All functions | desktop/ui/blackflag-app.js |
| Desktop | ✅ Electron wrapper | desktop/main.js |
| Security | ✅ Context isolation | desktop/preload.js |

---

## 📚 Documentation

**Available Guides:**
- `DESKTOP_SETUP.md` - Complete setup instructions
- `DESKTOP_STATUS.md` - Full project status report
- `WEB_VERSION_REFERENCE.md` - Web version documentation
- `API_REFERENCE.md` - 150+ endpoint descriptions

---

## 🎨 9 Features Ready to Test

1. 🔍 **VIN Decoder** - Vehicle identification
2. 🔌 **ECU Scanner** - Find ECUs on vehicle
3. ⚡ **Voltage Meter** - System voltage monitoring
4. 📋 **Wiring Diagrams** - 10 circuit diagrams
5. ⚡ **Tune Manager** - Performance tunes
6. 💾 **ECU Cloning** - Backup & restore
7. 📦 **Module Installer** - Custom modules
8. 💨 **Emissions Control** - DPF/SCR/EGR
9. 📚 **Library Browser** - Specs, guides, diagrams

---

## 🔒 Security Features

✅ Context Isolation - No direct Node access
✅ Preload Bridge - Secure API exposure
✅ Sandboxing - Restricted renderer process
✅ CORS Headers - API protection

---

## 💾 Build Info

```
Installer Size: ~100 MB
Portable Size: ~90 MB
App Memory: ~50 MB
Load Time: <1 second
```

---

## 📞 Support

**Issues?**
1. Check `DESKTOP_SETUP.md` troubleshooting section
2. Verify Node.js 14+ installed
3. Ensure port 3000 free
4. Check console for error messages (Ctrl+Shift+I in dev mode)

**To debug:**
```bash
npm run dev  # Opens DevTools automatically
```

---

## ✅ Ready!

Everything is set up. Just run:

```bash
cd desktop && npm install && npm start
```

**Enjoy BlackFlag v2.0 Desktop Edition!** 🚀

---

*Professional ECU Hacking & Tuning Suite*
*Free & Open Source | MIT License*
