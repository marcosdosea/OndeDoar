using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace OndeDoarWeb.Controllers
{
    public class DoadorController : Controller
    {
        IDoadorService _doadorService;
        IMapper _mapper;

        public DoadorController(IDoadorService doadorService, IMapper mapper)
        {
            _doadorService = doadorService;
            _mapper = mapper;
        }


        // GET: DoadorController
        public ActionResult Index()
        {
            var listarDoadores = _doadorService.ObterTodos();
            var listarDoadoresModel = _mapper.Map<List<DoadorModel>>(listarDoadores);
            return View(listarDoadoresModel);
        }

        // GET: DoadorController/Details/5
        public ActionResult Details(int id)
        {
            Doador doador = _doadorService.Obter(id);
            DoadorModel doadorModel = _mapper.Map<DoadorModel>(doador);
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
        public ActionResult InserirDoador(DoadorModel doadorModel)
        {
            if (ModelState.IsValid)
            {
                var doador = _mapper.Map<Doador>(doadorModel);
                _doadorService.Inserir(doador);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DoadorController/Edit/5
        public ActionResult AlterarDoador(int id)
        {
            Doador doador = _doadorService.Obter(id);
            DoadorModel doadorModel = _mapper.Map<DoadorModel>(doador);
            return View(doadorModel);
        }

        // POST: DoadorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarDoador(int id, DoadorModel doadorModel)
        {
           if (ModelState.IsValid)
            {
                var doador = _mapper.Map<Doador>(doadorModel);
                _doadorService.Editar(doador);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DoadorController/Delete/5
        public ActionResult RemoverDoador(int id)
        {
            Doador doador = _doadorService.Obter(id);
            DoadorModel doadorModel = _mapper.Map<DoadorModel>(doador);
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
