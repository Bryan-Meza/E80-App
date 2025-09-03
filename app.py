from flask import Flask, render_template, jsonify, request
import random
import base64
import os
from datetime import datetime

app = Flask(__name__)

# Enable CORS for Unity requests
@app.after_request
def after_request(response):
    response.headers.add('Access-Control-Allow-Origin', '*')
    response.headers.add('Access-Control-Allow-Headers', 'Content-Type,Authorization')
    response.headers.add('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE,OPTIONS')
    return response

@app.route('/')
def index():
    """Home page with map visualization"""
    return render_template('index.html')

@app.route('/graphics')
def graphics():
    """Graphics/charts page"""
    return render_template('graphics.html')

@app.route('/unity')
def unity():
    """Unity screenshots page"""
    return render_template('unity.html')

@app.route('/api/upload', methods=['POST'])
def api_upload():
    """Receive screenshot from Unity"""
    print(f"Received request method: {request.method}")
    print(f"Content type: {request.content_type}")
    
    try:
        # Get JSON data from Unity
        data = request.get_json()
        
        print(f"Received data: {type(data)}")
        
        if not data or 'imageBase64' not in data:
            print("Error: No image data provided")
            return jsonify({'error': 'No image data provided'}), 400
        
        # Decode base64 image
        image_data = data['imageBase64']
        print(f"Image data length: {len(image_data)}")
        
        image_bytes = base64.b64decode(image_data)
        
        # Create screenshots directory if it doesn't exist
        screenshots_dir = os.path.join('static', 'screenshots')
        if not os.path.exists(screenshots_dir):
            os.makedirs(screenshots_dir)
        
        # Generate filename with timestamp
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S_%f")[:-3]  # milliseconds
        filename = f"screenshot_{timestamp}.jpg"
        filepath = os.path.join(screenshots_dir, filename)
        
        # Save the image
        with open(filepath, 'wb') as f:
            f.write(image_bytes)
        
        print(f"Screenshot saved: {filepath}")
        
        # Return success response
        return jsonify({
            'status': 'success',
            'message': 'Screenshot received and saved',
            'filename': filename,
            'timestamp': timestamp
        }), 200
        
    except Exception as e:
        print(f"Error processing screenshot: {str(e)}")
        return jsonify({'error': f'Failed to process image: {str(e)}'}), 500

@app.route('/api/screenshots')
def api_screenshots():
    """Get list of recent screenshots"""
    try:
        screenshots_dir = os.path.join('static', 'screenshots')
        if not os.path.exists(screenshots_dir):
            return jsonify({'screenshots': []})
        
        # Get all screenshot files
        files = []
        for filename in os.listdir(screenshots_dir):
            if filename.endswith('.jpg') and filename.startswith('screenshot_'):
                filepath = os.path.join(screenshots_dir, filename)
                timestamp = os.path.getctime(filepath)
                files.append({
                    'filename': filename,
                    'url': f'/static/screenshots/{filename}',
                    'timestamp': timestamp
                })
        
        # Sort by timestamp (newest first)
        files.sort(key=lambda x: x['timestamp'], reverse=True)
        
        # Return only the 10 most recent
        return jsonify({'screenshots': files[:10]})
        
    except Exception as e:
        return jsonify({'error': f'Failed to get screenshots: {str(e)}'}), 500

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
