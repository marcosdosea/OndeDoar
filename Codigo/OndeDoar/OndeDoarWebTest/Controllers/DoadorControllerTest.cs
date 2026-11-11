using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Models;
using Moq;
using OndeDoarWeb.Controllers;

namespace OndeDoarWebTests.Controllers
{
    [TestClass]
    public class DoadorControllerTests
    {
        private Mock<IDoadorService> mockService = null!;
        private DoadorController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockService = new Mock<IDoadorService>();

            mockService.Setup(service => service.ObterTodos())
                .Returns(GetTestDoadores());

            mockService.Setup(service => service.Obter(1))
                .Returns(GetTestDoador());

            mockService.Setup(service => service.Inserir(It.IsAny<Doador>()));

            mockService.Setup(service => service.Editar(It.IsAny<Doador>()));

            mockService.Setup(service => service.Remover(1));

            controller = new DoadorController(
                mockService.Object
            );
        }

        [TestMethod]
        public void IndexTest()
        {
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<DoadorViewModel>));
        }

        [TestMethod]
        public void DetailsTest()
        {
            var result = controller.Details(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(DoadorViewModel));
        }

        [TestMethod]
        public void CreateTest()
        {
            var result = controller.InserirDoador() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateTest_Valid()
        {
            var doadorModel = GetTestDoadorModel();

            var result = controller.InserirDoador(doadorModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            mockService.Verify(service =>
                service.Inserir(It.IsAny<Doador>()), Times.Once);
        }

        [TestMethod]
        public void CreateTest_Invalid()
        {
            var doadorModel = GetTestDoadorModel();

            controller.ModelState.AddModelError("Nome", "Campo obrigatório");

            var result = controller.InserirDoador(doadorModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            mockService.Verify(service =>
                service.Inserir(It.IsAny<Doador>()), Times.Never);
        }

        [TestMethod]
        public void EditTest_Get_Valid()
        {
            var result = controller.AlterarDoador(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(DoadorViewModel));
        }

        [TestMethod]
        public void EditTest_Post_Valid()
        {
            var doadorModel = GetTestDoadorModel();

            var result = controller.AlterarDoador(1, doadorModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            mockService.Verify(service =>
                service.Editar(It.IsAny<Doador>()), Times.Once);
        }

        [TestMethod]
        public void EditTest_Post_Invalid()
        {
            var doadorModel = GetTestDoadorModel();

            controller.ModelState.AddModelError("Nome", "Campo obrigatório");

            var result = controller.AlterarDoador(1, doadorModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            mockService.Verify(service =>
                service.Editar(It.IsAny<Doador>()), Times.Never);
        }

        [TestMethod]
        public void DeleteTest_Get_Valid()
        {
            var result = controller.RemoverDoador(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(DoadorViewModel));
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            var collection = new FormCollection(
                new Dictionary<string, StringValues>()
            );

            var result = controller.RemoverDoador(1, collection) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            mockService.Verify(service =>
                service.Remover(1), Times.Once);
        }

        private Doador GetTestDoador()
        {
            return new Doador
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "79999999999"
            };
        }

        private DoadorViewModel GetTestDoadorModel()
        {
            return new DoadorViewModel
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "79999999999"
            };
        }

        private List<Doador> GetTestDoadores()
        {
            return new List<Doador>
            {
                GetTestDoador()
            };
        }
    }
}