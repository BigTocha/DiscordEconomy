using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEconomy.Data
{
    public class Active
    {
        public User User { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public DateTime Stamp { get; set; }

        public double Rating { get; set; }

        public byte[] Value { get; set; }

        public bool IsCompromated { get; set; }

        public string FileName { get; set; }
    }
}
