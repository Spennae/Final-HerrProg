
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalHerr.Models;
using FinalHerr.Services;
using Microsoft.AspNetCore.Authorization;

namespace FinalHerr.Controllers
{
    [Authorize(Roles = "SysAdmin,Administracion")]

    public class ClasesController : Controller
    {
        private readonly IClaseService _claseService;

        public ClasesController(IClaseService claseService)
        {
            _claseService = claseService;
        }

        public IActionResult Index(string Filtro)
        {
            var clases = _claseService.ObtenerTodos();
            
            if (!string.IsNullOrEmpty(Filtro))
            {
                clases = _claseService.FiltrarClases(Filtro);
            }

            return View(clases);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !_claseService.ClaseExiste(id.Value))
            {
                return NotFound();
            }

            var clase = _claseService.ObtenerPorId(id.Value);
            if (clase == null)
            {
                return NotFound();
            }

            return View(clase);
        }

        public IActionResult Create()
        {
            ViewData["ProfesorId"] = new SelectList(_claseService.ObtenerProfesores(), "ProfesorId", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClaseId,Nombre,Codigo,Aula,ProfesorId")] Clase clase)
        {
            if (ModelState.IsValid)
            {
                await _claseService.CrearClaseAsync(clase);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfesorId"] = new SelectList(_claseService.ObtenerProfesores(), "ProfesorId", "Nombre", clase.ProfesorId);
            return View(clase);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_claseService.ClaseExiste(id.Value))
            {
                return NotFound();
            }

            var clase = _claseService.ObtenerPorId(id.Value);
            if (clase == null)
            {
                return NotFound();
            }
            ViewData["ProfesorId"] = new SelectList(_claseService.ObtenerProfesores(), "ProfesorId", "Nombre", clase.ProfesorId);
            return View(clase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClaseId,Nombre,Codigo,Aula,ProfesorId")] Clase clase)
        {
            if (id != clase.ClaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _claseService.EditarClaseAsync(id, clase);
                }
                catch (ArgumentException)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfesorId"] = new SelectList(_claseService.ObtenerProfesores(), "ProfesorId", "Nombre", clase.ProfesorId);
            return View(clase);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_claseService.ClaseExiste(id.Value))
            {
                return NotFound();
            }

            var clase = _claseService.ObtenerPorId(id.Value);
            if (clase == null)
            {
                return NotFound();
            }

            return View(clase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _claseService.EliminarClaseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
