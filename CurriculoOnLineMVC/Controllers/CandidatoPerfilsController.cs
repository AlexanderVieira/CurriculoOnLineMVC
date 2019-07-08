using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurriculoOnLineMVC.Context;
using CurriculoOnLineMVC.Models;
using CurriculoOnLineMVC.Util;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace CurriculoOnLineMVC.Controllers
{
    public class CandidatoPerfilsController : Controller
    {
        private readonly CurriculoOnLineDbContext _context;
        private const string SESSION_RESULTS = "results";

        public CandidatoPerfilsController(CurriculoOnLineDbContext context)
        {
            _context = context;
        }

        // GET: CandidatoPerfils
        public async Task<IActionResult> Index(int? id)
        {
            var perfilIds = new List<int>();
            List<CandidatoPerfil> results = null;

            if (id == null)
            {
                var perfis = await _context.Perfis.ToListAsync();
                var associacoes = await _context.CandidatoPerfils.Include(c => c.Candidato).Include(c => c.Perfil).ToListAsync();

                //ViewBag.CandidatoPerfis = HttpContext.Session.GetComplexData<List<CandidatoPerfil>>(SESSION_RESULTS);
                //ViewBag.CandidatoId = new SelectList(_context.Candidatos, "Id", "Id");
                ViewBag.CandidatoId = id;
                ViewBag.perfis = perfis;

                return View(associacoes);
            }

            try
            {
                var candidatoPerfil = await _context.CandidatoPerfils
                    .Include(c => c.Candidato)
                    .Include(c => c.Perfil)
                    .FirstOrDefaultAsync(m => m.CandidatoId == id);

                if (candidatoPerfil == null)
                {
                    var perfis = await _context.Perfis.ToListAsync();
                    var associacoes = await _context.CandidatoPerfils.Include(c => c.Candidato).Include(c => c.Perfil).ToListAsync();
                    var candidato = await _context.Candidatos.FirstOrDefaultAsync(c => c.Id == id);
                    
                    //ViewBag.CandidatoPerfis = HttpContext.Session.GetComplexData<List<CandidatoPerfil>>(SESSION_RESULTS);
                    //ViewBag.CandidatoId = new SelectList(_context.Candidatos, "Id", "Id");

                    if (candidato != null)
                    {
                        ViewBag.perfis = perfis;
                        ViewBag.CandidatoId = candidato.Id;
                        ViewBag.CandidatoNome = candidato.Nome;
                        return View(associacoes);
                    }
                    else
                    {
                        ViewBag.perfis = perfis;
                        return View(associacoes);
                    }
                    
                }

                var candidatoPerfis = await _context.CandidatoPerfils
                    .Include(p => p.Perfil)
                    .Include(c => c.Candidato).Where(cp => cp.CandidatoId == id).DefaultIfEmpty()
                    .Select(cp => new CandidatoPerfil()
                    {
                        CandidatoId = cp.CandidatoId,
                        PerfilId = cp.PerfilId
                    })
                    .ToListAsync();

                int perfilId = 0;
                for (int i = 0; i < candidatoPerfis.Count; i++)
                {
                    perfilId = candidatoPerfis[i].PerfilId;
                    perfilIds.Add(perfilId);
                }

                results = (from p in _context.Perfis
                    join c in _context.Candidatos
                        on id equals c.Id
                        into groupjoin
                    from c in groupjoin.DefaultIfEmpty()
                    select (new CandidatoPerfil()
                    {
                        Candidato = c,
                        Perfil = p,
                        CandidatoId = c.Id,
                        PerfilId = p.Id

                    })).ToList();
            }
            catch (Exception ex)
            {
                var result = ex.Message;
            }

            if (results == null)
            {
                return NotFound();
                //return RedirectToAction("Details");
            }

            ViewBag.PerfilIds = perfilIds;
            ViewBag.CandidatoId = id;
            //ViewBag.CandidatoId = new SelectList(_context.Candidatos, "Id", "Id");

            return View(results);
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
                .Include(c => c.Candidato).Where(cp => cp.CandidatoId == id).DefaultIfEmpty()
                .Select(cp => new { cp.PerfilId, cp.CandidatoId })
                .ToListAsync();

            var perfilIds = new List<int>();
            List<CandidatoPerfil> results = null;

            try
            {
                    results = (from p in _context.Perfis
                               join c in _context.Candidatos
                               on id equals c.Id
                               into groupjoin
                               from c in groupjoin.DefaultIfEmpty()
                               select (new CandidatoPerfil()
                               {
                                   Candidato = c,
                                   Perfil = p,
                                   CandidatoId = c.Id,
                                   PerfilId = p.Id

                               })).ToList();
            }
            catch (Exception ex)
            {
                var result = ex.Message;
            }

            foreach (var item in candidatoPerfil)
            {
                perfilIds.Add(item.PerfilId);
            }

            ViewBag.perfilIds = perfilIds;

            if (results == null)
            {
                return NotFound();
                //return RedirectToAction("Details");
            }

            //HttpContext.Session.SetComplexData(SESSION_RESULTS, results);

            ViewBag.CandidatoId = new SelectList(_context.Candidatos, "Id", "Id");
            //ViewBag.Perfil = new SelectList(_context.Perfis, "Descricao", "Descricao", candidatoPerfil);
            //return RedirectToAction("Index");
            return View(results);
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
        public async Task<IActionResult> Delete(int? id, string nome, int[] perfilIdUnChecked, int[] perfilIdChecked)
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
        public async Task<IActionResult> DeleteConfirmed(int id, string nome, int[] perfilIdUnChecked, int[] perfilIdChecked)
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
