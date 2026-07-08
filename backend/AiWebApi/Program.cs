using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using AiWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddPolicy("VueCors", policy =>
{
    policy.WithOrigins("http://localhost:5173")
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

builder.Configuration.AddUserSecrets<Program>();
var geminiKey = builder.Configuration["Gemini:ApiKey"];

var kernelBuilder = builder.Services.AddKernel();

#pragma warning disable SKEXP0070 // Suppress the experimental warning explicitly
kernelBuilder.AddGoogleAIGeminiChatCompletion(
    modelId: "gemini-2.5-flash",
    apiKey: geminiKey
);
#pragma warning restore SKEXP0070

builder.Services.AddScoped<IChatService, GeminiChatService>();

var app = builder.Build();

app.UseRouting();
app.UseCors("VueCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
