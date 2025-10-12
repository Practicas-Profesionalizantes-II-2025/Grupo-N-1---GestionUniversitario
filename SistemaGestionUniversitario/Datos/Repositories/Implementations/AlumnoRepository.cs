using Entidades.Entities;
using Datos.Repositories.Contracts;
using Datos.Contexts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Datos.Repositories.Implementations
{
    public class AlumnoRepository : Repository<Alumno>, IAlumnoRepository
    {
        public AlumnoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Alumno>> FindAllAsync()
        {
            return await _context.Alumno
                .Include(u => u.Usuario)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Alumno>> FindByConditionAsync(Expression<Func<Alumno, bool>> expression)
        {
            return await _context.Alumno
                .Include(u => u.Usuario)
                .Where(expression)
                .ToListAsync();
        }
    }
}