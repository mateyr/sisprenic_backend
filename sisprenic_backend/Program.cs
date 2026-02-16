using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using sisprenic.Database;
using sisprenic.Extensions;

using sisprenic_backend.Authorization;

using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddAuthorization()
        .AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();


builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SisprenicContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.Run();
