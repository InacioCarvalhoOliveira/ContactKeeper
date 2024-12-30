// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Hosting;
// using ContactKeeper.API.Data;
// using Microsoft.IdentityModel.Tokens;

// namespace ContactKeeper.API
// {
//     public class Startup(IConfiguration configuration)
//     {
//         public IConfiguration Configuration { get; } = configuration;

//         public void ConfigureServices(IServiceCollection services)
//         {
//             services.AddControllers().
//             AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });
//             services.AddScoped<DataContext, DataContext>();
//             services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
//             var key = System.Text.Encoding.ASCII.GetBytes(Settings.Secret);
           
//             // Add Swagger for API documentation
//             services.AddSwaggerGen(c =>
//             {
//                 c.SwaggerDoc("v1", new() { Title = "ContactKeeper.API", Version = "v1" });
                              
//             });
            
//         }

//         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//         {
//             if (env.IsDevelopment())
//             {
//                 app.UseDeveloperExceptionPage();
//             }
//             else
//             {
//                 app.UseExceptionHandler("/Home/Error");
//                 app.UseHsts();
//             }

//             app.UseSwagger();
//             app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactKeeper.API"));

//             app.UseHttpsRedirection();
//             app.UseStaticFiles();

//             app.UseRouting();

//             app.UseAuthentication();
//             app.UseAuthorization();

//             app.Use(async (context, next) =>
//             {
//                 Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
                
//                 await next.Invoke();

//                 var headers = context.Response.Headers;
//                 Console.WriteLine($"Response Headers: {string.Join(", ", headers.Keys)}");
//             });

//             app.UseEndpoints(endpoints =>
//             {
//                 endpoints.MapControllers(); 
//             });
//         }
//     }
// }