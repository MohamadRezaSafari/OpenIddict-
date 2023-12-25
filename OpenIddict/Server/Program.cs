using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Polly;
using Server.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:OpenIddict"]);
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
                .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("connect/token");

        options.AllowClientCredentialsFlow();

        options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
                .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await manager.FindByClientIdAsync("mvc") is null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "mvc",
            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
            ConsentType = ConsentTypes.Explicit,
            DisplayName = "MVC client application",
            RedirectUris =
            {
                new Uri("https://localhost:44338/callback/login/local")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:44338/callback/logout/local")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        });
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
