using DAL.Context;
using FarmManagerBackend.Models.Settings;
using FarmManagerBackend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("DatabaseConfiguration"));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<PrinterService>();

//Automatically populates database
DbContextOptionsBuilder<ManagerContext> mOptions = new DbContextOptionsBuilder<ManagerContext>().UseMySql(
    $"server={builder.Configuration["DatabaseConfiguration:ServerAddress"]},{builder.Configuration["DatabaseConfiguration:Port"]};database={builder.Configuration["DatabaseConfiguration:Database"]};user={builder.Configuration["DatabaseConfiguration:User"]};password={builder.Configuration["DatabaseConfiguration:Password"]}",
    ServerVersion.AutoDetect(
        $"server={builder.Configuration["DatabaseConfiguration:ServerAddress"]},{builder.Configuration["DatabaseConfiguration:Port"]};database={builder.Configuration["DatabaseConfiguration:Database"]};user={builder.Configuration["DatabaseConfiguration:User"]};password={builder.Configuration["DatabaseConfiguration:Password"]}"));

var dbContext = new ManagerContext(mOptions.Options);
dbContext.Database.Migrate();
dbContext.Dispose();
//-------

builder.Services.AddDbContext<ManagerContext>(options =>
    options.UseMySql($"server={builder.Configuration["DatabaseConfiguration:ServerAddress"]},{builder.Configuration["DatabaseConfiguration:Port"]};database={builder.Configuration["DatabaseConfiguration:Database"]};user={builder.Configuration["DatabaseConfiguration:User"]};password={builder.Configuration["DatabaseConfiguration:Password"]}",
    ServerVersion.AutoDetect($"server={builder.Configuration["DatabaseConfiguration:ServerAddress"]},{builder.Configuration["DatabaseConfiguration:Port"]};database={builder.Configuration["DatabaseConfiguration:Database"]};user={builder.Configuration["DatabaseConfiguration:User"]};password={builder.Configuration["DatabaseConfiguration:Password"]}"),a => a.EnableRetryOnFailure(10)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Main",
        x =>
        {
            x.WithOrigins(builder.Configuration["Cors:Origin"])
                .WithMethods("GET","POST","PUT","DELETE","OPTIONS")
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

//app.UseHttpsRedirection();

app.UseCors("Main");

app.MapControllers();

app.Run();