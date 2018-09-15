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
        private readonly IDatabaseContext _context;

        public AccountController(IDatabaseContext context, AppManager appManager)
        {
            this._context = context;
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
        public async Task<IActionResult> LoginPost(IUser model)
        {
            IUser result = this._context.Users.SingleOrDefault(u => u.Cpf == model.Cpf && u.Senha == new Cripto(model.Senha).Encrypted);

            if (result != null)
            {
                await this._appManager.signIn(this.HttpContext, result, false);
            }

            return Ok(new { Authenticated = result != null, Message = result != null ? "Usuário logado com sucesso." : "Usuário ou senha incorreto(s)." });
        }

        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult Logout()
        {
            this._appManager.signOut(this.HttpContext);
            return new RedirectToActionResult("Index", "Home", new { });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPost(IUser model)
        {
            IUser Check = this._context.Users.FirstOrDefault(u => u.Cpf == model.Cpf || u.Email == model.Email);
            if (Check != null)
            {
                return Ok(new { Success = false, Message = "Já existe um usuário com este CPF ou E-mail." });
            }
            model.Status = true;
            model.Perfil = "NORMAL";
            model.Senha = new Cripto(model.Senha).Encrypted;

            this._context.Users.Add(model);
            await this._context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Usuário " + model.Nome + " registrado com sucesso." });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InputRecover(FUserRecover user)
        {
            IUser result = this._context.Users.SingleOrDefault(u => u.Cpf == user.Cpf && u.Email == user.Email);
            if (result != null)
            {
                this._appManager.addAlert("success", "Enviamos um e-mail para " + result.Email + " com instruções para recuperar a senha.", this.HttpContext);
                await this._appManager.sendRecoveryEmail(result, this._context);
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
            IUser user = this._context.Users.SingleOrDefault(u => u.Cpf == validateToken.Cpf);

            if (user == null)
            {
                this._appManager.addAlert("danger", "O CPF inserido não existe.", this.HttpContext);
                return RedirectToAction("Validate", "Account");
            }

            IUserRecover recover = this._context.UsersRecover.SingleOrDefault(r => r.Token == validateToken.Token && r.UserId == user.Id);

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

            if (this._appManager.Timestamp - recover.Created > 86400) // 86400 = 1 dia
            {
                this._appManager.addAlert("danger", "Este Token já expirou faz mais de 1 dia. Envie um e-mail novamente para recupearar sua senha.", this.HttpContext);
                return RedirectToAction("Validate", "Account");
            }

            user.Senha = new Cripto(validateToken.NovaSenha).Encrypted;
            recover.Validated = true;

            this._context.UsersRecover.Update(recover);
            this._context.Users.Update(user);
            this._appManager.addAlert("success", "Sua senha foi alterada com sucesso. ", this.HttpContext);
            await this._context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Edit(int id)
        {
            return View(this._context.Users.Find(id));
        }

    }
}