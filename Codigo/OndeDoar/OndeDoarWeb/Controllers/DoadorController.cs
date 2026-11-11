using Core;
using Core.Service;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace OndeDoarWeb.Controllers
{
    public class DoadorController : Controller
    {
        IDoadorService _doadorService;

        public DoadorController(IDoadorService doadorService)
        {
            _doadorService = doadorService;
        }


        // GET: DoadorController
        public ActionResult Index()
        {
            var listarDoadores = _doadorService.ObterTodos();
            var listarDoadoresModel = listarDoadores.Adapt<List<DoadorViewModel>>();
            return View(listarDoadoresModel);
        }

        // GET: DoadorController/Details/5
        public ActionResult Details(int id)
        {
            Doador doador = _doadorService.Obter(id);
            DoadorViewModel doadorModel = doador.Adapt<DoadorViewModel>();
            return View(doadorModel);
        }

        // GET: DoadorController/Create
        public ActionResult InserirDoador()
        {
            return View();
        }

        // POST: DoadorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserirDoador(DoadorViewModel doadorModel)
        {
            if (ModelState.IsValid)
            {
                var doador = doadorModel.Adapt<Doador>();
                _doadorService.Inserir(doador);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DoadorController/Edit/5
        public ActionResult AlterarDoador(int id)
        {
            Doador doador = _doadorService.Obter(id);
            DoadorViewModel doadorModel = doador.Adapt<DoadorViewModel>();
            return View(doadorModel);
        }

        // POST: DoadorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarDoador(int id, DoadorViewModel doadorModel)
        {
           if (ModelState.IsValid)
            {
                var doador = doadorModel.Adapt<Doador>();
                _doadorService.Editar(doador);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DoadorController/Delete/5
        public ActionResult RemoverDoador(int id)
        {
            Doador doador = _doadorService.Obter(id);
            DoadorViewModel doadorModel = doador.Adapt<DoadorViewModel>();
            return View(doadorModel);
        }

        // POST: DoadorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverDoador(int id, IFormCollection collection)
        {
            _doadorService.Remover(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
