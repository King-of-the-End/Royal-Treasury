using Microsoft.AspNetCore.HttpOverrides;
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
// FORWARDED HEADERS
//
// Render terminates HTTPS at its proxy and
// forwards the request to this application.
//
// Blazor Interactive Server needs ASP.NET
// Core to understand the original request
// scheme correctly so that SignalR / Blazor
// can negotiate its secure connection.
//
// Render does not provide a single fixed
// proxy address for an individual service,
// so this configuration accepts forwarded
// headers from Render's upstream proxy.
//
// IMPORTANT:
// Because this is configured here in code,
// remove the Render environment variable:
//
// ASPNETCORE_FORWARDEDHEADERS_ENABLED
//
// Keep:
//
// ASPNETCORE_ENVIRONMENT=Production
// =========================================

builder.Services
    .Configure<ForwardedHeadersOptions>(
        options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor
                |
                ForwardedHeaders.XForwardedProto;


            // =================================
            // RENDER PROXY TRUST
            //
            // Render's proxy addresses are not
            // fixed per application.
            //
            // Clearing these collections gives
            // the same cloud-style behavior as
            // ASPNETCORE_FORWARDEDHEADERS_ENABLED
            // while keeping the middleware
            // placement explicit in this file.
            // =================================

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });


// =========================================
// SERVICES
// =========================================


// =========================================
// SPELL SERVICE
//
// Loads spell JSON files used by the
// Spells section.
// =========================================

builder.Services
    .AddScoped<SpellService>();


// =========================================
// MONSTER SERVICE
//
// Loads monster JSON files used by the
// Bestiary section.
//
// Expected location:
//
// wwwroot/data/Monsters/
//
// Examples:
//
// CR0.json
// CR0.125.json
// CR0.25.json
// CR0.5.json
// CR1.json
// CR2.json
// ...
// CR30.json
//
// The MonsterService also supports folders
// with these names containing individual
// monster JSON files.
// =========================================

builder.Services
    .AddScoped<MonsterService>();


// =========================================
// GLOSSARY SERVICE
//
// Shared site glossary.
//
// The service loads:
//
// wwwroot/data/glossary.json
//
// once and reuses the parsed glossary.
//
// Singleton is appropriate here because the
// glossary is shared throughout the site and
// does not need a separate copy per user.
// =========================================

builder.Services
    .AddSingleton<GlossaryService>();


// =========================================
// BUILD APP
// =========================================

var app =
    builder.Build();


// =========================================
// FORWARDED HEADERS MUST RUN FIRST
//
// This must happen before HSTS and HTTPS
// redirection so ASP.NET Core sees the
// original HTTPS request from Render.
// =========================================

app.UseForwardedHeaders();


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
//
// Sends missing pages through the custom
// /not-found page.
// =========================================

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);


// =========================================
// HTTPS
// =========================================

app.UseHttpsRedirection();


// =========================================
// ANTIFORGERY
//
// Required by interactive server-side
// Razor components.
// =========================================

app.UseAntiforgery();


// =========================================
// STATIC FILES
//
// Makes files in wwwroot available,
// including:
//
// wwwroot/data/
// wwwroot/images/
// wwwroot/css/
// etc.
// =========================================

app.MapStaticAssets();


// =========================================
// BLAZOR
//
// AddInteractiveServerRenderMode maps the
// Interactive Server endpoints used by the
// Blazor circuit / SignalR connection.
// =========================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// =========================================
// RUN
// =========================================

app.Run();