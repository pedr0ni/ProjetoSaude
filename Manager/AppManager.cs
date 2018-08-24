using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using ProjetoSaude.Data;
using ProjetoSaude.Models;

namespace ProjetoSaude.Manager
{
    public class AppManager
    {
        private SmtpClient smtpClient;
        private readonly IDatabaseContext _context;

        public AppManager(IDatabaseContext context)
        {
            this._context = context;
            this.smtpClient = new SmtpClient("smtp.gmail.com");
            this.smtpClient.EnableSsl = true;
            this.smtpClient.Port = 587;
            this.smtpClient.Credentials = new NetworkCredential("matheus.pedroni2@gmail.com", "Mi48Zz1kx7");
        }

        public async void signIn(HttpContext http, IUser model, bool isPersistent)
        {
            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(model), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
          
            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public IDatabaseContext getDatabase()
        {
            return this._context;
        }

        public bool isLogged(HttpContext http)
        {
            return http.User.Identity.IsAuthenticated;
        }

        public async void signOut(HttpContext http)
        {
            await http.SignOutAsync();
        }

        public IUser getLoggedUser(HttpContext http)
        {
            int id = Int32.Parse((from c in http.User.Claims where c.Type == "Id" select c.Value).FirstOrDefault());
            return this._context.Users.Find(id);
        }

        private IEnumerable<Claim> GetUserClaims(IUser user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Nome));
            claims.Add(new Claim("Id", user.Id.ToString()));
            return claims;
        }

        public SmtpClient GetSmtpClient()
        {
            return this.smtpClient;
        }

        public async Task sendRecoveryEmail(IUser user)
        {
            IUserRecover recover = new IUserRecover
            {
                UserId = user.Id,
                Token = generateToken(8),
                Validated = false,
                Created = Timestamp
            };

            this._context.UsersRecover.Add(recover);
            await this._context.SaveChangesAsync();

            MailMessage message = new MailMessage();
            message.From = new MailAddress("pedroni.dev@gmail.com");
            message.To.Add(user.Email);
            message.IsBodyHtml = true;
            message.Body = this.getEmailBody().Replace("%nome%", user.Nome).Replace("%token%", recover.Token);
            message.Subject = "Recuperação de Conta - Projeto Saúde";
            await this.smtpClient.SendMailAsync(message);
        }

        private string getEmailBody()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader($"{Directory.GetCurrentDirectory()}{@"\wwwroot\Email.html"}");
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        private string generateToken(int length)
        {
            Random rnd = new Random();
            string keys = "abcdefghijklmnopqrstuvwxyz0123456789";
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += keys[rnd.Next(keys.Length)];
            }
            return result.ToUpper();
        }

        /// <summary>
        /// Get current time in milliseconds (Unix TimeStamp)
        /// </summary>
        /// <returns>long with the timestamp</returns>
        public long Timestamp {
            get
            {
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                return (long)timeSpan.TotalSeconds;
            }
        }

    }
}
