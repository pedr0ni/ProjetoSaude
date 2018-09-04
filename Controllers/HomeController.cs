using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetoSaude.Models;
using ProjetoSaude.Manager;
using Microsoft.AspNetCore.Authorization;
using ProjetoSaude.Data;

namespace ProjetoSaude.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private AppManager _appManager;
        private readonly IDatabaseContext _context;
            
        public HomeController(IDatabaseContext context, AppManager appManager)
        {
            this._context = context;
            this._appManager = appManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(this._context.Users.ToList());
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
