namespace FinalHerr.Services;

using FinalHerr.Data;
using FinalHerr.Models;
using Microsoft.EntityFrameworkCore;
public class ClaseService : IClaseService
{
    private readonly ApplicationDbContext _context;
    private readonly IProfesorService _profesorService;

    public ClaseService(ApplicationDbContext context, IProfesorService profesorService)
    {
        _context = context;
        _profesorService = profesorService;
    }

public List<Clase> ObtenerTodos()
{
    return _context.Clase
        .Include(c => c.Profesor)
        .Include(c => c.Alumnos)  
        .ToList();
}

    public Clase ObtenerPorId(int id)
    {
        return _context.Clase.Include(c => c.Profesor).FirstOrDefault(c => c.ClaseId == id);
    }

    public async Task CrearClaseAsync(Clase clase)
    {
        _context.Add(clase);
        await _context.SaveChangesAsync();
    }

    public async Task EditarClaseAsync(int id, Clase clase)
    {
        if (id != clase.ClaseId)
            throw new ArgumentException("ID no coincide con el ID de la clase");

        _context.Update(clase);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarClaseAsync(int id)
    {
        var clase = await _context.Clase.FindAsync(id);
        if (clase != null)
        {
            _context.Clase.Remove(clase);
            await _context.SaveChangesAsync();
        }
    }

    public List<Clase> FiltrarClases(string filtro)
    {
        return _context.Clase.Include(c => c.Profesor)
            .Where(c => c.Nombre.ToLower().Contains(filtro.ToLower()) || c.Codigo.ToLower().Contains(filtro.ToLower()))
            .ToList();
    }

    public bool ClaseExiste(int id)
    {
        return _context.Clase.Any(e => e.ClaseId == id);
    }

    public List<Profesor> ObtenerProfesores()
    {
        return _profesorService.ObtenerProfesoresSinClase();
    }

    public void InscribirAlumno(Clase clase, Alumno alumno)
    {
        if (clase == null || alumno == null)
        {
            throw new ArgumentNullException();
        }

        _context.Entry(clase).Collection(c => c.Alumnos).Load();

        if (!clase.Alumnos.Any(a => a.AlumnoId == alumno.AlumnoId))
        {

            clase.Alumnos.Add(alumno);

            _context.SaveChanges();
        }
    }


    public void DesinscribirAlumno(Clase clase, Alumno alumno)
    {
        if (clase == null || alumno == null)
        {
            // Manejar el caso de argumentos nulos si es necesario
            throw new ArgumentNullException();
        }

        // Cargar explícitamente la lista de alumnos asociados a la clase
        _context.Entry(clase).Collection(c => c.Alumnos).Load();

        // Verificar si el alumno está inscrito en la clase
        var alumnoInscrito = clase.Alumnos.FirstOrDefault(a => a.AlumnoId == alumno.AlumnoId);
        if (alumnoInscrito != null)
        {
            // Remover la relación de la clase con el alumno
            clase.Alumnos.Remove(alumnoInscrito);

            // Guardar cambios en la base de datos
            _context.SaveChanges();


        }
    }





    public List<Alumno> ObtenerAlumnosInscritos(int claseId)
    {
        var clase = _context.Clase
            .Include(c => c.Alumnos)
            .FirstOrDefault(c => c.ClaseId == claseId);

        return clase?.Alumnos.ToList() ?? new List<Alumno>();
    }
}
