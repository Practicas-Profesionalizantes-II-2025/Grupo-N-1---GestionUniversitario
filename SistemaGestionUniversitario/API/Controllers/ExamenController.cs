using Entidades.DTOs.Crear;
using Entidades.DTOs.Modificar;
using Entidades.DTOs.Respuestas;
using Logica.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamenController : ControllerBase
    {
        private readonly IExamenLogic _examenLogic;

        public ExamenController(IExamenLogic examenLogic)
        {
            _examenLogic = examenLogic;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerExamenes()
        {
            List<ExamenDTO> examenDTO = await _examenLogic.ObtenerExamenes();

            if (examenDTO.Count == 0)
            {
                return NoContent();
            }

            return Ok(examenDTO);
        }

        [HttpGet("{nombreMateria}")]
        public async Task<IActionResult> ObtenerExamenesPorMateria(string nombreMateria)
        {
            List<ExamenDTO> examenDTO = await _examenLogic.ObtenerExamenesPorMateria(nombreMateria);

            if (examenDTO == null)
            {
                return NotFound();
            }

            return Ok(examenDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearExamenDTO crearExamenDTO)
        {
            try
            {
                await _examenLogic.AltaExamen(crearExamenDTO.NombreMateria, crearExamenDTO.DescripcionDiaHorario, crearExamenDTO.Tipo, crearExamenDTO.Fecha);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Modificar([FromBody] ModificarExamenDTO modificarExamenDTO)
        {

            try
            {
                var examenDTO = await _examenLogic.ActualizacionExamen(
                    modificarExamenDTO.ID,
                    modificarExamenDTO.NombreMateria,
                    modificarExamenDTO.DescripcionDiaHorario,
                    modificarExamenDTO.Tipo,
                    modificarExamenDTO.Fecha
                );

                return Ok(examenDTO);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }


        [HttpDelete("{nombreMateria}/{descripcionDiaHorario}")]
        public async Task<IActionResult> EliminarPorID(string nombreMateria, string descripcionDiaHorario, DateTime fecha)
        {
            try
            {
                await _examenLogic.BajaExamen(nombreMateria, descripcionDiaHorario,fecha);

                return Ok("El examen se elimino correctamente.");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}