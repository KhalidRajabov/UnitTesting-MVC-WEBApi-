using Microsoft.AspNetCore.Mvc;
using Moq;
using MVC_Project.Controllers;
using MVC_Project.Models;
using MVC_Project.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestin.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> _products;

        public ProductControllerTest()
        {
            //commented code means that whatever method of repository is being used,
            //it MUST be mocked, not matter even if only return type is being tested
            //default version is empty ctor, or "MockBehavior.Default"
            //_mockRepo = new Mock<IRepository<Product>>(MockBehavior.Strict);


            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            _products = new List<Product>() 
            {
                new Product {Id=1, Name="PS-5", Color="White",Price=400, Stock=2 },
                new Product {Id=2, Name="PC", Color="Red", Price=600, Stock=120 },
            };
        }


        [Fact]
        public async void Index_ActionWorking_ReturnView()
        {
            var result = await _controller.Index();

            //checking if index method returns the type of {ViewResult}
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnsListProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);
            var result =await _controller.Index();

            //declaring it as variable will help to reach the product that index returns
            var viewResult = Assert.IsType<ViewResult>(result);

            //this will return the 2 products in the constructor
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }


        [Fact]
        public async void Details_Id_IsNull_ReturnsRedirectToIndex()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }


        [Fact]
        public async void Detail_IdIsInvalid_ReturnsNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x=>x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Detail_ValidId_ReturnsProduct(int productId)
        {
            Product product = _products.First(x=>x.Id==productId);
            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }


        [Fact]
        public void Create_ActionExecutes_ReturnsView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async void CreatePOST_InvalidModelState_ReturnsView()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var result = await _controller.Create(_products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }


        [Fact]
        public async void CreatePOST_ValidModelState_ReturnsRedirectToAction()
        {
            var result = await _controller.Create(_products.First());
            var redirect= Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal<string>("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecutes()
        {
            //this object below's value is assigned by the code below
            Product product = null;
            _mockRepo.Setup(repo=>repo.Create(It.IsAny<Product>())).Callback<Product>(repo => product = repo);
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo=>repo.Create(It.IsAny<Product>()), Times.Once);
            Assert.Equal(_products.First().Id, product.Id);
        }



        //testing if create method wont work when any modelstate is wrong
        [Fact]
        public async void CreatePOST_InvalidModelState_CreateNeverExecutes()
        {
            _controller.ModelState.AddModelError("Name", "Name can not be empty");
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
        }


        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result =await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }


        [Theory]
        [InlineData(3)]
        public async void Edit_IdInvalid_ReturnsNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result=await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }


        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnsProduct(int productId)
        {
            var product = _products.First(x=>x.Id==productId);
            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct=Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, _products.First(x=>x.Id== productId));
            var redirect = Assert.IsType<NotFoundResult>(result);
        }


        [Theory]
        [InlineData(1)]
        public void EditPOST_InvalidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = _controller.Edit(productId, _products.First(x=>x.Id== productId));
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _controller.Edit(productId, _products.First(x=>x.Id== productId));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnUpdateMethodExecute(int productId)
        {
            var product = _products.First(x=>x.Id==productId);
            _mockRepo.Setup(repo => repo.Update(product));
            _controller.Edit(productId, product);

            _mockRepo.Verify(repo=>repo.Update(It.IsAny<Product>()), Times.Once);

        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result =await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x=>x.Id== productId);
            _mockRepo.Setup(repo=>repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }


        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteConfirmedMethodExecutes(int productId)
        {
            var product=_products.First(x=>x.Id== productId);
            _mockRepo.Setup(repo => repo.Delete(product));
            await _controller.DeleteConfirmed(productId);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }
    }
}