using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProjetoSaude.Models
{
    public class IUser
    {
        public int Id { get; set; }

        public string Cpf { get;set; }

        public string Nome { get; set; }

        public string Rg { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public bool Status { get; set; }

        public string Perfil { get; set; }
    }
}
