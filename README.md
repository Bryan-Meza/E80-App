# Electtric E80 Dashboard - Flask Application

A Flask-based dashboard for the Electtric E80 project with real-time data visualization, interactive charts, and a modern sidebar navigation with FontAwesome icons.

## Features

- 🏠 **Interactive Dashboard** with map visualization
- 📊 **Real-time Charts** for sales, production, efficiency, and quality metrics
- 🎨 **Modern UI** with FontAwesome icons and active link highlighting
- 🖼️ **Logo Integration** in the header
- 📱 **Responsive Design** that works on all devices
- 🔗 **RESTful API** endpoints for dynamic data
- ⚡ **Flask Hot Reload** for development

## Quick Start

### Option 1: Automated Cleanup & Start
```cmd
cleanup-and-start.bat
```
This will:
- Remove old static HTML files
- Install Flask dependencies  
- Start the development server
- Show all available URLs

### Option 2: Manual Start
```powershell
pip install -r requirements.txt
python app.py
```

Then open: **http://127.0.0.1:5000**

## Project Structure

```
E80-App/
├── app.py                 # Main Flask application
├── config.py             # Configuration settings
├── requirements.txt      # Python dependencies
├── templates/           # Jinja2 templates
│   ├── base.html        # Base template with header/sidebar/footer
│   ├── index.html       # Dashboard home page
│   └── graphics.html    # Charts and analytics page
├── static/             # Static assets
│   ├── css/
│   │   └── style.css   # Main stylesheet
│   ├── js/
│   │   └── script.js   # Frontend JavaScript
│   └── img/
│       └── logo.png    # Logo image
└── README.md           # This file
```

## Installation

1. **Create a virtual environment** (recommended):
   ```powershell
   python -m venv venv
   venv\Scripts\Activate.ps1
   ```

2. **Install dependencies**:
   ```powershell
   pip install -r requirements.txt
   ```

## Running the Application

### Development Mode

```powershell
python app.py
```

The application will be available at: http://localhost:5000

### Production Mode

For production deployment, use a WSGI server like Gunicorn:

```powershell
pip install gunicorn
gunicorn -w 4 -b 0.0.0.0:5000 app:app
```

## API Endpoints

- `GET /` - Dashboard home page
- `GET /graphics` - Charts and analytics page
- `GET /api/sales` - Sales data JSON
- `GET /api/production` - Production data JSON
- `GET /api/efficiency` - Efficiency metrics JSON
- `GET /api/quality` - Quality control data JSON

## Development

### Adding New Pages

1. Create a new route in `app.py`
2. Create a corresponding template in `templates/`
3. Add navigation link to `templates/base.html`

### Adding New API Endpoints

1. Add new route function in `app.py`
2. Return data in JSON format using `jsonify()`
3. Update frontend JavaScript to consume the new endpoint

## Configuration

- Development settings are in `config.py`
- Set `FLASK_ENV=production` for production deployment
- Update `SECRET_KEY` for production use

## Migration Notes

This Flask application was migrated from a static HTML/CSS/JS project. Key changes:

- HTML files converted to Jinja2 templates with template inheritance
- Static files moved to Flask's `static/` directory structure  
- JavaScript updated to work with Flask URL routing
- Added real API endpoints for chart data
- Improved code organization and maintainability
