using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllersWithViews();

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

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the token endpoint.
                options.SetTokenEndpointUris("connect/token");

                // Enable the client credentials flow.
                options.AllowClientCredentialsFlow();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableTokenEndpointPassthrough();
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

builder.Services.AddHostedService<Worker>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



//using Microsoft.EntityFrameworkCore;
//using OpenIddict.Abstractions;
//using Server.Data;
//using static OpenIddict.Abstractions.OpenIddictConstants;

//var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration["ConnectionStrings:OpenIddict"]);
//    options.UseOpenIddict();
//});

//builder.Services.AddOpenIddict()
//    .AddCore(options =>
//    {
//        options.UseEntityFrameworkCore()
//                .UseDbContext<ApplicationDbContext>();
//    })
//    .AddServer(options =>
//    {
//        options.SetTokenEndpointUris("connect/token");

//        options.AllowClientCredentialsFlow();

//        options.AddDevelopmentEncryptionCertificate()
//                .AddDevelopmentSigningCertificate();

//        options.UseAspNetCore()
//                .EnableTokenEndpointPassthrough();
//    })
//    .AddValidation(options =>
//    {
//        options.UseLocalServer();
//        options.UseAspNetCore();
//    });


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();


//    using var scope = app.Services.CreateScope();

//    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    await context.Database.EnsureCreatedAsync();

//    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

//    if (await manager.FindByClientIdAsync("console") == null)
//    {
//        await manager.CreateAsync(new OpenIddictApplicationDescriptor
//        {
//            ClientId = "console",
//            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
//            DisplayName = "My client application",
//            Permissions =
//                {
//                    Permissions.Endpoints.Token,
//                    Permissions.GrantTypes.ClientCredentials
//                }
//        });
//    }

//    if (await manager.FindByClientIdAsync("mvc") is null)
//    {
//        await manager.CreateAsync(new OpenIddictApplicationDescriptor
//        {
//            ClientId = "mvc",
//            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
//            //ConsentType = ConsentTypes.Explicit,
//            DisplayName = "MVC client application",
//            //RedirectUris =
//            //{
//            //    new Uri("https://localhost:7107/callback/login/local")
//            //},
//            //PostLogoutRedirectUris =
//            //{
//            //    new Uri("https://localhost:7107/callback/logout/local")
//            //},
//            //Permissions =
//            //{
//            //    Permissions.Endpoints.Authorization,
//            //    Permissions.Endpoints.Logout,
//            //    Permissions.Endpoints.Token,
//            //    Permissions.GrantTypes.AuthorizationCode,
//            //    Permissions.ResponseTypes.Code,
//            //    Permissions.Scopes.Email,
//            //    Permissions.Scopes.Profile,
//            //    Permissions.Scopes.Roles
//            //},
//            //Requirements =
//            //{
//            //    Requirements.Features.ProofKeyForCodeExchange
//            //}
//            Permissions =
//            {
//                Permissions.Endpoints.Token,
//                Permissions.GrantTypes.ClientCredentials
//            }
//        });
//    }
//}

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();
