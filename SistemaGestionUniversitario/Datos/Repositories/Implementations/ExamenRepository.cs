using Microsoft.EntityFrameworkCore;
using Datos.Repositories.Contracts;
using Entidades.Entities;
using Datos.Contexts;
using System.Linq.Expressions;

namespace Datos.Repositories.Implementations
{
    public class ExamenRepository : Repository<Examen>, IExamenRepository
    {
        public ExamenRepository(ApplicationDbContext context) : base(context)
        {

        }

        public override async Task<IEnumerable<Examen>> FindAllAsync()
        {
            return await _context.Examen
                .Include(u => u.Materia)
                .Include(u => u.DiaHorario)
                .Include(u => u.Alumnos)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Examen>> FindByConditionAsync(Expression<Func<Examen, bool>> expression)
        {
            return await _context.Examen
                .Include(u => u.Materia)
                .Include(u => u.DiaHorario)
                .Include(u => u.Alumnos)
                .Where(expression)
                .ToListAsync();
        }

    }
}