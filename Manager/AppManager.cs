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

        public async void sendRecoveryEmail(string nome, string email)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("pedroni.dev@gmail.com");
            message.To.Add(email);
            message.IsBodyHtml = true;
            message.Body =  this.getEmailBody().Replace("%nome%", nome);
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
    }
}
