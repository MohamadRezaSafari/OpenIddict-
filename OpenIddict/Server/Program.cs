using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

#region API
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
# endregion

# region MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
#endregion

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:OpenIddict"]);
    options.UseOpenIddict();
});

#region MVC

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
                .UseDbContext<ApplicationDbContext>();
    })
    .AddClient(options =>
    {
        options.AllowAuthorizationCodeFlow();

        options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
                .EnableStatusCodePagesIntegration()
                .EnableRedirectionEndpointPassthrough();

        options.UseSystemNetHttp()
                .SetProductInformation(typeof(Program).Assembly);

        options.UseWebProviders()
                .AddGitHub(options =>
                {
                    options.SetClientId("c4ade52327b01ddacff3")
                            .SetClientSecret("da6bed851b75e317bf6b2cb67013679d9467c122")
                            .SetRedirectUri("callback/login/github");
                });
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("connect/authorize")
                .SetLogoutEndpointUris("connect/logout")
                .SetTokenEndpointUris("connect/token")
                .SetUserinfoEndpointUris("connect/userinfo");

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        options.AllowAuthorizationCodeFlow();

        options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableTokenEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough()
                .EnableStatusCodePagesIntegration();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });


builder.Services.AddHostedService<Worker>();


#endregion


#region API 
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
#endregion

builder.Services.AddHostedService<Worker>();


var app = builder.Build();

#region API
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.MapControllers();
#endregion

#region MVC
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseStatusCodePagesWithReExecute("~/error");
}

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.UseStaticFiles();
app.UseRouting();

#endregion

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

