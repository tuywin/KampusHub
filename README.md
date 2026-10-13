# Nokta – Kampüs Kayıp Eşya ve GençOfis Masa Rezervasyonu

İleri Web Programlama dersi projesi · ASP.NET Core 8 MVC

Kampüste kaybolan veya bulunan eşyaların ilan olarak paylaşıldığı ve GençOfis çalışma alanında masa rezervasyonu yapılabilen bir web uygulaması.

## Çalıştırma

**Visual Studio / Rider:** `Nokta.csproj` dosyasını açıp çalıştırın.

**Terminal:**
```
cd Nokta
dotnet run
```

## Sayfalar

| Controller | View'ler | Ne yapar? |
|---|---|---|
| `HomeController` | `Index` (Pano), `Hakkinda` | Sayaçlar, son ilanlar, GençOfis anlık doluluk |
| `KayipEsyaController` | `Index`, `Detay`, `Ekle` | İlan listesi (arama, durum, kategori), ilan detayı, ilan verme formu, "teslim edildi" işaretleme |
| `RezervasyonController` | `Index`, `Olustur`, `Liste` | Gün + seans seçerek masa planı, rezervasyon formu, günün rezervasyonları ve iptal |

## Teknik Yapı

- **Service katmanı + Dependency Injection:** `IKayipEsyaService`, `IRezervasyonService` → `Program.cs`'te `AddSingleton` ile kayıtlı, Controller'lara constructor ile enjekte ediliyor.
- **ViewModel:** `PanoViewModel`, `KayipEsyaListeViewModel`, `MasaPlaniViewModel`, `RezervasyonFormViewModel`, `RezervasyonListeViewModel`
- **Partial View:** `_IlanKarti`, `_DurumRozeti`
- **Formlar ve doğrulama:** Data Annotations (`Required`, `StringLength`, `EmailAddress`, `RegularExpression`, `Range`), `ModelState`, `[ValidateAntiForgeryToken]`, Post-Redirect-Get + `TempData` bildirimleri
- **Razor:** `@foreach`, `@if`, `GroupBy` ile bölgelere ayrılmış masa planı, `@inject` ile View içinde servis kullanımı
- **Responsive CSS:** Yan menü mobilde açılır panele, tablolar kartlara dönüşür

## Klasör Yapısı

```
Nokta/
├── Controllers/   → Home, KayipEsya, Rezervasyon
├── Models/        → KayipEsya, Masa, Rezervasyon, enum'lar
├── ViewModels/    → Sayfalara özel modeller
├── Services/      → Veri işlemleri (şimdilik bellekte)
├── Views/
│   ├── Shared/    → _Layout, _IlanKarti, _DurumRozeti
│   ├── Home/ KayipEsya/ Rezervasyon/
└── wwwroot/       → css, js
```

## Sonraki Adımlar
- Entity Framework Core ile veritabanı (servis arayüzleri değişmeden)
- Öğrenci girişi (Identity) ve "Rezervasyonlarım" sayfası
- İlanlara fotoğraf yükleme
