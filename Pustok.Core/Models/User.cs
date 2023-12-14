using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Core.Models
{
    public class User : IdentityUser
    {
        public string Fullname { get; set; }
        public string BirthDate { get; set; }

        public List<BasketProduct> BasketItems { get; set; }
    }
}
