using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Validaciones
{
    public class PesoArchivoValidacion: ValidationAttribute
    {
        private readonly int pesoMaximoEnMEgaBytes;

        //vamos a validar por atributos el peso dela rchivo
        public PesoArchivoValidacion(int PesoMaximoEnMEgaBytes)
        {
            pesoMaximoEnMEgaBytes = PesoMaximoEnMEgaBytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formfile = value as IFormFile;

            if(formfile == null)
            {
                return ValidationResult.Success;
            }

            if(formfile.Length > pesoMaximoEnMEgaBytes * 1024 * 1024)
            {
                return new ValidationResult($"El peso no debe superar {pesoMaximoEnMEgaBytes} mb ");
            }
            return ValidationResult.Success;
        }
    }
}
