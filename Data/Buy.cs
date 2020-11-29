using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEconomy.Data
{
    public class Buy
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        public Active Active { get; set; }

        public double Price { get; set; }

        public DateTime Stamp { get; set; }
    }
}
