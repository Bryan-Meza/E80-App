from flask import Flask, render_template, jsonify
import random

app = Flask(__name__)

@app.route('/')
def index():
    """Home page with map visualization"""
    return render_template('index.html')

@app.route('/graphics')
def graphics():
    """Graphics/charts page"""
    return render_template('graphics.html')

# API endpoints for chart data (demo data)
@app.route('/api/sales')
def api_sales():
    """Sales data API endpoint"""
    return jsonify({
        'labels': ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        'datasets': [{
            'label': 'Sales',
            'data': [random.randint(10, 100) for _ in range(6)],
            'borderColor': '#ffd700',
            'backgroundColor': 'rgba(255, 215, 0, 0.1)'
        }]
    })

@app.route('/api/production')
def api_production():
    """Production data API endpoint"""
    return jsonify({
        'labels': ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        'datasets': [{
            'label': 'Production',
            'data': [random.randint(20, 80) for _ in range(6)],
            'borderColor': '#00ff00',
            'backgroundColor': 'rgba(0, 255, 0, 0.1)'
        }]
    })

@app.route('/api/efficiency')
def api_efficiency():
    """Efficiency data API endpoint"""
    return jsonify({
        'labels': ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        'datasets': [{
            'label': 'Efficiency %',
            'data': [random.randint(60, 95) for _ in range(6)],
            'borderColor': '#ff6b6b',
            'backgroundColor': 'rgba(255, 107, 107, 0.1)'
        }]
    })

@app.route('/api/quality')
def api_quality():
    """Quality control data API endpoint"""
    return jsonify({
        'labels': ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        'datasets': [{
            'label': 'Quality Score',
            'data': [random.randint(80, 100) for _ in range(6)],
            'borderColor': '#4ecdc4',
            'backgroundColor': 'rgba(78, 205, 196, 0.1)'
        }]
    })

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5000)
