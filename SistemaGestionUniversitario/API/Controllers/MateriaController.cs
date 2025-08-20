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
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaLogic _materiaLogic;

        public MateriaController(IMateriaLogic materiaLogic)
        {
            _materiaLogic = materiaLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterias()
        {
            List<MateriaDTO> materiasDTO = await _materiaLogic.ObtenerMaterias();

            if (materiasDTO.Count == 0)
            {
                return NoContent();
            }

            return Ok(materiasDTO);
        }

        [HttpGet("NombreMateria/{nombreMateria}")]
        public async Task<IActionResult> GetMateria(string nombreMateria)
        {
            try
            {
                MateriaDTO materiaDTO = await _materiaLogic.ObtenerMateriaNombre(nombreMateria);
                
                if (materiaDTO == null)
                {
                    return NotFound();
                }

                return Ok(materiaDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }     
        }

        [HttpGet("DNIProfesor/{nombreMateria}")]
        public async Task<IActionResult> GetMateriasDNIProfesor(string dni)
        {
            try
            {
                List<MateriaDTO> materiasDTO = await _materiaLogic.ObtenerMateriasDNIProfesor(dni);

                if (materiasDTO == null)
                {
                    return NoContent();
                }

                return Ok(materiasDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostMateria([FromBody] CrearMateriaDTO materia)
        {
            try
            {
                await _materiaLogic.AltaMateria(
                materia.Nombre,
                materia.ProfesoresIDs,
                materia.DiasHorariosIDs,
                materia.Modalidad,
                materia.Anio);

                return Ok("Materia creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{nombreMateria}")]
        public async Task<IActionResult> PutMateria(string nombreMateria, [FromBody] ModificarMateriaDTO materia)
        {
            try
            {
                MateriaDTO materiaDTO = await _materiaLogic.ActualizacionMateria(
                nombreMateria,
                materia.ProfesoresIDs,
                materia.DiasHorariosIDs);

                if (materiaDTO == null)
                {
                    return NotFound();
                }

                return Ok(materiaDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }   
        }

        [HttpDelete("{nombreMateria}")]
        public async Task<IActionResult> DeleteMateria(string nombreMateria)
        {
            try
            {
                await _materiaLogic.BajaMateria(nombreMateria);

                return Ok($"La materia {nombreMateria} fue eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}