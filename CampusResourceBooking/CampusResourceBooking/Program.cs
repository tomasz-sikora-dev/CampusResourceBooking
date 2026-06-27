using CampusResourceBooking.Components;
using CampusResourceBooking.Models;
using CampusResourceBooking.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddDbContextFactory<CampusResourceBookingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CampusResourceBooking")));

builder.Services.AddScoped(provider =>
    provider.GetRequiredService<IDbContextFactory<CampusResourceBookingContext>>().CreateDbContext());

builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<PasswordHashingService>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<BookingPdfService>();
builder.Services.AddSingleton<BookingTimeValidator>();
builder.Services.AddSingleton<StatusInfoProvider>();

var app = builder.Build();

_ = Task.Run(async () =>
{
    try
    {
        var factory = app.Services.GetRequiredService<IDbContextFactory<CampusResourceBookingContext>>();
        await using var db = await factory.CreateDbContextAsync();
        await db.Resources.AsNoTracking().Take(1).ToListAsync();
    }
    catch
    {
        // Best-effort warm-up. Startup should not fail when the database is unavailable.
    }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
