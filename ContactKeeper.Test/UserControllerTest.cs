using ContactKeeper.Contracts;
using ContactKeeper.Controllers;
using ContactKeeper.Data;
using ContactKeeper.Interfaces;
using ContactKeeper.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ContactKeeper.Test;

[TestFixture]
public class UserControllerTest
{
    private UserController _controller;
    private Mock<DataContext> _mockContext;
    private Mock<IuserRepository> _mockRepository;
    private Mock<IunitOfWork> _mockIunitOfWork;
    //private Mock<IuserRepository> _mockUserRepository;
    
    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IuserRepository>();
        _mockContext = new Mock<DataContext>();
        _mockIunitOfWork = new Mock<IunitOfWork>();
       
        _mockIunitOfWork.Setup(u => u.UserRepository).Returns(_mockRepository.Object);

        _controller = new UserController(_mockContext.Object, _mockRepository.Object, _mockIunitOfWork.Object);
    }
    #region [getallUsers]   
    [Test]
    public async Task GetAllUsers()
    {

        //Arrange
        var users = new List<User>
        {
            new User{
                Id = 1,
                Username = "mudai",
                Email = "muda@muda",
                PhoneNumber = new PhoneNumber
                {
                    DDI = "32",
                    DDD = 21,
                    LocalNumber = "11223344"
                }

            }
        };
        _mockRepository.Setup(repo => repo.GetUsers()).ReturnsAsync(users);
        var result = await _controller.GetUsers();
        var okResult = result.Result as OkObjectResult;
        
        Assert.That(okResult, Is.Not.Null); //check if okResult is not null
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); //check if status code is 200
        
        var results = okResult.Value as List<User>;
        Assert.That(results, Is.Not.Null); //check if users is not null
        Assert.That(results, Is.InstanceOf<List<User>>()); //check if users is instance of List<User>
    }
    #endregion
    #region [getUsersByDdd]
    [Test]
    public async Task GetUserByDdd()
    {
        //Arrange
        var users = new List<User>
        {
            new User{
                Id = 1,
                Username = "mudai",
                Email = "muda@muda",
                PhoneNumber = new PhoneNumber
                {
                    DDI = "32",
                    DDD = 21,
                    LocalNumber = "11223344"
                }

            }
        };
        _mockRepository.Setup(repo => repo.GetInfoUserByDdd(21)).ReturnsAsync(users);
        var result = await _controller.GetUsersByDdd(21);
        var okResult = result as OkObjectResult;
        
        Assert.That(okResult, Is.Not.Null); //check if okResult is not null
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); //check if status code is 200
        Console.WriteLine(okResult);

        var results = okResult.Value as List<User>;
        Assert.That(results, Is.Not.Null); //check if users is not null
        Assert.That(results, Is.InstanceOf<List<User>>()); //check if users is instance of List<User>
    }
    #endregion
}