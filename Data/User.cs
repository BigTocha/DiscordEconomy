using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEconomy.Data
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        public double Rating { get; set; }

        public double Money { get; set; }

        public bool IsAdmin { get; set; }
        
        public virtual List<Active> Actives { get; set; }
    }
}
