using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using sisprenic.Database;
using sisprenic.Extensions;

using sisprenic_backend.Authorization;

using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

string[]? allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddAuthorization()
        .AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();


builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SisprenicContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins",
                          policy =>
                          {
                              policy.WithOrigins(allowedOrigins)
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .AllowCredentials();
                          });
});

var app = builder.Build();
app.MapIdentityApi<IdentityUser>().WithTags("Authentication");

// Map application endpoints
app.MapEndpoints();

await app.MigrateDbAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.UseHttpsRedirection();

app.UseCors("AllowOrigins");
app.Run();
