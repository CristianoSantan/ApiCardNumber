using System.ComponentModel.DataAnnotations;

namespace testeef.Models
{
    public class Product
    {
        [Key]

        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]

        public string Title { get; set; }

        public int Number_card { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]

        public string UserId_email { get; set; }

        public User User { get; set; }
    }
}