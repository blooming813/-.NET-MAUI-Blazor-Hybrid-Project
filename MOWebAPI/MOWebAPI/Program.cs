using Microsoft.EntityFrameworkCore;
using MOWebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MedicalOfficeContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MedicalOfficeContext")));

builder.Services.AddControllers();

//builder.Services.AddControllers()
//    .AddJsonOptions(o =>
//    {
//        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MOInitializer.Seed(app);

app.Run();
