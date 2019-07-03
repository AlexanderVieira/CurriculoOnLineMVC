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

            //var candidatoPerfil = await _context.CandidatoPerfils
            //    .Include(p => p.Perfil).Where(cp => cp.PerfilId == cp.Perfil.Id)
            //    .Include(c => c.Candidato).Where(cp => cp.CandidatoId == id)
            //    .Select(c => new { c.Candidato.Nome, c.Perfil.Descricao })
            //    .DefaultIfEmpty().ToListAsync();            

            var candidatoPerfil = await _context.CandidatoPerfils
                .Include(p => p.Perfil).Where(cp => cp.PerfilId == cp.Perfil.Id)
                .Include(c => c.Candidato).Where(cp => cp.CandidatoId == id)
                .Select(c => new
                {
                    Id = c.Candidato.Id,
                    Nome = c.Candidato.Nome == null ? string.Empty : c.Candidato.Nome,
                    Descricao = c.Perfil.Descricao == null ? string.Empty : c.Perfil.Descricao,
                    CandidatoId = c == null ? 0 : c.CandidatoId,
                    PerfilId = c == null ? 0 : c.PerfilId
                })
                .ToListAsync();


            var results = (from data1 in _context.Perfis                           
                           join data2 in _context.Candidatos
                           on id equals data2.Id
                           into groupjoin
                           from data2 in groupjoin.DefaultIfEmpty()
                           select new
                           {
                                data1.Id, data1.Descricao, Nome = data2.Nome == null ? "" : data2.Nome
                           }).ToList();            

            foreach (var item in results)
            {
                var value = item;
                //var perfilId = item.perfilId;
                //var descricao = item.descricaoPerfil;
                //var candidatoId = item.candidatoId;
                //var nome = item.candidatoNome;
                //var cand_Id = item.CandidatoId;
                //var perf_Id = item.PerfilId;
            }

            if (candidatoPerfil == null)
            {
                return NotFound();
            }

            //ViewBag.Candidato = new SelectList(_context.Candidatos, "Nome", "Nome", candidatoPerfil[0].Candidato.Nome);
            //ViewBag.Perfil = new SelectList(_context.Perfis, "Descricao", "Descricao", candidatoPerfil);

            return View(candidatoPerfil);
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
