using Core.Service;
using Microsoft.AspNetCore.Mvc;
using OndeDoarWeb.Mapper;
using OndeDoarWeb.Models;
using Service;

namespace OndeDoarWeb.Controllers
{
    public class InstituicaoController : Controller
    {
        private readonly IInstituicaoService instituicaoService;

        public InstituicaoController(IInstituicaoService instituicaoService)
        {
            this.instituicaoService = instituicaoService;
        }

        // GET: InstituicaoController?nome=abc
        public ActionResult Index(string? nome)
        {
            var instituicoes = string.IsNullOrWhiteSpace(nome)
                ? instituicaoService.GetAll()
                : instituicaoService.GetByNome(nome.Trim());

            ViewData["Nome"] = nome;
            return View(InstituicaoMapper.ToViewModel(instituicoes));
        }

        // GET: InstituicaoController/Details/5
        public ActionResult Details(int id)
        {
            var instituicao = instituicaoService.Get(id);
            if (instituicao == null)
                return NotFound();

            return View(InstituicaoMapper.ToViewModel(instituicao));
        }

        // GET: InstituicaoController/Create
        public ActionResult Create()
        {
            return View(new InstituicaoViewModel
            {
                HoraAbertura = new TimeOnly(8, 0),
                HoraFechamento = new TimeOnly(17, 0)
            });
        }

        // POST: InstituicaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstituicaoViewModel instituicaoViewModel)
        {
            if (!ModelState.IsValid)
                return View(instituicaoViewModel);

            try
            {
                instituicaoService.Create(InstituicaoMapper.ToEntity(instituicaoViewModel));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(instituicaoViewModel);
            }
        }

        // GET: InstituicaoController/Edit/5
        public ActionResult Edit(int id)
        {
            var instituicao = instituicaoService.Get(id);
            if (instituicao == null)
                return NotFound();

            return View(InstituicaoMapper.ToViewModel(instituicao));
        }

        // POST: InstituicaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InstituicaoViewModel instituicaoViewModel)
        {
            if (!ModelState.IsValid)
                return View(instituicaoViewModel);

            try
            {
                instituicaoService.Edit(InstituicaoMapper.ToEntity(instituicaoViewModel));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(instituicaoViewModel);
            }
        }

        // GET: InstituicaoController/Delete/5
        public ActionResult Delete(int id)
        {
            var instituicao = instituicaoService.Get(id);
            if (instituicao == null)
                return NotFound();

            return View(InstituicaoMapper.ToViewModel(instituicao));
        }

        // POST: InstituicaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(InstituicaoViewModel instituicaoViewModel)
        {
            try
            {
                instituicaoService.Delete(instituicaoViewModel.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                var instituicao = instituicaoService.Get(instituicaoViewModel.Id);
                if (instituicao == null)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(InstituicaoMapper.ToViewModel(instituicao));
            }
        }
    }
}