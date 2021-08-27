using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace NZMSA_HYD.Model
{
    public class Day
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string PublishKey { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
