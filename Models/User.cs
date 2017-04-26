using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Web.Models
{
    public class Users
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; } = 12;

        public string Address { get; set; }
    }
}
