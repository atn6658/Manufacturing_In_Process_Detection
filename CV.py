import os

from flask import Flask, render_template, request, url_for, send_from_directory

def create_app(test_config=None):
    # create and configure the app
    app = Flask(__name__, instance_relative_config=True)
    app.config.from_mapping(
        SECRET_KEY='dev',
        DATABASE=os.path.join(app.instance_path, 'flaskr.sqlite'),
    )

    if test_config is None:
        # load the instance config, if it exists, when not testing
        app.config.from_pyfile('config.py', silent=True)
    else:
        # load the test config if passed in
        app.config.from_mapping(test_config)

    # ensure the instance folder exists
    try:
        os.makedirs(app.instance_path)
    except OSError:
        pass
    
    UPLOAD_FOLDER = 'uploads'
    if not os.path.exists('FlaskServer/' + UPLOAD_FOLDER):
        os.makedirs('FlaskServer/' + UPLOAD_FOLDER)

    # Index Page
    @app.route('/', methods=['GET', 'POST'])
    def index():
        return render_template("index.html")
    
    @app.route('/upload', methods=['POST'])
    def upload_files():
        files = request.files.getlist('files[]')
        
        html_content = '<html><body>'
        
        for file in os.listdir('FlaskServer/' + UPLOAD_FOLDER):
            print(f"Removing {file}")
            os.remove(os.path.join('FlaskServer/' + UPLOAD_FOLDER, file))
        
        for file in files:
            if file.filename == '':
                continue
            
            # Optionally save the file
            file_path = os.path.join('FlaskServer/' + UPLOAD_FOLDER, file.filename)
            file.save(file_path)
            
            # Read file content and write to HTML
            file_content = file.read().decode('utf-8')
            html_content += f'<h2>{file.filename}</h2>'
            html_content += f'<pre>{url_for('download_file', filename=file.filename)}</pre><br>'
        
        html_content += '</body></html>'
        
        return html_content
    
    # Route to serve the files
    @app.route('/uploads/<filename>')
    def download_file(filename):
        print(f"Downloading {filename}")
        # Serve the file from the upload folder
        return send_from_directory(UPLOAD_FOLDER, filename)

    return app
