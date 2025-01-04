using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using url_shortener.api.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option => 
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options
    => options.AddPolicy("wasm", policy
        => policy.WithOrigins(
            [
                builder.Configuration["WebApiUrl"] ?? "https://localhost:7287", 
                builder.Configuration["AppUrl"] ?? "https://localhost:7108"
            ])
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("wasm");
app.MapCarter();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();