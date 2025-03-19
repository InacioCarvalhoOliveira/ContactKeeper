// using ContactKeeper.Controller;
// using ContactKeeper.Data;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using NUnit.Framework;

// namespace ContactKeeper.Test
// {
//     [TestFixture]
//     public class HealthControllerTest
//     {
//         private DataContext _context;
//         private HealthController _controller;

//         [SetUp]
//         public void Setup()
//         {
//             // Configura o DataContext para usar um banco de dados em memória
//             var options = new DbContextOptionsBuilder<DataContext>()
//                 .UseInMemoryDatabase(databaseName: "TestDatabase")
//                 .Options;

//             _context = new DataContext(options);

//             // Inicializa o controlador com o DataContext
//             _controller = new HealthController(_context);
//         }

//         [Test]
//         public void HealthDataBase_ReturnsOk_WhenDatabaseIsConnected()
//         {
//             // Act
//             var result = _controller.HealthDataBase().Result; // Obtém o resultado síncrono
//             var okResult = result as OkObjectResult;

//             // Assert
//             Assert.That(okResult, Is.Null);
//             Assert.That(okResult.StatusCode, Is.EqualTo(200));
//             Assert.That(okResult.Value.ToString(), Does.Contain("connected at"));
//         }

//         [Test]
//         public void HealthDataBase_ReturnsBadRequest_WhenDatabaseCannotConnect()
//         {
//             // Simula falha na conexão com o banco de dados
//             _context.Database.EnsureDeleted();

//             // Act
//             var result = _controller.HealthDataBase().Result; // Obtém o resultado síncrono
//             var badRequestResult = result as BadRequestObjectResult;

//             // Assert
//             Assert.That(badRequestResult, Is.Not.Null);
//             Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // BadRequest geralmente retorna 400
//             Assert.That(badRequestResult.Value.ToString(), Does.Contain("Connection to the database is not working"));
//         }
//     }
// }