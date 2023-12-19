namespace FinalHerr.Services;

using FinalHerr.Data;
using FinalHerr.Models;
using Microsoft.EntityFrameworkCore;

// AlumnoService.cs
public class AlumnoService : IAlumnoService
{
    private readonly ApplicationDbContext _context;

    public AlumnoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Alumno> ObtenerTodos()
    {
        return _context.Alumno
            .Include(a => a.Clases)  // Incluye la propiedad Clases
            .ToList();
    }

    public Alumno ObtenerPorId(int id)
    {
        return _context.Alumno
                   .Include(a => a.Clases)  
                   .FirstOrDefault(a => a.AlumnoId == id);
    }

    
    public async Task CrearAlumnoAsync(Alumno alumno)
    {
        _context.Add(alumno);
        await _context.SaveChangesAsync();
    }

    public async Task EditarAlumnoAsync(int id, Alumno alumno)
    {
        if (id != alumno.AlumnoId)
            throw new ArgumentException("ID no coincide con el ID del alumno");

        _context.Update(alumno);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAlumnoAsync(int id)
    {
        var alumno = await _context.Alumno.FindAsync(id);
        if (alumno != null)
        {
            _context.Alumno.Remove(alumno);
            await _context.SaveChangesAsync();
        }
    }

    public List<Alumno> FiltrarAlumnos(string filtro)
    {
        return _context.Alumno
            .Where(a => a.Nombre.ToLower().Contains(filtro.ToLower()) || a.Carrera.ToLower().Contains(filtro.ToLower()))
            .ToList();
    }

    public bool AlumnoExiste(int id)
    {
        return _context.Alumno.Any(e => e.AlumnoId == id);
    }
}
