using Entidades.DTOs.Crear;
using Entidades.DTOs.Modificar;
using Entidades.DTOs.Respuestas;
using Entidades.Entities;
using Logica.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaLogic _asistenciaLogic;

        public AsistenciaController(IAsistenciaLogic asistenciaLogic)
        {
            _asistenciaLogic = asistenciaLogic;
        }

        [HttpGet("NombreMateria/{nombreMateria}")]
        public async Task<IActionResult> ObtenerAsistenciasPorMateria(string nombreMateria)
        {
            try
            {
                List<AsistenciaDTO> asistenciaDTO = await _asistenciaLogic.ObtenerAsistenciasPorMateria(nombreMateria);

                if (asistenciaDTO.Count == 0)
                {
                    return NoContent();
                }

                return Ok(asistenciaDTO);

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("DNI/{dni}")]
        public async Task<IActionResult> ObtenerInasistenciasPorAlumno(string dni)
        {
            try
            {
                List<AsistenciaDTO> asistenciaDTO = await _asistenciaLogic.ObtenerInasistenciasPorAlumno(dni);

                if (asistenciaDTO.Count == 0)
                {
                    return NoContent();
                }

                return Ok(asistenciaDTO);

            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearAsistenciaDTO crearAsistenciaDTO)
        {
            try
            {
                await _asistenciaLogic.AltaAsistencia(crearAsistenciaDTO.idInscripcion, crearAsistenciaDTO.idDiaHorarioMateria, crearAsistenciaDTO.Estado, crearAsistenciaDTO.Fecha);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{dniAlumno}/{nombreMateria}")]
        public async Task<IActionResult> Modificar(string dniAlumno, string nombreMateria, [FromBody] ModificarAsistenciaDTO modificarAsistenciaDTO)
        {
            try
            {
                AsistenciaDTO asistenciaDTO = await _asistenciaLogic.ActualizarAsistencia(dniAlumno, nombreMateria, modificarAsistenciaDTO.Fecha, modificarAsistenciaDTO.Estado);

                if (asistenciaDTO == null)
                {
                    return NotFound();
                }

                return Ok(asistenciaDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }       
        }

        [HttpDelete("{dniAlumno}/{nombreMateria}/{fecha}")]
        public async Task<IActionResult> Eliminar(string dniAlumno, string nombreMateria, DateTime fecha)
        {
            try
            {
                await _asistenciaLogic.EliminarAsistencia(dniAlumno, nombreMateria, fecha);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}