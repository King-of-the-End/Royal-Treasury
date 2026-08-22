using Website_of_Everything.Components;
using Website_of_Everything.Services;

var builder = WebApplication.CreateBuilder(args);


// =========================================
// RAZOR / BLAZOR
// =========================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// =========================================
// APPLICATION SERVICES
// =========================================

builder.Services.AddScoped<SpellService>();
builder.Services.AddScoped<MonsterService>();
builder.Services.AddSingleton<GlossaryService>();


// =========================================
// BUILD
// =========================================

var app = builder.Build();


// =========================================
// ERROR HANDLING
// =========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}


// =========================================
// HTTPS
//
// Render handles HTTPS outside the container.
// Do not redirect container traffic to HTTPS
// in production.
// =========================================

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


// =========================================
// STATUS PAGES
// =========================================

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);


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
    .AddInteractiveServerRenderMode(options =>
    {
        options.DisableWebSocketCompression = true;
    });


// =========================================
// START
// =========================================

app.Run();