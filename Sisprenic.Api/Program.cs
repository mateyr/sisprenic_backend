using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Serilog;
using Serilog.Events;

using Sisprenic.Api.Common;
using Sisprenic.Api.Database;
using Sisprenic.Api.Extensions;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Modules.Auth.Login;
using Sisprenic.Api.Modules.Auth.Register;
using Sisprenic.Api.Modules.Clients.CreateClient;
using Sisprenic.Api.Modules.Clients.UpdateClient;
using Sisprenic.Api.Modules.Loans.CreateLoan;
using Sisprenic.Api.Modules.Loans.UpdateLoan;
using Sisprenic.Api.Modules.Payments.CreatePayment;

using Sisprenic.Reports.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    string[]? allowedOrigins = builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>() ?? Array.Empty<string>();

    builder.Services.AddDatabase(builder.Configuration);

    builder.Services.AddAuthorization()
            .AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

    builder.Services.AddIdentityApiEndpoints<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<SisprenicContext>();

    // Default to the httpOnly application cookie instead of the bearer scheme.
    builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme);

    builder.Services.ConfigureApplicationCookie(options =>
    {
        // API, not a server-rendered app: return 401/403 instead of redirecting to login pages.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.Converters.Add(new OptionalJsonConverterFactory()));

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

    builder.Services.AddGlobalExceptionHandling();

    // Validators
    builder.Services.AddScoped<IValidator<CreatePaymentRequest>, CreatePaymentValidator>();
    builder.Services.AddScoped<IValidator<CreateClientRequest>, CreateClientValidator>();
    builder.Services.AddScoped<IValidator<UpdateClientRequest>, UpdateClientValidator>();
    builder.Services.AddScoped<IValidator<CreateLoanRequest>, CreateLoanValidator>();
    builder.Services.AddScoped<IValidator<UpdateLoanRequest>, UpdateLoanValidator>();
    builder.Services.AddScoped<IValidator<LoginRequest>, LoginValidator>();
    builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();

    builder.Services.AddReporting();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<SisprenicContext>();

    builder.Services.AddApiRateLimiting();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseGlobalExceptionHandling();

    // Map application endpoints
    app.MapEndpoints();
    app.MapHealthChecks("/health").AllowAnonymous();

    await app.MigrateDbAsync();
    await app.SeedAdminAsync();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerWithUi();
    }

    app.UseHttpsRedirection();

    app.UseApiRateLimiting();

    app.UseCors("AllowOrigins");

    app.UseAuthentication();
    app.UseAuthorization();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
