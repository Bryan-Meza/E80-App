Write-Host "Starting Electtric E80 Flask Dashboard..." -ForegroundColor Green
Write-Host ""
Write-Host "Installing/updating dependencies..." -ForegroundColor Yellow
pip install -r requirements.txt
Write-Host ""
Write-Host "Starting Flask development server..." -ForegroundColor Yellow
Write-Host "Dashboard will be available at: http://127.0.0.1:5000" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Cyan
Write-Host ""
python app.py
