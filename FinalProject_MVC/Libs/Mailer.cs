using System.Net.Http;

namespace FinalProject_MVC.Libs
{
    public class Mailer
    {
        public Mailer()
        {
        }

        public static bool SendEmail(string recipent, string subject, string body)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new System.Uri("http://localhost:5000");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            var response = httpClient.PostAsync("send_email",
                //form: reciepent, subject, message
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "recipient", recipent },
                    { "subject", subject },
                    { "message", body }
                }
                )).Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }     
    }
}
