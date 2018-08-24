using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoSaude.Models;
using ProjetoSaude.Models.Forms;
using ProjetoSaude.Manager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ProjetoSaude.Data;

namespace ProjetoSaude.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {

        private readonly AppManager _appManager;

        public AccountController(AppManager appManager)
        {
            this._appManager = appManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (Request.Cookies["Type"] != null && Request.Cookies["Message"] != null)
            {
                ViewData["Type"] = Request.Cookies["Type"];
                ViewData["Message"] = Request.Cookies["Message"];
                Response.Cookies.Delete("Type");
                Response.Cookies.Delete("Message");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> InputLogin(IUser model, string ReturnUrl)
        {
            IUser result = this._appManager.getDatabase().Users.SingleOrDefault(u => u.Cpf == model.Cpf && u.Senha == new Cripto(model.Senha).Encrypted);

            if (result == null)
            {
                Response.Cookies.Append("Type", "danger");
                Response.Cookies.Append("Message", "Usuário ou senha incorreto(s).");
                return RedirectToAction("Login", "Account");
            }

            this._appManager.signIn(this.HttpContext, result, false);
            if (ReturnUrl != null) return Redirect(ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            Console.WriteLine("=== " + this._appManager.getDatabase().Users.ToList().Count);
            if (Request.Cookies["Type"] != null && Request.Cookies["Message"] != null)
            {
                ViewData["Type"] = Request.Cookies["Type"];
                ViewData["Message"] = Request.Cookies["Message"];
                Response.Cookies.Delete("Type");
                Response.Cookies.Delete("Message");
            }
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Append("Type", "warning");
            Response.Cookies.Append("Message", "Você saiu da aplicação.");
            this._appManager.signOut(this.HttpContext);
            return new RedirectToActionResult("Login", "Account", new { });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> InputRegistrar(IUser model)
        {

            model.Status = true;
            model.Perfil = "NORMAL";
            model.Senha = new Cripto(model.Senha).Encrypted;

            this._appManager.getDatabase().Users.Add(model);
            this._appManager.getDatabase().SaveChanges();

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InputRecover(IUserRecover user)
        {
            /*List<Dictionary<string, object>> rows = this._userDatabase.custom("SELECT nome FROM users WHERE cpf = '" + user.cpf + "' AND email = '" + user.email + "'", new string[] { "nome" });

            if (rows.Count > 0)
            {
                Response.Cookies.Append("Type", "success");
                Response.Cookies.Append("Message", "Enviamos um e-mail para " + user.email + " com instruções para recuperar a senha.");
                this._appManager.sendRecoveryEmail(rows.First()["nome"].ToString(), user.email);
            } else
            {
                Response.Cookies.Append("Type", "danger");
                Response.Cookies.Append("Message", "Não existe nenhuma conta com esse CPF e E-mail.");
            }
            */
            return RedirectToAction("Recover", "Account");
        }

        [AllowAnonymous]
        public IActionResult Recover()
        {
            if (Request.Cookies["Type"] != null && Request.Cookies["Message"] != null)
            {
                ViewData["Type"] = Request.Cookies["Type"];
                ViewData["Message"] = Request.Cookies["Message"];
                Response.Cookies.Delete("Type");
                Response.Cookies.Delete("Message");
            }
            return View();
        }

    }
}