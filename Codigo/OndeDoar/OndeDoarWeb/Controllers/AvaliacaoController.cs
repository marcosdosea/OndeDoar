using Core.Service;
using Microsoft.AspNetCore.Mvc;
using OndeDoarWeb.Mapper;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly IAvaliacaoService avaliacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            this.avaliacaoService = avaliacaoService;
        }

        // GET: AvaliacaoController
        public ActionResult Index()
        {
            var avaliacoes = avaliacaoService.GetAll();
            var avaliacoesViewModel = AvaliacaoMapper.ToViewModel(avaliacoes);
            return View(avaliacoesViewModel);
        }

        // GET: AvaliacaoController/Details/5
        public ActionResult Details(int id)
        {
            var avaliacao = avaliacaoService.Get(id);
            var avaliacaoViewModel = avaliacao != null ? AvaliacaoMapper.ToViewModel(avaliacao) : new AvaliacaoViewModel();
            return View(avaliacaoViewModel);
        }

        // GET: AvaliacaoController/Create
        public ActionResult Create()
        {
            var avaliacaoViewModel = new AvaliacaoViewModel
            {
                Data = DateTime.Now
            };
            return View(avaliacaoViewModel);
        }

        // POST: AvaliacaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AvaliacaoViewModel avaliacaoViewModel)
        {
            if (ModelState.IsValid)
            {
                var avaliacao = AvaliacaoMapper.ToEntity(avaliacaoViewModel);
                avaliacaoService.Create(avaliacao);
                return RedirectToAction(nameof(Index));
            }
            return View(avaliacaoViewModel);
        }

        // GET: AvaliacaoController/Edit/5
        public ActionResult Edit(int id)
        {
            var avaliacao = avaliacaoService.Get(id);
            var avaliacaoViewModel = avaliacao != null ? AvaliacaoMapper.ToViewModel(avaliacao) : new AvaliacaoViewModel();
            return View(avaliacaoViewModel);
        }

        // POST: AvaliacaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AvaliacaoViewModel avaliacaoViewModel)
        {
            if (ModelState.IsValid)
            {
                var avaliacao = AvaliacaoMapper.ToEntity(avaliacaoViewModel);
                avaliacaoService.Edit(avaliacao);
                return RedirectToAction(nameof(Index));
            }
            return View(avaliacaoViewModel);
        }

        // GET: AvaliacaoController/Delete/5
        public ActionResult Delete(int id)
        {
            var avaliacao = avaliacaoService.Get(id);
            var avaliacaoViewModel = avaliacao != null ? AvaliacaoMapper.ToViewModel(avaliacao) : new AvaliacaoViewModel();
            return View(avaliacaoViewModel);
        }

        // POST: AvaliacaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AvaliacaoViewModel avaliacaoViewModel)
        {
            avaliacaoService.Delete(avaliacaoViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}