using OTC.Api.Services;
using OTC.Api.Models;
using OTC.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILoginMasterService, LoginMasterService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IRoleMasterService, RoleMasterService>();
builder.Services.AddScoped<IRegionalMasterService, RegionalMasterService>();
builder.Services.AddScoped<IAtmBulkUploadService, AtmBulkUploadService>();
builder.Services.AddScoped<IAdminMasterService, AdminMasterService>();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");
// app.UseMiddleware<EncryptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
