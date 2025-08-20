using Entidades.DTOs.Crear;
using Entidades.DTOs.Respuestas;
using Logica.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionController : ControllerBase
    {
        private readonly IInscripcionLogic _inscripcionLogic;
        
        public InscripcionController(IInscripcionLogic inscripcionLogic)
        {
            _inscripcionLogic = inscripcionLogic;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerInscripciones()
        {
            try
            {
                List<InscripcionDTO> inscripcionDTO = await _inscripcionLogic.ObtenerInscripciones();

                if (inscripcionDTO.Count == 0)
                {
                    return NoContent();
                }

                return Ok(inscripcionDTO);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Inscripcion/PorMateria/NombreMateria
        [HttpGet("PorMateria/{nombreMateria}")]
        public async Task<IActionResult> ObtenerInscripcionesMateria(string nombreMateria)
        {
            try
            {
                List<InscripcionDTO> inscripcionDTO = await _inscripcionLogic.ObtenerInscripcionesMateria(nombreMateria);

                if (inscripcionDTO.Count == 0)
                {
                    return NoContent();
                }

                return Ok(inscripcionDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Inscripcion/PorDNI/DNIAlumno
        [HttpGet("PorDNI/{nombreMateria}")]
        public async Task<IActionResult> ObtenerInscripcionesDNI(string dni)
        {
            try
            {
                List<InscripcionDTO> inscripcionDTO = await _inscripcionLogic.ObtenerInscripcionesDNI(dni);

                if (inscripcionDTO.Count == 0)
                {
                    return NoContent();
                }

                return Ok(inscripcionDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AltaInscripcion([FromBody]CrearInscripcionDTO crearInscripcionDTO)
        {
            try
            {
                await _inscripcionLogic.AltaInscripcion(crearInscripcionDTO.DNIAlumno, crearInscripcionDTO.NombreMateria);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{nombreMateria}/{dniAlumno}")]
        public async Task<IActionResult> BajaInscripcion(string nombreMateria, string dniAlumno)
        {
            try
            {
                await _inscripcionLogic.BajaInscripcion(nombreMateria, dniAlumno);

                return Ok();
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}