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
// IMPORTANT FOR RENDER:
//
// Render terminates public HTTPS at its load
// balancer and forwards the request to this
// container over HTTP.
//
// Render already redirects public HTTP
// requests to HTTPS before they reach the
// container.
//
// Calling UseHttpsRedirection() in Production
// can therefore interfere with the Blazor
// Interactive Server WebSocket handshake.
//
// Keep HTTPS redirection only for local
// development.
// =========================================

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


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
//
// WebSocket compression is disabled here
// deliberately for deployment behind
// Render's reverse proxy.
//
// Interactive Server still uses WebSockets
// normally; the frames are simply not
// compressed.
// =========================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(
        options =>
        {
            options.DisableWebSocketCompression =
                true;
        });


// =========================================
// RUN
// =========================================

app.Run();
