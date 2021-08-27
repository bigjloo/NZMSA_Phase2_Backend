using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace NZMSA_HYD.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Github { get; set; }

        public string ImageURI { get; set; }
        public ICollection<Day> Days { get; set; } = new List<Day>();

    }
}
