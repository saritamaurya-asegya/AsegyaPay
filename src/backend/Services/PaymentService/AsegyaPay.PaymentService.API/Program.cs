using AsegyaPay.PaymentService.Application.Commands.CreatePayment;
using AsegyaPay.PaymentService.Application.Commands.CapturePayment;
using AsegyaPay.PaymentService.Application.Commands.RefundPayment;
using AsegyaPay.PaymentService.Application.Queries.GetPayment;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ───────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/payment-service-.log", rollingInterval: RollingInterval.Day));

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AsegyaPay Payment Service API",
        Version = "v1",
        Description = "Enterprise payment processing API — create, capture, refund payments.",
        Contact = new OpenApiContact { Name = "AsegyaPay Team", Email = "api@asegyapay.com" }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token: ******",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
});

// ── MediatR + Validation ──────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreatePaymentCommandValidator).Assembly);

// ── OpenTelemetry ─────────────────────────────────────────────────────────────
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("AsegyaPay.PaymentService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(o =>
        {
            o.AgentHost = builder.Configuration["Jaeger:Host"] ?? "localhost";
            o.AgentPort = int.Parse(builder.Configuration["Jaeger:Port"] ?? "6831");
        }));

// ── Authentication ────────────────────────────────────────────────────────────
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMerchantPortal", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Service v1"));
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowMerchantPortal");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "payment-service", timestamp = DateTime.UtcNow }))
   .AllowAnonymous();

app.Run();
