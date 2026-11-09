using Core.Service;
using Microsoft.AspNetCore.Mvc;
using OndeDoarWeb.Mapper;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Controllers
{
    public class PostagemController : Controller
    {
        private readonly IPostagemService postagemService;

        public PostagemController(IPostagemService postagemService)
        {
            this.postagemService = postagemService;
        }

        // GET: PostagemController
        public ActionResult Index()
        {
            var postagens = postagemService.GetAll();
            var postagensViewModel = PostagemMapper.ToViewModel(postagens);
            return View(postagensViewModel);
        }

        // GET: PostagemController/Details/5
        public ActionResult Details(uint id)
        {
            var postagem = postagemService.Get(id);
            var postagemViewModel = postagem != null ? PostagemMapper.ToViewModel(postagem) : new PostagemViewModel();
            return View(postagemViewModel);
        }

        // GET: PostagemController/Create
        public ActionResult Create()
        {
            var postagemViewModel = new PostagemViewModel
            {
                Data = DateTime.Now
            };
            return View(postagemViewModel);
        }

        // POST: PostagemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PostagemViewModel postagemViewModel)
        {
            if (ModelState.IsValid)
            {
                var postagem = PostagemMapper.ToEntity(postagemViewModel);
                postagemService.Create(postagem);
                return RedirectToAction(nameof(Index));
            }
            return View(postagemViewModel);
        }

        // GET: PostagemController/Edit/5
        public ActionResult Edit(uint id)
        {
            var postagem = postagemService.Get(id);
            var postagemViewModel = postagem != null ? PostagemMapper.ToViewModel(postagem) : new PostagemViewModel();
            return View(postagemViewModel);
        }

        // POST: PostagemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostagemViewModel postagemViewModel)
        {
            if (ModelState.IsValid)
            {
                var postagem = PostagemMapper.ToEntity(postagemViewModel);
                postagemService.Edit(postagem);
                return RedirectToAction(nameof(Index));
            }
            return View(postagemViewModel);
        }

        // GET: PostagemController/Delete/5
        public ActionResult Delete(uint id)
        {
            var postagem = postagemService.Get(id);
            var postagemViewModel = postagem != null ? PostagemMapper.ToViewModel(postagem) : new PostagemViewModel();
            return View(postagemViewModel);
        }

        // POST: PostagemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PostagemViewModel postagemViewModel)
        {
            postagemService.Delete(postagemViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}