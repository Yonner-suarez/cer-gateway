using cer_gateway.Aggregates;
using cer_gateway.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuración de Ocelot según el entorno
var env = builder.Environment;
var appsettingsOcelot = env.IsProduction() ? "ocelot.json" : "ocelot.dev.json";

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddJsonFile(appsettingsOcelot, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// 🔹 Ocelot + Aggregators + Handlers
builder.Services.AddOcelot()
    .AddDelegatingHandler<NoGzipDelegatingHandler>(true)
    .AddDelegatingHandler<HeaderDelegatingHandler>()
    .AddTransientDefinedAggregator<ProveedoresAggregates>()
    .AddTransientDefinedAggregator<PresupuestosAggregates>();

builder.Services.ConfigureDownstreamHostAndPortsPlaceholders(builder.Configuration);
builder.Services.DecorateClaimAuthoriser();

// 🔹 Controllers + SwaggerForOcelot
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerForOcelot(builder.Configuration, o =>
{
    o.GenerateDocsForAggregates = true;
});

// 🔹 JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Token:Bearer"] ?? "");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b =>
        b.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());
});

var app = builder.Build();

// 🔹 Middlewares
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.UseSwaggerForOcelotUI(builder.Configuration, opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
})
.UseOcelot()
.Wait();

app.Run();
