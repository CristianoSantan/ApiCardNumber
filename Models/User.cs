using System.ComponentModel.DataAnnotations;

namespace testeef.Models
{
    public class User
    {
        [Key]

        public string Id_email { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]

        public string Name { get; set; }
    }
}