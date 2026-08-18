using Website_of_Everything.Components;
using Website_of_Everything.Services;

var builder =
    WebApplication.CreateBuilder(args);


// Razor / Blazor
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// Spell data service
builder.Services
    .AddScoped<SpellService>();


var app =
    builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}


app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);


app.UseHttpsRedirection();

app.UseAntiforgery();


app.MapStaticAssets();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();