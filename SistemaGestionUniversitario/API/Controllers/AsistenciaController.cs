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
                    return Ok(new List<AsistenciaDTO>());
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
                    return Ok(new List<AsistenciaDTO>());
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
                await _asistenciaLogic.AltaAsistencia(crearAsistenciaDTO.idInscripcion, crearAsistenciaDTO.nombreMateria, crearAsistenciaDTO.Estado, crearAsistenciaDTO.Fecha);

                return Ok("Asistencia registrada correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error inesperado al registrar la asistencia." });
            }
        }

        [HttpDelete("{nombreMateria}/{fecha}")]
        public async Task<IActionResult> Eliminar(string nombreMateria, DateTime fecha)
        {
            try
            {
                await _asistenciaLogic.EliminarAsistencia(nombreMateria, fecha);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // ------ Se deja codigo por si se cambia la forma de tomar la asistencia, pero por el momento no se usa ------
        #region
        //[HttpPut("{dniAlumno}/{nombreMateria}")]
        //public async Task<IActionResult> Modificar(string dniAlumno, string nombreMateria, [FromBody] ModificarAsistenciaDTO modificarAsistenciaDTO)
        //{
        //    try
        //    {
        //        AsistenciaDTO asistenciaDTO = await _asistenciaLogic.ActualizarAsistencia(dniAlumno, nombreMateria, modificarAsistenciaDTO.Fecha, modificarAsistenciaDTO.Estado);

        //        if (asistenciaDTO == null)
        //        {
        //            return NotFound();
        //        }

        //        return Ok(asistenciaDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { mensaje = ex.Message });
        //    }       
        //}
        #endregion
    }
}