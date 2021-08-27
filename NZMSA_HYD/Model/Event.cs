using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace NZMSA_HYD.Model
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Order { get; set; }

        public string PhotoURI { get; set; }

        [Required]
        public int DayId { get; set; }

        public Day Day { get; set; }
    }
}
