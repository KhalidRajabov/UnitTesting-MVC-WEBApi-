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


    }
}
