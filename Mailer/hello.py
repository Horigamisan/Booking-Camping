import os
from flask import Flask, request, jsonify
from send_email import send_email_v1
from send_email_v2 import send_email_v2
from datetime import datetime
from flask import Flask, render_template, session, redirect, url_for, flash
from flask_bootstrap import Bootstrap4
from flask_moment import Moment
from flask_wtf import FlaskForm
from wtforms import StringField, PasswordField, SelectField, BooleanField, SubmitField
from wtforms.fields import DateField, EmailField
from wtforms.validators import DataRequired, Email, EqualTo, InputRequired
from flask_sqlalchemy import SQLAlchemy
from datetime import date 

basedir = os.path.abspath(os.path.dirname(__file__))
app = Flask(__name__)
bootstrap = Bootstrap4(app)
moment = Moment(app)
app.config['SECRET_KEY'] = 'h@rd t0 guess String'

class FormEmail(FlaskForm):
    recipient = EmailField('Recipient:', validators=[DataRequired()])
    subject = StringField('Subject:', validators=[DataRequired()])
    message = StringField('Message:', validators=[DataRequired()])

@app.route('/send_email', methods=['POST'])
def send_email():
    #get form form data
    recipient = request.form['recipient']
    subject = request.form['subject']
    message = request.form['message']
    #send email
    if send_email_v2(recipient, subject, message):
        return jsonify({'status': 'success'})
    else:
        return jsonify({'status': 'error'})

@app.errorhandler(404)
def page_not_found(e):
    return render_template('404.html'), 404

@app.errorhandler(500)
def internal_server_error(e):
    return render_template('500.html'), 500

