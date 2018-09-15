using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoSaude.Controllers
{
    [Authorize]
    public class IMCController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}