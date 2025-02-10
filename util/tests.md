dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk

dotnet new nunit -o ContactKeeper.Test
dotnet add ContactKeeper.Test/ContactKeeper.Test.csproj reference ../ContactKeeper/ContactKeeper.csproj


###
using ContactKeeper.Contracts;
using ContactKeeper.Controllers;
using NUnit.Framework;
using Moq; // Certifique-se de incluir essa linha
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContactKeeper.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ContactKeeper.Interfaces;
using ContactKeeper.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactKeeper.Test.Controllers
{
  [TestFixture]
    public class TestUserControllerTest
    {
        private UserController _userController;
        private Mock<IuserRepository> _mockUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IuserRepository>();
            _userController = new UserController(_mockUserRepository.Object);
        }

        [Test]
        public async Task GetAllUsers_ReturnsOk()
        {
            // Arrange
            var users = new List<User> 
            { 
                new User 
                { 
                    Id = 1, 
                    Username = "John Doe", 
                    Email = "inacio.test@gmail.com", 
                    PhoneNumber = new PhoneNumber 
                    {
                        DDI = "+55",
                        DDD = 12,
                        LocalNumber = "12345678"
                    } 
                }
            };
            
            _mockUserRepository.Setup(repo => repo.GetUsers()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.EqualTo(users));
        }
    

        // [Test]
        // public async Task GetAllUsers_ReturnsNotFound_WhenNoUsers()
        // {
        //     // Arrange
        //     _mockUserRepository.Setup(repo => repo.GetUsers()).Returns(Task.FromResult<IEnumerable<User>>(null));

        //     // Act
        //     var result = _userController.GetUsers();

        //     // Assert
        //     Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        // }

        // [Test]
        // public async Task GetUserById_ReturnsOk_WhenUserExists()
        // {
        //     // Arrange
        //     var user = new User { Id = 1 };
        //     _mockUserRepository.Setup(repo => repo.GetUserById(1)).Returns(Task.FromResult<User>(user));

        //     // Act
        //     var result = await _userController.GetUserById(1);

        //     // Assert
        //     Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        //     var okResult = result.Result as OkObjectResult;
        //     Assert.That(okResult, Is.Not.Null);
        //     var returnedUser = okResult.Value as User;
        //     Assert.That(returnedUser, Is.Not.Null); 
        //     Assert.That(returnedUser.Id, Is.EqualTo(user.Id));
        
           
        // }

        // [Test]
        // public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        // {
        //     // Arrange
        //     _mockUserRepository.Setup(repo => repo.GetUserById(5)).Returns(Task.FromResult<User>(null));

        //     // Act
        //     var result = await _userController.GetUserById(5);

        //     // Assert
        //     Assert.That(result.Result,Is.InstanceOf<NotFoundResult>());
        // }
    }
}
###