@echo off
echo Starting Electtric E80 Flask Dashboard...
echo.
echo Installing/updating dependencies...
pip install -r requirements.txt
echo.
echo Starting Flask development server...
echo Dashboard will be available at: http://127.0.0.1:5000
echo Press Ctrl+C to stop the server
echo.
python app.py
