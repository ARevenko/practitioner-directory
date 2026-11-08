using System.ComponentModel.DataAnnotations;

namespace PractitionerDirectory.API.Models
{
    public class CreatePractitionerRequest
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 150 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Фамилия должна быть от 2 до 150 символов")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30, ErrorMessage = "Номер телефона должен быть не более 30 символов")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Специальность обязательна")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Специальность должно быть от 2 до 250 символов")]
        public string Speciality { get; set; } = string.Empty;

        public string? PlaceOfWork { get; set; }

        [Required(ErrorMessage = "Эл. почта обязательна")]
        [MaxLength(250, ErrorMessage = "Эл. почта должно быть не более 250 символов")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
