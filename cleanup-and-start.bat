@echo off
echo =========================================
echo  E80 Dashboard - Cleanup and Start
echo =========================================
echo.

echo Cleaning up old static files...
if exist "index.html" del "index.html"
if exist "graphic.html" del "graphic.html" 
if exist "header.html" del "header.html"
if exist "sidebar.html" del "sidebar.html"
if exist "footer.html" del "footer.html"
if exist "style.css" del "style.css"
if exist "script.js" del "script.js"
if exist "img" rmdir /s /q "img"

echo Old files cleaned up!
echo.

echo Installing Flask dependencies...
pip install -r requirements.txt
echo.

echo Starting Flask development server...
echo.
echo ========== READY! ==========
echo Dashboard: http://127.0.0.1:5000
echo Graphics:  http://127.0.0.1:5000/graphics
echo API Sales: http://127.0.0.1:5000/api/sales
echo ===========================
echo.
echo Press Ctrl+C to stop the server
echo.
python app.py
