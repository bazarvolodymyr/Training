using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Training.API.Auth;
using Training.Domain;
using Training.Application;
using Training.Domain.Models;
using Training.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));
var uiOrigin = builder.Configuration["Auth:UiOrigin"] ?? "http://localhost:4200";
var cookieSecure = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.SameAsRequest
    : CookieSecurePolicy.Always;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services
    .AddApplication()
    .AddPersistence(builder.Configuration);

services.AddIdentity<TrainingUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<TrainingDbContext>()
    .AddDefaultTokenProviders();

services.AddScoped<IdentitySeeder>();
services.AddScoped<IAuthorizationHandler, FullExerciseAccessHandler>();
services.AddAuthorization(options =>
{
    options.AddPolicy(TrainingPolicies.Practice, policy => policy.RequireRole(
        TrainingRoles.Standard,
        TrainingRoles.Premium,
        TrainingRoles.Manager,
        TrainingRoles.Admin));
    options.AddPolicy(TrainingPolicies.PracticeFull, policy =>
        policy.Requirements.Add(new FullExerciseAccessRequirement()));
    options.AddPolicy(TrainingPolicies.EditContent, policy => policy.RequireRole(
        TrainingRoles.Manager,
        TrainingRoles.Admin));
});

services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Training.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = cookieSecure;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
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

services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "XSRF-REQUEST";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = cookieSecure;
});

services.AddCors(options =>
{
    options.AddPolicy("Ui", policy =>
    {
        policy.WithOrigins(uiOrigin)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}

app.UseCors("Ui");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
