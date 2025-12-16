using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests
{
    public class RegisterNurseRequestDto: RegisterBaseDto
    {
        [Required]
        public string LicenseNumber { get; set; }
        
   
        public string Specialization { get; set; }

        [Required]
        public int ExperienceYears { get; set; }
        [Required]
        public int NursePhoneNumber { get; set; }
        [Required]
        public bool IsActive { get; set; }=true;

    }
}
