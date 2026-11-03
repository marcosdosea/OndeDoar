using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using OndeDoarWeb.Controllers;
using System.ComponentModel.DataAnnotations;

namespace OndeDoarWebTests.Controllers
{
    [TestClass()]
    public class DoadorControllerTests
    {
        private static DoadorController controller;
        private static Mock<IDoadorService> mockService;
        private static Mock<IMapper> mockMapper;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            mockService = new Mock<IDoadorService>();
            mockMapper = new Mock<IMapper>();
            mockService.Setup(service => service.ObterTodos()).Returns(GetTestDoadores());
            mockService.Setup(service => service.Obter(1)).Returns(GetTargetDoador());
            mockService.Setup(service => service.Inserir(It.IsAny<Doador>())).Verifiable();
            mockService.Setup(service => service.Editar(It.IsAny<Doador>())).Verifiable();
            mockService.Setup(service => service.Remover(It.IsAny<int>())).Verifiable();
            controller = new DoadorController(
                mockService.Object,
                mockMapper.Object
            );
        }

        [TestMethod()]
        public void IndexTest()
        {
            // Arrange
            var doadores = GetTestDoadores();
            var doadoresModel = GetTestDoadoresModel();
            mockService.Setup(service => service.ObterTodos())
                .Returns(doadores);
            mockMapper.Setup(mapper => mapper.Map<List<DoadorModel>>(
                It.IsAny<List<Doador>>()))
                .Returns(doadoresModel);
            // Act
            var result = controller.Index();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.ViewData.Model);
            Assert.IsInstanceOfType(viewResult.ViewData.Model,typeof(List<DoadorModel>));
            List<DoadorModel> lista = (List<DoadorModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count);
        }
        [TestMethod()]
        public void DetailsTest()
        {
            // Arrange
            var doador = GetTargetDoador();
            var doadorModel = GetTargetDoadorModel();
            mockMapper.Setup(mapper => mapper.Map<DoadorModel>(doador)).Returns(doadorModel);
            // Act
            var result = controller.Details(1);
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(
                viewResult.ViewData.Model,
                typeof(DoadorModel)
            );
            DoadorModel model = (DoadorModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Doador 1", model.Nome);
            Assert.AreEqual("doador1@example.com", model.Email);
            Assert.AreEqual("11999999999", model.Telefone);
        }
        [TestMethod()]
        public void CreateTest()
        {
            // Act
            var result = controller.Create();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod()]
        public void CreateTest_Valid()
        {
            // Arrange
            var doadorModel = GetNewDoadorModel();
            mockMapper.Setup(mapper => mapper.Map<Doador>(doadorModel))
                .Returns(GetNewDoador());
            // Garante que o ModelState esteja válido
            controller.ModelState.Clear();
            // Act
            var result = controller.Create(doadorModel);
            // Assert
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult =
                (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(
                service => service.Inserir(It.IsAny<Doador>()),
                Times.Once
            );
        }
        [TestMethod()]
        public void CreateTest_Invalid()
        {
            // Arrange
            controller.ModelState.AddModelError(
                "Nome",
                "Nome é obrigatório."
            );
            // Act
            var result = controller.Create(GetNewDoadorModel());
            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(
                result,
                typeof(RedirectToActionResult)
            );
            RedirectToActionResult redirectToActionResult =
                (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(
                service => service.Inserir(It.IsAny<Doador>()),
                Times.Never
            );
        }
        [TestMethod()]
        public void EditTest_Get_Valid()
        {
            // Arrange
            var doador = GetTargetDoador();
            var doadorModel = GetTargetDoadorModel();
            mockService.Setup(service => service.Obter(1)).Returns(doador);
            mockMapper.Setup(mapper => mapper.Map<DoadorModel>(doador)).Returns(doadorModel);
            // Act
            var result = controller.Edit(1);
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(
                viewResult.ViewData.Model,
                typeof(DoadorModel)
            );
            DoadorModel model = (DoadorModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Doador 1", model.Nome);
            Assert.AreEqual("doador1@example.com", model.Email);
            Assert.AreEqual("11999999999", model.Telefone);
        }
        [TestMethod()]
        public void EditTest_Post_Valid()
        {
            // Arrange
            var doadorModel = GetNewDoadorModel();
            var doador = GetNewDoador();
            mockMapper.Setup(mapper => mapper.Map<Doador>(doadorModel))
                .Returns(doador);
            // Act
            var result = controller.Edit(doadorModel.Id, doadorModel);
            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(RedirectToActionResult)
            );

            RedirectToActionResult redirectToActionResult =
                (RedirectToActionResult)result;

            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(
                service => service.Editar(doador),
                Times.Once
            );
        }
        [TestMethod()]
        public void EditTest_Post_Invalid()
        {
            // Arrange
            controller.ModelState.AddModelError(
                "Nome",
                "Nome é obrigatório."
            );
            // Act
            var result = controller.Edit(1, GetNewDoadorModel());
            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(
                result,
                typeof(RedirectToActionResult)
            );
            RedirectToActionResult redirectToActionResult =
                (RedirectToActionResult)result;

            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(
                service => service.Editar(It.IsAny<Doador>()),
                Times.Never
            );
        }
        [TestMethod()]
        public void DeleteTest_Get_Valid()
        {
            // Arrange
            var doador = GetTargetDoador();
            var doadorModel = GetTargetDoadorModel();

            mockService.Setup(service => service.Obter(1)) .Returns(doador);
            mockMapper.Setup(mapper => mapper.Map<DoadorModel>(doador)) .Returns(doadorModel);
            // Act
            var result = controller.Delete(1);
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(
                viewResult.ViewData.Model,
                typeof(DoadorModel)
            );
            DoadorModel model = (DoadorModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Doador 1", model.Nome);
            Assert.AreEqual("doador1@example.com", model.Email);
            Assert.AreEqual("11999999999", model.Telefone);
        }
        [TestMethod()]
        public void DeleteTest_Post_Valid()
        {
            // Arrange
            var form = new FormCollection(
                new Dictionary<string, StringValues>()
            );
            // Act
            var result = controller.Delete(1, form);
            // Assert
            Assert.IsInstanceOfType(
                result,
                typeof(RedirectToActionResult)
            );
            RedirectToActionResult redirectToActionResult =
                (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(
                service => service.Remover(1),
                Times.Once
            );
        }
        private static Doador GetNewDoador()
        {
            return new Doador
            {
                Id = 2,
                Nome = "Novo Doador",
                Email = "novo@example.com",
                Telefone = "11888888888"
            };
        }
        private static DoadorModel GetNewDoadorModel()
        {
            return new DoadorModel
            {
                Id = 2,
                Nome = "Novo Doador",
                Email = "novo@example.com",
                Telefone = "11888888888"
            };
        }
        private static Doador GetTargetDoador()
        {
            return new Doador
            {
                Id = 1,
                Nome = "Doador 1",
                Email = "doador1@example.com",
                Telefone = "11999999999"
            };
        }
        private static DoadorModel GetTargetDoadorModel()
        {
            return new DoadorModel
            {
                Id = 1,
                Nome = "Doador 1",
                Email = "doador1@example.com",
                Telefone = "11999999999"
            };
        }
        private static List<Doador> GetTestDoadores()
        {
            return new List<Doador>
            {
                new Doador
                {
                    Id = 1,
                    Nome = "Doador 1",
                    Email = "doador1@example.com",
                    Telefone = "11999999999"
                },
                new Doador
                {
                    Id = 2,
                    Nome = "Doador 2",
                    Email = "doador2@example.com",
                    Telefone = "11888888888"
                },
                new Doador
                {
                    Id = 3,
                    Nome = "Doador 3",
                    Email = "doador3@example.com",
                    Telefone = "11777777777"
                }
            };
        }
        private static List<DoadorModel> GetTestDoadoresModel()
        {
            return new List<DoadorModel>
            {
                new DoadorModel
                {
                    Id = 1,
                    Nome = "Doador 1",
                    Email = "doador1@example.com",
                    Telefone = "11999999999"
                },
                new DoadorModel
                {
                    Id = 2,
                    Nome = "Doador 2",
                    Email = "doador2@example.com",
                    Telefone = "11888888888"
                },
                new DoadorModel
                {
                    Id = 3,
                    Nome = "Doador 3",
                    Email = "doador3@example.com",
                    Telefone = "11777777777"
                }
            };
        }
    }
}
