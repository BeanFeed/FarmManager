using DAL.Context;
using FarmManagerBackend.Models.Settings;
using FarmManagerBackend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<PrinterService>();

//Automatically populates database
DbContextOptionsBuilder<ManagerContext> mOptions = new DbContextOptionsBuilder<ManagerContext>();

var dbContext = new ManagerContext(mOptions.Options);
dbContext.Database.Migrate();
dbContext.Dispose();
//-------

builder.Services.AddDbContext<ManagerContext>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("Main",
        x =>
        {
            x.WithOrigins(builder.Configuration["Cors:Origin"])
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(bool.Parse(builder.Configuration["UseHttpsRedirection"])) app.UseHttpsRedirection();

app.UseCors("Main");
/*
app.Use((context, next) =>
{
    context.Response.Headers["Access-Control-Allow-Origin"] = builder.Configuration["Cors:Origin"];
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
    context.Response.Headers["Access-Control-Allow-Headers"] = "*";
    context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
    return next();
});
*/



app.MapControllers();

app.Run();