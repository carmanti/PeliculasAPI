using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Services;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext context,
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await Get<Actor, ActorDTO>(paginacionDTO);
            //var queryable = context.Actores.AsQueryable();
            //await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);

            //var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            //var dtos = mapper.Map<List<ActorDTO>>(entidades);

            //return dtos;
        }

        [HttpGet("{id}", Name ="obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id) 
        {
            return await Get<Actor, ActorDTO>(id);
            //var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            //if (entidad is null)
            //{
            //    return NotFound();
            //}

            ////Destino
            //var dto = mapper.Map<ActorDTO>(entidad);   
            //return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO) //Usamos From Form para enviar desde un archivo 
        {
            //En postman se usa form-data y cambiamos al momento de enviar la foto a file
            //mapeamos a la entidad
            var entidad = mapper.Map<Actor>(actorCreacionDTO);

            if(actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    entidad.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                        actorCreacionDTO.Foto.ContentType);
                }
            }

            context.Add(entidad);
            await context.SaveChangesAsync();
            //mapeamos al dto de lectura
            var dto = mapper.Map<ActorDTO>(entidad);
            return new CreatedAtRouteResult("obtenerActor",new {id = entidad.Id},dto);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if(actorDB == null) { return NotFound(); }
            actorDB = mapper.Map(actorCreacionDTO, actorDB);
            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                        actorDB.Foto,
                        actorCreacionDTO.Foto.ContentType);
                }
            }
            //var entidad = mapper.Map<Actor>(actorCreacionDTO);
            //entidad.Id = id;
            //context.Entry(entidad).State= EntityState.Modified;
            context.SaveChanges();
            return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument) 
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
            //if(patchDocument == null)
            //{
            //    return BadRequest();
            //}

            //var entidad = await context.Actores.FirstOrDefaultAsync(x=> x.Id == id);

            //if (entidad == null)
            //{
            //    return NotFound();
            //}

            //var entidadDTO = mapper.Map<ActorPatchDTO>(entidad);

            //patchDocument.ApplyTo(entidadDTO, ModelState);

            //var esValido = TryValidateModel(entidadDTO);

            //if (!esValido)
            //{
            //    return BadRequest();
            //}

            //mapper.Map(entidadDTO, entidad);
            //await context.SaveChangesAsync();

            //return NoContent();
        
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
            //var existe = await context.Actores.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Actor() { Id = id });
            //await context.SaveChangesAsync();
            //return NoContent();
        }

    }
}
