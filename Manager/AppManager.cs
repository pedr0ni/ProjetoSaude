using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
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

        public AppManager()
        {

            /*
             * Configuração para SMTP (Envio de E-mails)
             */
            this.smtpClient = new SmtpClient("smtp.gmail.com");
            this.smtpClient.EnableSsl = true;
            this.smtpClient.Port = 587;
            this.smtpClient.Credentials = new NetworkCredential("contas.projetosaude@gmail.com", "15cbdj6ksq"); // Senha gerada random (15cbdj6ksq)
        }

        /// <summary>
        /// Adiciona o usuário logado na sessão
        /// </summary>
        /// <param name="http"></param>
        /// <param name="model"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task signIn(HttpContext http, IUser model, bool isPersistent)
        {
            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(model), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public bool isLogged(HttpContext http)
        {
            return http.User.Identity.IsAuthenticated;
        }

        public async void signOut(HttpContext http)
        {
            await http.SignOutAsync();
        }

        /// <summary>
        /// Pega o usuário logado pelo Id
        /// </summary>
        /// <param name="http"></param>
        /// <param name="context"></param>
        /// <returns>IUser</returns>
        public IUser getLoggedUser(HttpContext http, IDatabaseContext context)
        {
            if (!this.isLogged(http)) return null;
            int id = Int32.Parse(http.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return context.Users.Find(id);
        }

        /// <summary>
        /// Pega as informações de Sessão
        /// </summary>
        /// <param name="user"></param>
        /// <returns>IEnumerable of Claim</returns>
        private IEnumerable<Claim> GetUserClaims(IUser user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Nome));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            return claims;
        }

        public SmtpClient GetSmtpClient()
        {
            return this.smtpClient;
        }

        /// <summary>
        /// Envia um E-mail para o Usuário com instruções para recuperar a senha
        /// </summary>
        /// <param name="user">IUser</param>
        /// <param name="context">IDatabaseContext</param>
        /// <returns>async Task</returns>
        public async Task sendRecoveryEmail(IUser user, IDatabaseContext context)
        {
            IUserRecover recover = new IUserRecover
            {
                UserId = user.Id,
                Token = generateToken(8),
                Validated = false,
                Created = Timestamp
            };

            context.UsersRecover.Add(recover);
            await context.SaveChangesAsync();

            new Thread(async () =>
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("accounts@projetosaude.com.br");
                message.To.Add(user.Email);
                message.IsBodyHtml = true;
                message.Body = this.getEmailBody().Replace("%nome%", user.Nome).Replace("%token%", recover.Token);
                message.Subject = "Recuperação de Conta - Projeto Saúde";
                await this.smtpClient.SendMailAsync(message);
            }).Start();
            
        }

        /// <summary>
        /// Lê o HTML do corpo do e-mail
        /// </summary>
        /// <returns>string com o corpo do email</returns>
        private string getEmailBody()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader($"{Directory.GetCurrentDirectory()}{@"/wwwroot/Email.html"}");
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

        public void addAlert(string Type, string Message, HttpContext http)
        {
            http.Response.Cookies.Append("Type", Type);
            http.Response.Cookies.Append("Message", Message);
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
