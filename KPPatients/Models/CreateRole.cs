using System.ComponentModel.DataAnnotations;

namespace KPPatients.Models
{
    public class CreateRole
    {
        [Required]
        public string RoleName { get; set; }
    }
}
