using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class ActorCreacionDTO: ActorPatchDTO
    {
        //Para recibir la infromacion de la foto

        [PesoArchivoValidacion(PesoMaximoEnMEgaBytes:4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
