using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Nurse
    {
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }

        public string LicenseNumber { get; set; }
        public string Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public int PhoneNumber { get; set; }
        public bool IsActive { get; set; }


        public ICollection<HomeServiceRequest> HomeServiceRequests { get; set; }
    }

}
