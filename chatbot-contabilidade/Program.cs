var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Adiciona o GroqService como um serviço de escopo para injeção de dependência
builder.Services.AddScoped<chatbot_contabilidade.Services.GroqService>(); // <- adiciona essa
// Adiciona o HttpClient para que o GroqService possa fazer requisições HTTP
builder.Services.AddHttpClient(); // <- e essa

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();