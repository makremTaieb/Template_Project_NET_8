using APIs_Signature_DigiGO.Iservices;
using APIs_Signature_DigiGO.Services;
using common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddHealthChecks();

// Ajouter HttpClientFactory pour gérer les instances HttpClient efficacement.
builder.Services.AddHttpClient();

builder.Services.AddControllers();

// Enregistrer les services et leurs interfaces.
// Singleton pour TokenService afin de gérer la mise en cache du jeton.
builder.Services.AddSingleton<ITokenService, TokenService>();
// Scoped pour LegalisationService car il dépend de HttpClientFactory.
builder.Services.AddScoped<ILegalisationService, LegalisationService>();



// Ajouter le nouveau service pour l'API Graph
builder.Services.AddSingleton<IGraphApiService, GraphApiService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHealthChecks("/hc");

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Une erreur inattendue est survenue.", details = ex.Message });
    }
});
app.UseAuthorization();

app.MapControllers();

app.Run();
