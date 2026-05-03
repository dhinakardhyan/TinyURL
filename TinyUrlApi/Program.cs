using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReact",
//        policy => policy
//            .WithOrigins("http://localhost:5173")
//            .AllowAnyMethod()
//            .AllowAnyHeader());
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy
            .WithOrigins(
                "http://localhost:5173",
                "https://zealous-dune-036b78200.7.azurestaticapps.net"
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer("Server=.;Database=TinyUrlDb;Trusted_Connection=True;TrustServerCertificate=True"));


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tinyurl.db"));


var app = builder.Build();




// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthorization();

app.MapControllers();

app.Run();