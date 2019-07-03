using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurriculoOnLineMVC.Context;
using CurriculoOnLineMVC.Models;

namespace CurriculoOnLineMVC.Controllers
{
    public class CandidatoPerfilsController : Controller
    {
        private readonly CurriculoOnLineDbContext _context;

        public CandidatoPerfilsController(CurriculoOnLineDbContext context)
        {
            _context = context;
        }

        // GET: CandidatoPerfils
        public async Task<IActionResult> Index()
        {           
            var curriculoOnLineDbContext = _context.CandidatoPerfils.Include(c => c.Candidato).Include(c => c.Perfil);
            return View(await curriculoOnLineDbContext.ToListAsync());
        }

        // GET: CandidatoPerfils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidatoPerfil = await _context.CandidatoPerfils
                .Include(p => p.Perfil)
                .Include(c => c.Candidato).Where(cp => cp.CandidatoId == id)
                .Select(c => new { c.Candidato.Id, c.Perfil.Descricao, c.PerfilId })
                .DefaultIfEmpty().ToListAsync();

            //var perfilDbContext = await _context.Perfis
            //    .Include(x => x.Candidatos)                
            //    .Select(x => new { x.Id, x.Descricao, candidato = _context.Candidatos }).DefaultIfEmpty().ToListAsync();

            //var candidatoPerfil = await _context.CandidatoPerfils
            //    .Include(p => p.Perfil)
            //    .Include(c => c.Candidato)
            //    .Select(c => new
            //    {
            //        Id = c.Candidato.Id,
            //        Nome = c.Candidato.Nome == null ? string.Empty : c.Candidato.Nome,
            //        Descricao = c.Perfil.Descricao == null ? string.Empty : c.Perfil.Descricao,                    
            //        CandidatoId = c == null ? 0 : c.CandidatoId,
            //        PerfilId = c == null ? 0 : c.PerfilId
            //    })
            //    .ToListAsync();

            var results = await (from data1 in _context.Perfis
                                 join data2 in _context.Candidatos
                                 on id equals data2.Id
                                 into groupjoin
                                 from data2 in groupjoin.DefaultIfEmpty()
                                 select new
                                 {
                                     //Perfis = data1.Candidatos.ToList(),
                                     perfil_Id = data1.Id,
                                     Descricao = data1.Descricao,
                                     CandidatoId = data2.Id,
                                     Nome = data2.Nome == null ? "" : data2.Nome

                                 }).ToListAsync();
            Candidato candidato = null;
            Perfil perfil = null;
            CandidatoPerfil cand_perfil = null;
            var candidatoPerfils = new List<CandidatoPerfil>();
            var perfilIds = new List<int>();
            //var perfilIdsDbCtx = await _context.CandidatoPerfils.ToListAsync();

            for (int i = 0; i < candidatoPerfil.Count; i++)
            {
                perfilIds.Add(candidatoPerfil[i].PerfilId);
            }

            ViewBag.PerfilIds = perfilIds;

            for (int i = 0; i < results.Count; i++)
            {
                candidato = new Candidato()
                {
                    Id = results.ToList()[i].CandidatoId,
                    Nome = results.ToList()[i].Nome
                };

                perfil = new Perfil()
                {
                    Id = results.ToList()[i].perfil_Id,
                    Descricao = results.ToList()[i].Descricao
                };

                cand_perfil = new CandidatoPerfil()
                {
                    Candidato = candidato,
                    Perfil = perfil
                };



                candidatoPerfils.Add(cand_perfil);

            }



            if (candidatoPerfils == null)
            {
                return NotFound();
            }

            //ViewBag.Candidato = new SelectList(_context.Candidatos, "Nome", "Nome", candidatoPerfil[0].Candidato.Nome);
            //ViewBag.Perfil = new SelectList(_context.Perfis, "Descricao", "Descricao", candidatoPerfil);
            
            return View(candidatoPerfils);
        }

        // GET: CandidatoPerfils/Create
        public IActionResult Create()
        {
            ViewData["CandidatoId"] = new SelectList(_context.Candidatos, "Id", "Id");
            ViewData["PerfilId"] = new SelectList(_context.Perfis, "Id", "Id");
            return View();
        }

        // POST: CandidatoPerfils/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CandidatoId,PerfilId")] CandidatoPerfil candidatoPerfil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(candidatoPerfil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CandidatoId"] = new SelectList(_context.Candidatos, "Id", "Id", candidatoPerfil.CandidatoId);
            ViewData["PerfilId"] = new SelectList(_context.Perfis, "Id", "Id", candidatoPerfil.PerfilId);
            return View(candidatoPerfil);
        }

        // GET: CandidatoPerfils/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidatoPerfil = await _context.CandidatoPerfils
                .Include(c => c.Candidato)
                .Include(c => c.Perfil)
                .FirstOrDefaultAsync(m => m.CandidatoId == id);

            if (candidatoPerfil == null)
            {
                return NotFound();
            }
            ViewBag.CandidatoId = new SelectList(_context.Candidatos, "Id", "Id", candidatoPerfil.CandidatoId);
            ViewBag.PerfilId = new SelectList(_context.Perfis, "Id", "Id", candidatoPerfil.PerfilId);
            return View(candidatoPerfil);
        }

        // POST: CandidatoPerfils/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CandidatoId,PerfilId")] CandidatoPerfil candidatoPerfil)
        {
            if (id != candidatoPerfil.CandidatoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(candidatoPerfil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandidatoPerfilExists(candidatoPerfil.CandidatoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CandidatoId"] = new SelectList(_context.Candidatos, "Id", "Id", candidatoPerfil.CandidatoId);
            ViewData["PerfilId"] = new SelectList(_context.Perfis, "Id", "Id", candidatoPerfil.PerfilId);
            return View(candidatoPerfil);
        }

        // GET: CandidatoPerfils/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidatoPerfil = await _context.CandidatoPerfils
                .Include(c => c.Candidato)
                .Include(c => c.Perfil)
                .FirstOrDefaultAsync(m => m.CandidatoId == id);
            if (candidatoPerfil == null)
            {
                return NotFound();
            }

            return View(candidatoPerfil);
        }

        // POST: CandidatoPerfils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidatoPerfil = await _context.CandidatoPerfils.FindAsync(id);
            _context.CandidatoPerfils.Remove(candidatoPerfil);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidatoPerfilExists(int id)
        {
            return _context.CandidatoPerfils.Any(e => e.CandidatoId == id);
        }
    }
}
