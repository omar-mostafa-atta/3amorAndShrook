using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }

        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public string? Bio { get; set; } // el Bio nullable ya Shrook 34an momkn ykon msh 3andoh ay Bio 3aiz yktbo bs lw 7aba t8ereha 2olili 
        public int PhoneNumber { get; set; }
        public string AvailabilitySchedule { get; set; }

        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalTask> MedicalTasks { get; set; }
    }

}
