using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Serilog;
using Serilog.Events;

using Sisprenic.Api.Database;
using Sisprenic.Api.Extensions;

using Sisprenic.Api.Authorization;
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

    // Validators
    builder.Services.AddScoped<IValidator<CreatePaymentRequest>, CreatePaymentValidator>();
    builder.Services.AddScoped<IValidator<CreateLoanRequest>, CreateLoanValidator>();
    builder.Services.AddScoped<IValidator<UpdateLoanRequest>, UpdateLoanValidator>();

    builder.Services.AddReporting();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapIdentityApi<IdentityUser>().WithTags("Authentication");

    // Map application endpoints
    app.MapEndpoints();

    await app.MigrateDbAsync();
    await app.SeedAdminAsync();

    // Initializes Playwright and prepares Chromium for PDF generation at startup
    await app.InitializeReportingAsync();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerWithUi();
    }

    app.UseHttpsRedirection();

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
