using Core;
using Core.Service;
using Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Service;

namespace OndeDoarWeb.Controllers
{
    public class NecessidadeController : Controller
    {
        // Provisório: enquanto o CSU01 (Autenticar Usuário) não existe, usamos uma instituição fixa.
        // Depois, trocar pelo id da instituição autenticada.
        private const int IdInstituicaoProvisorio = 1;

        private readonly INecessidadeService _necessidadeService;

        public NecessidadeController(INecessidadeService necessidadeService)
        {
            _necessidadeService = necessidadeService;
        }

        private int ObterIdInstituicao()
        {
            return IdInstituicaoProvisorio;
        }

        // Retorna a necessidade apenas se ela pertencer à instituição atual.
        private Necessidade? ObterNecessidadeDaInstituicao(int id)
        {
            var necessidade = _necessidadeService.Get(id);
            if (necessidade == null || necessidade.IdInstituicao != ObterIdInstituicao())
                return null;

            return necessidade;
        }

        // GET: NecessidadeController
        public ActionResult Index()
        {
            var listaNecessidades = _necessidadeService.GetAll()
                .Where(n => n.IdInstituicao == ObterIdInstituicao());
            var listaNecessidadesModel = NecessidadeMapper.ToViewModel(listaNecessidades);
            return View(listaNecessidadesModel);
        }

        // GET: NecessidadeController/Details/5
        public ActionResult Details(int id)
        {
            var necessidade = ObterNecessidadeDaInstituicao(id);
            if (necessidade == null)
                return NotFound();

            return View(NecessidadeMapper.ToViewModel(necessidade));
        }

        // GET: NecessidadeController/Create
        public ActionResult Create()
        {
            return View(new NecessidadeViewModel());
        }

        // POST: NecessidadeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NecessidadeViewModel necessidadeModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    necessidadeModel.IdInstituicao = ObterIdInstituicao();
                    var necessidade = NecessidadeMapper.ToEntity(necessidadeModel);
                    _necessidadeService.Create(necessidade);
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(necessidadeModel);
        }

        // GET: NecessidadeController/Edit/5
        public ActionResult Edit(int id)
        {
            var necessidade = ObterNecessidadeDaInstituicao(id);
            if (necessidade == null)
                return NotFound();

            return View(NecessidadeMapper.ToViewModel(necessidade));
        }

        // POST: NecessidadeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NecessidadeViewModel necessidadeModel)
        {
            if (ObterNecessidadeDaInstituicao(id) == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    necessidadeModel.Id = id;
                    necessidadeModel.IdInstituicao = ObterIdInstituicao();
                    var necessidade = NecessidadeMapper.ToEntity(necessidadeModel);
                    _necessidadeService.Edit(necessidade);
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(necessidadeModel);
        }

        // GET: NecessidadeController/Delete/5
        public ActionResult Delete(int id)
        {
            var necessidade = ObterNecessidadeDaInstituicao(id);
            if (necessidade == null)
                return NotFound();

            return View(NecessidadeMapper.ToViewModel(necessidade));
        }

        // POST: NecessidadeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            if (ObterNecessidadeDaInstituicao(id) == null)
                return NotFound();

            _necessidadeService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}