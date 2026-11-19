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
                var id = instituicaoService.Create(InstituicaoMapper.ToEntity(instituicaoViewModel));
                return RedirectToAction(nameof(Verificacao), new { id });
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(instituicaoViewModel);
            }
        }

        // GET: InstituicaoController/Verificacao/5
        public ActionResult Verificacao(int id)
        {
            var instituicao = instituicaoService.Get(id);
            if (instituicao == null)
                return NotFound();

            return View(new VerificacaoViewModel { IdInstituicao = id });
        }

        // POST: InstituicaoController/Verificacao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verificacao(VerificacaoViewModel verificacaoViewModel)
        {
            if (!ModelState.IsValid)
                return View(verificacaoViewModel);

            return RedirectToAction(nameof(PerfilPublico), new { id = verificacaoViewModel.IdInstituicao });
        }

        // GET: InstituicaoController/PerfilPublico/5
        public ActionResult PerfilPublico(int id)
        {
            var instituicao = instituicaoService.Get(id);
            if (instituicao == null)
                return NotFound();

            return View(new PerfilPublicoViewModel
            {
                IdInstituicao = id,
                NomeExibido = instituicao.NomeFantasia,
                Cidade = instituicao.Cidade,
                Estado = instituicao.Estado
            });
        }

        // POST: InstituicaoController/PerfilPublico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerfilPublico(PerfilPublicoViewModel perfilPublicoViewModel)
        {
            if (!ModelState.IsValid)
                return View(perfilPublicoViewModel);

            return RedirectToAction(nameof(Index));
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