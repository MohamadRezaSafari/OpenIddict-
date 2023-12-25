using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;

Console.WriteLine("Hi");

var services = new ServiceCollection();

services.AddOpenIddict()

    // Register the OpenIddict client components.
    .AddClient(options =>
    {
        // Allow grant_type=client_credentials to be negotiated.
        options.AllowClientCredentialsFlow();

        // Disable token storage, which is not necessary for non-interactive flows like
        // grant_type=password, grant_type=client_credentials or grant_type=refresh_token.
        options.DisableTokenStorage();

        // Register the System.Net.Http integration and use the identity of the current
        // assembly as a more specific user agent, which can be useful when dealing with
        // providers that use the user agent as a way to throttle requests (e.g Reddit).
        options.UseSystemNetHttp()
               .SetProductInformation(typeof(Program).Assembly);

        // Add a client registration matching the client application definition in the server project.
        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:44302/", UriKind.Absolute),

            ClientId = "console",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207"
        });
        //options.AddRegistration(new OpenIddictClientRegistration
        //{
        //    Issuer = new Uri("https://localhost:7107/", UriKind.Absolute),
        //    ClientId = "mvc",
        //    ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
        //    Scopes = { Scopes.Email, Scopes.Profile },
        //    RedirectUri = new Uri("callback/login/local", UriKind.Relative),
        //    PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
        //});
    });

await using var provider = services.BuildServiceProvider();

var token = await GetTokenAsync(provider);
Console.WriteLine("Access token: {0}", token);
Console.WriteLine();


static async Task<string> GetTokenAsync(IServiceProvider provider)
{
    var service = provider.GetRequiredService<OpenIddictClientService>();

    var result = await service.AuthenticateWithClientCredentialsAsync(new());
    return result.AccessToken;
}

//var host = new HostBuilder()
//    .ConfigureLogging(options => options.AddDebug())
//    .ConfigureServices(services =>
//    {
//        services.AddDbContext<DbContext>(options =>
//        {
//            //options.UseSqlServer(@"");
//            options.UseOpenIddict();
//        });

//        services.AddOpenIddict()

//            // Register the OpenIddict core components.
//            .AddCore(options =>
//            {
//                // Configure OpenIddict to use the Entity Framework Core stores and models.
//                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
//                options.UseEntityFrameworkCore()
//                       .UseDbContext<DbContext>();
//            })

//            // Register the OpenIddict client components.
//            .AddClient(options =>
//            {
//                // Note: this sample uses the authorization code flow,
//                // but you can enable the other flows if necessary.
//                options.AllowAuthorizationCodeFlow()
//                       .AllowRefreshTokenFlow();

//                // Register the signing and encryption credentials used to protect
//                // sensitive data like the state tokens produced by OpenIddict.
//                options.AddDevelopmentEncryptionCertificate()
//                       .AddDevelopmentSigningCertificate();

//                // Add the operating system integration.
//                options.UseSystemIntegration()
//                       .SetAllowedEmbeddedWebServerPorts(7890);

//                // Register the System.Net.Http integration and use the identity of the current
//                // assembly as a more specific user agent, which can be useful when dealing with
//                // providers that use the user agent as a way to throttle requests (e.g Reddit).
//                options.UseSystemNetHttp()
//                       .SetProductInformation(typeof(Program).Assembly);

//                // Add a client registration matching the client application definition in the server project.
//                //options.AddRegistration(new OpenIddictClientRegistration
//                //{
//                //    Issuer = new Uri("https://localhost:44319/", UriKind.Absolute),

//                //    ClientId = "console_app",
//                //    RedirectUri = new Uri("http://localhost:7890/", UriKind.Absolute)
//                //});
//                options.AddRegistration(new OpenIddictClientRegistration
//                {
//                    Issuer = new Uri("https://localhost:7107/", UriKind.Absolute),
//                    ClientId = "mvc",
//                    ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
//                    Scopes = { Scopes.Email, Scopes.Profile },
//                    RedirectUri = new Uri("callback/login/local", UriKind.Relative),
//                    PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
//                });
//            });

//        // Register the worker responsible for creating the database used to store tokens
//        // and adding the registry entries required to register the custom URI scheme.
//        //
//        // Note: in a real world application, this step should be part of a setup script.
//        //services.AddHostedService<Worker>();

//        // Register the background service responsible for handling the console interactions.
//        services.AddHostedService<InteractiveService>();
//    })
//    .UseConsoleLifetime()
//    .Build();

//await host.RunAsync();
