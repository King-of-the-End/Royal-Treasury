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
// RESPONSE COMPRESSION
//
// Compress the initial HTML/CSS/JSON
// responses sent to the browser. Static
// images are already compressed separately.
// =========================================

builder.Services.AddResponseCompression(
    options =>
    {
        options.EnableForHttps = true;
    });


// =========================================
// APPLICATION SERVICES
// =========================================

builder.Services.AddSingleton<SpellService>();
builder.Services.AddSingleton<CompendiumJsonService>();
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
// RESPONSE COMPRESSION
// =========================================

app.UseResponseCompression();


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