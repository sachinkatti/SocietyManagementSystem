using System.ComponentModel.DataAnnotations;

namespace SMS.Application
{
    public class LanguageDto
    {
        public long LanguageId { get; set; }

        [Required(ErrorMessage = "Language code is required.")]
        [StringLength(10, ErrorMessage = "Language code cannot exceed 10 characters.")]
        public string LanguageCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language name is required.")]
        [StringLength(100, ErrorMessage = "Language name cannot exceed 100 characters.")]
        public string LanguageName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifyOn { get; set; }

        public long? ModifyBy { get; set; }
    }
}