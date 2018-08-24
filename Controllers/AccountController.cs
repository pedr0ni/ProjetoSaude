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
        public async Task<IActionResult> InputRecover(FUserRecover user)
        {
            IUser result = this._appManager.getDatabase().Users.SingleOrDefault(u => u.Cpf == user.Cpf && u.Email == user.Email);
            if (result != null)
            {
                Response.Cookies.Append("Type", "success");
                Response.Cookies.Append("Message", "Enviamos um e-mail para " + result.Email + " com instruções para recuperar a senha.");
                await this._appManager.sendRecoveryEmail(result);
            } else
            {
                Response.Cookies.Append("Type", "danger");
                Response.Cookies.Append("Message", "Não existe nenhuma conta com esse CPF e E-mail.");
            }
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

        [AllowAnonymous]
        public IActionResult Validate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> InputValidate(FValidateToken validateToken)
        {
            IUser user = this._appManager.getDatabase().Users.SingleOrDefault(u => u.Cpf == validateToken.Cpf);

            if (user == null)
            {
                Response.Cookies.Append("Type", "danger");
                Response.Cookies.Append("Message", "O CPF inserido não existe.");
                return RedirectToAction("Validate", "Account");
            }

            IUserRecover recover = this._appManager.getDatabase().UsersRecover.SingleOrDefault(r => r.Token == validateToken.Token && r.UserId == user.Id);

            if (recover == null)
            {
                Response.Cookies.Append("Type", "danger");
                Response.Cookies.Append("Message", "O Token inserido está incorreto.");
                return RedirectToAction("Recover", "Account");
            }

            if (recover.Validated)
            {
                Response.Cookies.Append("Type", "warning");
                Response.Cookies.Append("Message", "O Token inserido já foi validado.");
                return RedirectToAction("Recover", "Account");
            }

            if (new Cripto(validateToken.NovaSenha).Encrypted == user.Senha)
            {
                Response.Cookies.Append("Type", "danger");
                Response.Cookies.Append("Message", "Você não pode utilizar uma senha igual a antiga.");
                return RedirectToAction("Validate", "Account");
            }

            user.Senha = new Cripto(validateToken.NovaSenha).Encrypted;
            recover.Validated = true;

            this._appManager.getDatabase().UsersRecover.Update(recover);
            this._appManager.getDatabase().Users.Update(user);
            await this._appManager.getDatabase().SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

    }
}