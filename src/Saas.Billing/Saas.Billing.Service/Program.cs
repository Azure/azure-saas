using Azure.Identity;
using Saas.Billing.Service;
using Saas.Billing.Service.Interfaces;
using Saas.Billing.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (builder.Environment.IsProduction())
{
    // Get Secrets From Azure Key Vault if in production. If not in production, secrets are automatically loaded in from the .NET secrets manager
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0

    // We don't want to fetch all the secrets for the other microservices in the app/solution, so we only fetch the ones with the prefix of "signupadmin-".
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix

    builder.Configuration.AddAzureKeyVault(
        new Uri(builder.Configuration[SR.KeyVaultProperty]),
        new DefaultAzureCredential(),
        new CustomPrefixKeyVaultSecretManager("billing"));
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

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

app.Run();