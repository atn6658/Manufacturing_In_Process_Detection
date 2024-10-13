import os

from flask import Flask, render_template, request, redirect, url_for, session, send_file

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
    
    UPLOAD_FOLDER = 'uploads/'
    if not os.path.exists('FlaskServer/' + UPLOAD_FOLDER):
        os.makedirs('FlaskServer/' + UPLOAD_FOLDER)

    # Index Page
    @app.route('/', methods=['GET', 'POST'])
    def index():
        files = session.get('files', [])
        return render_template("index.html", files=files)
    
    @app.route('/upload', methods=['POST'])
    def upload_files():
        files = request.files.getlist('files[]')
        file_names = []
        
        for file in os.listdir('FlaskServer/' + UPLOAD_FOLDER):
            print(f"Removing {file}")
            os.remove(os.path.join('FlaskServer/' + UPLOAD_FOLDER, file))
        
        for file in files:
            if file.filename == '':
                continue
            
            file_path = os.path.join('FlaskServer/' + UPLOAD_FOLDER, file.filename)
            file.save(file_path)
            
            file_names.append(file.filename)
            
        session['files'] = file_names
        
        return redirect(url_for('index'))
    
    @app.route('/jankfiles', methods=['POST'])
    def jank_files():
        request_data = request.get_json()
        
        # Write to filedata.txt in FlaskServer/jank from file in FlaskServer/uploads
        file_path = os.path.join('FlaskServer/' + UPLOAD_FOLDER, request_data['filename'])
        
        with open(file_path, 'r') as file:
            file_content = file.read()
            
        with open('FlaskServer/jank/filedata.txt', 'w') as file:
            file.write(request_data['filename'] + '\n')
            file.write(file_content)
            
        return "Success"
    
    @app.route('/jankfileselection', methods=['POST', 'GET'])
    def jank_file_selection():
        # read filedata.txt from FlaskServer/jank
        file_path = os.path.join('jank', "filedata.txt")
        # with open(file_path, 'r') as file:
        #     file_content = file.read()
            
        return send_file(file_path, as_attachment=False)

    return app
