from __future__ import print_function
import os.path
import base64
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.utils import formataddr
from datetime import datetime
from jinja2 import Environment, FileSystemLoader



# If modifying these scopes, delete the file token.json.
SCOPES = ['https://www.googleapis.com/auth/gmail.readonly', 'https://www.googleapis.com/auth/gmail.send',
          'https://www.googleapis.com/auth/gmail.compose']


def send_email_v2(recipient, subject=None, content=None, port=0):
    """This code will authorize the client and call Gmail.Send API"""

    creds = None
    if os.path.exists('token.json'):
        creds = Credentials.from_authorized_user_file('token.json', SCOPES)

    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file('credentials_desktop_apps.json', SCOPES)
            creds = flow.run_local_server(port=port)  # https://dhpit.com/go/f5kizi

        # Save the credentials for the next run
        with open('token.json', 'w') as token:
            token.write(creds.to_json())

    # Call Gmail API
    try:
        # Call the Gmail API
        service = build('gmail', 'v1', credentials=creds)

        # Tải mẫu HTML từ file template.html
        file_loader = FileSystemLoader('.')
        env = Environment(loader=file_loader)
        template = env.get_template('template.html')

        # Prepare a message
        message = MIMEMultipart()
        message['To'] = recipient
        message['From'] = formataddr(('Camping App', service.users().getProfile(userId='me').execute()['emailAddress']))
        if subject is not None:
            message['Subject'] = str(subject)
        else:
            message['Subject'] = 'Test Message (' + str(datetime.now().strftime('%m/%d/%Y %H:%M:%S')) + ')'

        # Load the HTML template
        html_content = template.render(content=content)
        
        # Attach the HTML content to the message
        message.attach(MIMEText(html_content, 'html'))

        # Encoded message
        encoded_message = base64.urlsafe_b64encode(message.as_bytes()).decode()

        create_message = {
            'raw': encoded_message
        }

        send_message = service.users().messages().send(userId="me", body=create_message).execute()
        return True
    except HttpError as e:
        print(f'Error occurred: {e}')
        return False
