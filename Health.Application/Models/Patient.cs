using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        public User User { get; set; }

    }
}
