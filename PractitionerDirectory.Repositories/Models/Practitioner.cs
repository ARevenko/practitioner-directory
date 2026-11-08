using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PractitionerDirectory.Repositories.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Practitioner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(250)]
        public string Speciality { get; set; } = string.Empty;

        public string? PlaceOfWork { get; set; }

        [Required]
        [MaxLength(250)]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
