using Website_of_Everything.Components;
using Website_of_Everything.Services;


var builder =
    WebApplication.CreateBuilder(args);


// =========================================
// RAZOR / BLAZOR
// =========================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// =========================================
// SERVICES
// =========================================

builder.Services
    .AddScoped<SpellService>();

builder.Services
    .AddScoped<MonsterService>();

builder.Services
    .AddSingleton<GlossaryService>();


// =========================================
// BUILD APP
// =========================================

var app =
    builder.Build();


// =========================================
// PRODUCTION ERROR HANDLING
// =========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}


// =========================================
// NOT FOUND
// =========================================

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);


// =========================================
// HTTPS
//
// Render terminates HTTPS at its proxy.
//
// Keep this Render environment variable:
//
// ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
//
// ASP.NET Core will use the forwarded scheme
// supplied by Render.
// =========================================

app.UseHttpsRedirection();


// =========================================
// ANTIFORGERY
// =========================================

app.UseAntiforgery();


// =========================================
// STATIC ASSETS
// =========================================

app.MapStaticAssets();


// =========================================
// BLAZOR INTERACTIVE SERVER
// =========================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// =========================================
// RUN
// =========================================

app.Run();