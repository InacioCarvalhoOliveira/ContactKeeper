// using ContactKeeper.Services.Repositories;// using ContactKeeper.Controllers;
// using ContactKeeper.Data;
// using ContactKeeper.Services.Interfaces;
// using ContactKeeper.Microservices;
// using ContactKeeper.Models;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using NUnit.Framework;

// namespace ContactKeeper.Test;

// [TestFixture]
// public class UserControllerTest
// {

    


    
//     private UserController _controller;
//         private Mock<DataContext> _mockContext;
//         private Mock<IuserRepository> _mockRepository;
//         private Mock<IunitOfWork> _mockIunitOfWork;
//         private Mock<IConfiguration> _mockConfiguration;
//         private Mock<HalfOpenCircuit> _mockCircuitBreakerPolicy;
//     // Uncomment the following line if you need to mock the user repository                                                                                                                                                                                 
//         //private Mock<IuserRepository> _mockUserRepository;

//         [SetUp]
//         public void Setup()
//         {
//             _mockRepository = new Mock<IuserRepository>();
//             _mockContext = new Mock<DataContext>();
//             _mockIunitOfWork = new Mock<IunitOfWork>();
//             _mockConfiguration = new Mock<IConfiguration>();
//             _mockCircuitBreakerPolicy = new Mock<HalfOpenCircuit>();

//             _mockIunitOfWork.Setup(u => u.UserRepository).Returns(_mockRepository.Object);

//             _controller = new UserController(
//                 _mockContext.Object,
//                 _mockRepository.Object,
//                 _mockIunitOfWork.Object,
//                 _mockConfiguration.Object,
//                 _mockCircuitBreakerPolicy.Object
//             );
//         }

//         #region [getallUsers]
//         [Test]
//         public async Task GetAllUsers()
//         {

//             //Arrange
//             var users = new List<User>
//             {
//                 new User{
//                     Password = "password123",
//                     Username = "mudai",
//                     Role = "admin",
//                     // PhoneNumber = new PhoneNumber
//                     // {
//                     //     DDI = "32",
//                     //     DDD = 21,
//                     //     LocalNumber = "11223344"
//                     // }

//                 }
//             };
//             _mockRepository.Setup(repo => repo.GetUsers()).ReturnsAsync(users);
//             var result = await _controller.GetUsers();
//             var okResult = result.Result as OkObjectResult;

//             Assert.That(okResult, Is.Not.Null); //check if okResult is not null
//             Assert.That(okResult.StatusCode, Is.EqualTo(200)); //check if status code is 200

//             var results = okResult.Value as List<User>;
//             Assert.That(results, Is.Not.Null); //check if users is not null
//             Assert.That(results, Is.InstanceOf<List<User>>()); //check if users is instance of List<User>
//         }
//         #endregion

//     #region [getUsersByDdd]
//     [Test]
//     public async Task GetUserByDdd()
//     {
//         //Arrange
//         var users = new List<User>
//         {
//             new User{
//                 Id = 1,
//                 Username = "mudai",
//                 Email = "muda@muda",
//                 PhoneNumber = new PhoneNumber
//                 {
//                     DDI = "32",
//                     DDD = 21,
//                     LocalNumber = "11223344"
//                 }

//             }
//         };

//         _mockRepository.Setup(repo => repo.GetInfoUserByDdd(21)).ReturnsAsync(users);
//         var result = await _controller.GetUsersByDdd(21);
//         var okResult = result as OkObjectResult;

//         Assert.That(okResult, Is.Not.Null); //check if okResult is not null
//         Assert.That(okResult.StatusCode, Is.EqualTo(200)); //check if status code is 200
//         Console.WriteLine(okResult);

//         var results = okResult.Value as List<User>;
//         Assert.That(results, Is.Not.Null); //check if users is not null
//         Assert.That(results, Is.InstanceOf<List<User>>()); //check if users is instance of List<User>

//     }
//     #endregion

//     #region [getUserById]
//     [Test]
//     public async Task GetUserById()
//     {
//         //Arrange
//         var user = new User
//         {
//             Id = 1,
//             Username = "inacio",
//             Email = "inacio.carvalho@gmail.com",
//             PhoneNumber = new PhoneNumber
//             {
//                 DDI = "32",
//                 DDD = 21,
//                 LocalNumber = "11223344"
//             }
//         };        
//         _mockRepository.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
//         var result = await _controller.GetUserById(user.Id);
//         var okResult = result.Result as OkObjectResult;
//         Assert.That(okResult, Is.Not.Null); // Check if okResult is not null
//         var returnedUser = okResult.Value as User;
//         Assert.That(returnedUser, Is.EqualTo(user)); // Verify that the returned user matches the expected user
//         Assert.That(returnedUser, Is.Not.Null); // Check if returned user is not null
//         Assert.That(okResult.Value, Is.InstanceOf<User>()); // Ensure the returned value is of type User
//         Assert.That(okResult.StatusCode, Is.EqualTo(200)); // Check if status code is 200
//         Assert.That(returnedUser.Id, Is.EqualTo(user.Id)); // Verify user ID
//         Assert.That(returnedUser.Username, Is.EqualTo(user.Username)); // Verify username
//         Assert.That(returnedUser.Email, Is.EqualTo(user.Email)); // Verify email
//         Assert.That(returnedUser.PhoneNumber.DDI, Is.EqualTo(user.PhoneNumber.DDI)); // Verify phone number DDI
//         Assert.That(returnedUser.PhoneNumber.DDD, Is.EqualTo(user.PhoneNumber.DDD)); // Verify phone number DDD
//         Assert.That(returnedUser.PhoneNumber.LocalNumber, Is.EqualTo(user.PhoneNumber.LocalNumber)); // Verify phone number local number
//     }
//     #endregion
// }''