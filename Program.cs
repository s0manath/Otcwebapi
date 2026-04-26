using OTC.Api.Services;
using OTC.Api.Models;
using OTC.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// -------------------- SERVICES --------------------
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// -------------------- PIPELINE --------------------

// If hosting under IIS virtual directory
app.UsePathBase("/LMSOTCWEBAPP");

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// -------------------- SWAGGER --------------------
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/LMSOTCWEBAPP/swagger/v1/swagger.json", "OTC API v1");
    c.RoutePrefix = "swagger";
});

// -------------------- RUN --------------------
app.Run();