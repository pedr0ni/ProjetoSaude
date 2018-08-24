using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoSaude.Models
{
    public class IUserRecover
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public bool Validated { get; set; }

        public long Created { get; set; }
    }
}
