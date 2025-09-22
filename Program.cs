using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// SQLite connection
var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Data Source=homefinder.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));



var app = builder.Build();

app.UseCors("ui");
app.UseStaticFiles(); // serves wwwroot (for uploaded photos)


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
