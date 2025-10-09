using Entidades.DTOs.Crear;
using Entidades.DTOs.Modificar;
using Entidades.DTOs.Respuestas;
using Logica.Contracts;
using Logica.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotaAlumnoController : ControllerBase
    {
        private readonly INotaAlumnoLogic _notaAlumnoLogic;

        public NotaAlumnoController(INotaAlumnoLogic notaAlumnoLogic)
        {
            _notaAlumnoLogic = notaAlumnoLogic;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerNotas()
        {
            List<NotaAlumnoDTO> notaAlumnoDTO = await _notaAlumnoLogic.ObtenerNotas();

            if (notaAlumnoDTO.Count == 0)
            {
                return NoContent();
            }

            return Ok(notaAlumnoDTO);
        }

        [HttpGet("nombreMateria/{nombreMateria}")]
        public async Task<IActionResult> ObtenerNotasPorMateria(string nombreMateria)
        {
            try
            {
                List<NotaAlumnoDTO> notaAlumnoDTO = await _notaAlumnoLogic.ObtenerNotasPorMateria(nombreMateria);

                if (notaAlumnoDTO == null)
                {
                    return NotFound();
                }

                return Ok(notaAlumnoDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }           
        }

        [HttpGet("dniAlumno/{dniAlumno}")]
        public async Task<IActionResult> ObtenerNotasPorAlumno(string dniAlumno)
        {
            try
            {
                List<NotaAlumnoDTO> notaAlumnoDTO = await _notaAlumnoLogic.ObtenerNotasPorAlumno(dniAlumno);
                
                if (notaAlumnoDTO == null)
                {
                    return NotFound();
                }

                return Ok(notaAlumnoDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpGet("idExamen/{idExamen}")]
        public async Task<IActionResult> ObtenerNotasPorExamen(int idExamen)
        {
            try
            {
                List<NotaAlumnoDTO> notaAlumnoDTO = await _notaAlumnoLogic.ObtenerNotasPorExamen(idExamen);

                if (notaAlumnoDTO == null)
                {
                    return NotFound();
                }

                return Ok(notaAlumnoDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearNotaAlumno([FromBody] CrearNotaAlumnoDTO crearNotaAlumnoDTO)
        {
            try
            {
                await _notaAlumnoLogic.AltaNotaAlumno(crearNotaAlumnoDTO.Nota, crearNotaAlumnoDTO.DNIAlumno, crearNotaAlumnoDTO.IDExamen);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{dniAlumno}/{idExamen}")]
        public async Task<IActionResult> ModificarNotaAlumno(string dniAlumno, int idExamen, [FromBody] ModificarNotaAlumnoDTO modificarNotaAlumnoDTO)
        {
            try
            {
                NotaAlumnoDTO notaAlumnoDTO = await _notaAlumnoLogic.ActualizacionNotaAlumno(
                modificarNotaAlumnoDTO.Nota,
                dniAlumno,
                idExamen);

                if (notaAlumnoDTO == null)
                {
                    return NotFound();
                }

                return Ok(notaAlumnoDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }      
        }

        [HttpDelete("{dniAlumno}/{idExamen}")]
        public async Task<IActionResult> EliminarNotaAlumno(string dniAlumno, int idExamen)
        {
            try
            {
                await _notaAlumnoLogic.BajaNotaAlumno(dniAlumno, idExamen);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}