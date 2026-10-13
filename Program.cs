using System.Globalization;
using Nokta.Services;

// Tarih ve gün adları Türkçe görünsün (örn. "13 Ekim 2026, Salı")
var turkce = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = turkce;
CultureInfo.DefaultThreadCurrentUICulture = turkce;

var builder = WebApplication.CreateBuilder(args);

// MVC servisleri
builder.Services.AddControllersWithViews();

// Dependency Injection: Controller'lar veriye doğrudan değil, bu arayüzler üzerinden erişir.
// Singleton: uygulama çalıştığı sürece aynı (bellekteki) veri kullanılır.
builder.Services.AddSingleton<IKayipEsyaService, KayipEsyaService>();
builder.Services.AddSingleton<IRezervasyonService, RezervasyonService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
