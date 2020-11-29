using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEconomy.Data
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; }

        public User User { get; set; }

        public Active Active { get; set; }

        public int Value { get; set; }
    }
}
