import os

from flask import Flask, render_template, request

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

    # Index Page
    @app.route('/', methods=['GET', 'POST'])
    def index():
        return render_template("index.html")
    
    @app.route('/upload', methods=['POST'])
    def upload_files():
        files = request.files.getlist('files[]')
        
        print(files)
        
        html_content = '<html><body>'
        
        for file in files:
            if file.filename == '':
                continue
            
            # Read file content and write to HTML
            file_content = file.read().decode('utf-8')
            html_content += f'<h2>{file.filename}</h2>'
            html_content += f'<pre>{file_content}</pre><br>'
        
        html_content += '</body></html>'
        
        return html_content

    return app
