from flask import Flask, render_template, jsonify, request
import random
import base64
import os
from datetime import datetime

app = Flask(__name__)

# Variable global para stream en vivo (sin archivos)
live_frame = None
live_timestamp = None

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
    """Receive screenshot from Unity - ULTRA FAST MODE"""
    try:
        # Get JSON data from Unity (sin logs para máxima velocidad)
        data = request.get_json()
        
        if not data or 'imageBase64' not in data:
            return jsonify({'error': 'No data'}), 400
        
        # Decode base64 image
        image_bytes = base64.b64decode(data['imageBase64'])
        
        # Create screenshots directory if it doesn't exist
        screenshots_dir = os.path.join('static', 'screenshots')
        if not os.path.exists(screenshots_dir):
            os.makedirs(screenshots_dir)
        
        # Nombre de archivo ultra simple para máxima velocidad
        timestamp = datetime.now().strftime("%H%M%S%f")  # Solo hora para velocidad
        filename = f"f_{timestamp}.jpg"
        filepath = os.path.join(screenshots_dir, filename)
        
        # Save the image (sin logs)
        with open(filepath, 'wb') as f:
            f.write(image_bytes)
        
        # Limpiar archivos cada 200 requests para máxima velocidad
        import random
        if random.randint(1, 200) == 1:
            cleanup_old_screenshots(screenshots_dir, max_files=20)  # Solo 20 archivos max
        
        # Respuesta mínima para máxima velocidad
        return jsonify({'ok': 1}), 200
        
    except:
        return jsonify({'error': 'fail'}), 500

@app.route('/api/live', methods=['POST'])
def api_live():
    """Receive live stream - NO FILES, PURE SPEED"""
    global live_frame, live_timestamp
    
    try:
        data = request.get_json()
        if data and 'imageBase64' in data:
            # Solo guardar en memoria - SIN ARCHIVOS
            live_frame = data['imageBase64']
            live_timestamp = datetime.now().timestamp()
            return '{"ok":1}', 200  # Respuesta mínima
    except:
        pass
    
    return '{"error":1}', 400

@app.route('/api/live-frame')
def api_live_frame():
    """Get current live frame from memory"""
    global live_frame, live_timestamp
    
    if live_frame and live_timestamp:
        # Verificar que el frame sea reciente (menos de 1 segundo)
        if (datetime.now().timestamp() - live_timestamp) < 1:
            return jsonify({
                'frame': live_frame,
                'timestamp': live_timestamp,
                'live': True
            })
    
    return jsonify({'live': False})

def cleanup_old_screenshots(screenshots_dir, max_files=20):
    """Mantiene solo los archivos más recientes - ULTRA FAST"""
    try:
        files = []
        for filename in os.listdir(screenshots_dir):
            if filename.endswith('.jpg'):
                filepath = os.path.join(screenshots_dir, filename)
                files.append((filepath, os.path.getctime(filepath)))
        
        # Ordenar y mantener solo los más recientes (menos archivos = más velocidad)
        files.sort(key=lambda x: x[1], reverse=True)
        
        # Eliminar archivos antiguos rápidamente
        for filepath, _ in files[max_files:]:
            try:
                os.remove(filepath)
            except:
                pass
                
    except:
        pass  # Ignorar todos los errores para máxima velocidad

@app.route('/api/screenshots')
def api_screenshots():
    """Get list of recent screenshots - ULTRA FAST MODE"""
    try:
        screenshots_dir = os.path.join('static', 'screenshots')
        if not os.path.exists(screenshots_dir):
            return jsonify({'screenshots': []})
        
        # Solo obtener archivos rápidamente
        files = []
        for filename in os.listdir(screenshots_dir):
            if filename.endswith('.jpg'):
                files.append({
                    'filename': filename,
                    'url': f'/static/screenshots/{filename}',
                    'timestamp': os.path.getctime(os.path.join(screenshots_dir, filename))
                })
        
        # Sort y solo retornar los 3 más recientes para máxima velocidad
        files.sort(key=lambda x: x['timestamp'], reverse=True)
        
        return jsonify({'screenshots': files[:3]})  # Solo 3 para velocidad extrema
        
    except:
        return jsonify({'screenshots': []})

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
