using Core.Models;
using System.Net;
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BLL
{
    public class EmailSender
    {
        public static void SendMessage(AppUser entity, string subject, string bodyPart)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("sidkaty174@gmail.com");
            mailMessage.To.Add(entity.Email);

            mailMessage.Subject = subject;
            mailMessage.Body = bodyPart;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("sidkaty174@gmail.com", "kdwnklypguuisajz");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}
