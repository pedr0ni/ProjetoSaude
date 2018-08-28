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
                this._appManager.addAlert("danger", "Usuário ou senha incorreto(s).", this.HttpContext);
                return RedirectToAction("Login", "Account");
            }

            await this._appManager.signIn(this.HttpContext, result, false);
            if (ReturnUrl != null) return Redirect(ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult Logout()
        {
            this._appManager.addAlert("info", "Você saiu da aplicação.", this.HttpContext);
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
            await this._appManager.getDatabase().SaveChangesAsync();

            this._appManager.addAlert("info", "Sua conta foi criada com sucesso. Obrigado :)", this.HttpContext);

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InputRecover(FUserRecover user)
        {
            IUser result = this._appManager.getDatabase().Users.SingleOrDefault(u => u.Cpf == user.Cpf && u.Email == user.Email);
            if (result != null)
            {
                this._appManager.addAlert("success", "Enviamos um e-mail para " + result.Email + " com instruções para recuperar a senha.", this.HttpContext);
                await this._appManager.sendRecoveryEmail(result);
            } else
            {
                this._appManager.addAlert("danger", "Não existe nenhuma conta com esse CPF e E-mail.", this.HttpContext);
            }
            return RedirectToAction("Recover", "Account");
        }

        [AllowAnonymous]
        public IActionResult Recover()
        {
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
                this._appManager.addAlert("danger", "O CPF inserido não existe.", this.HttpContext);
                return RedirectToAction("Validate", "Account");
            }

            IUserRecover recover = this._appManager.getDatabase().UsersRecover.SingleOrDefault(r => r.Token == validateToken.Token && r.UserId == user.Id);

            if (recover == null)
            {
                this._appManager.addAlert("danger", "O Token inserido está incorreto.", this.HttpContext);
                return RedirectToAction("Recover", "Account");
            }

            if (recover.Validated)
            {
                this._appManager.addAlert("warning", "O Token inserido já foi validado.", this.HttpContext);
                return RedirectToAction("Recover", "Account");
            }

            if (new Cripto(validateToken.NovaSenha).Encrypted == user.Senha)
            {
                this._appManager.addAlert("danger", "Você não pode utilizar uma senha igual a antiga.", this.HttpContext);
                return RedirectToAction("Validate", "Account");
            }

            user.Senha = new Cripto(validateToken.NovaSenha).Encrypted;
            recover.Validated = true;

            this._appManager.getDatabase().UsersRecover.Update(recover);
            this._appManager.getDatabase().Users.Update(user);
            await this._appManager.getDatabase().SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Edit(int id)
        {
            return View(this._appManager.getDatabase().Users.Find(id));
        }

    }
}