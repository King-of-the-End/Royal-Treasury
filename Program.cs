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

// Spell JSON loading
builder.Services
    .AddScoped<SpellService>();


// Shared site glossary.
//
// The service loads:
// wwwroot/data/glossary.json
//
// once and reuses the parsed glossary.
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
// =========================================

app.UseHttpsRedirection();


// =========================================
// ANTIFORGERY
// =========================================

app.UseAntiforgery();


// =========================================
// STATIC FILES
// =========================================

app.MapStaticAssets();


// =========================================
// BLAZOR
// =========================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// =========================================
// RUN
// =========================================

app.Run();