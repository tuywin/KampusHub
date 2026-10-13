using System.ComponentModel.DataAnnotations;
using Nokta.Models;

namespace Nokta.ViewModels
{
    // Ana sayfa (Pano) için gereken tüm veriler tek bir sınıfta
    public class PanoViewModel
    {
        public int KayipSayisi { get; set; }
        public int BulunanSayisi { get; set; }
        public int TeslimSayisi { get; set; }
        public Seans AktifSeans { get; set; }
        public int BosMasa { get; set; }
        public int ToplamMasa { get; set; }
        public List<KayipEsya> SonIlanlar { get; set; } = new();

        public int DolulukYuzdesi => ToplamMasa == 0 ? 0 : (ToplamMasa - BosMasa) * 100 / ToplamMasa;
    }

    // Kayıp eşya listesi + filtre değerleri
    public class KayipEsyaListeViewModel
    {
        public List<KayipEsya> Ilanlar { get; set; } = new();
        public string? Arama { get; set; }
        public EsyaDurumu? Durum { get; set; }
        public string? Kategori { get; set; }
        public IReadOnlyList<string> Kategoriler { get; set; } = Array.Empty<string>();
        public int KayipSayisi { get; set; }
        public int BulunanSayisi { get; set; }
        public bool FiltreVar => !string.IsNullOrWhiteSpace(Arama) || Durum.HasValue || !string.IsNullOrEmpty(Kategori);
    }

    // GençOfis masa planı
    public class MasaPlaniViewModel
    {
        public DateTime Tarih { get; set; }
        public Seans Seans { get; set; }
        public List<MasaDurumu> Masalar { get; set; } = new();
        public List<DateTime> Gunler { get; set; } = new();
        public int BosSayisi => Masalar.Count(m => !m.Dolu);
        public string TarihParametre => Tarih.ToString("yyyy-MM-dd");
    }

    // Rezervasyon formu: formdan gelen veriler + doğrulama kuralları
    public class RezervasyonFormViewModel
    {
        public int MasaId { get; set; }
        public DateTime Tarih { get; set; }
        public Seans Seans { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Adınızı ve soyadınızı yazın.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Ad soyad 3-50 karakter olmalıdır.")]
        public string OgrenciAdi { get; set; } = "";

        [Display(Name = "Öğrenci numarası")]
        [Required(ErrorMessage = "Öğrenci numaranızı yazın.")]
        [RegularExpression(@"^\d{9,10}$", ErrorMessage = "Öğrenci numarası 9 veya 10 haneli olmalıdır.")]
        public string OgrenciNo { get; set; } = "";

        [Display(Name = "Kuralları kabul ediyorum")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Devam etmek için GençOfis kurallarını kabul edin.")]
        public bool KurallariKabul { get; set; }

        // Sadece ekranda göstermek için (formdan gelmez)
        public Masa? Masa { get; set; }
    }

    // Günün rezervasyonları listesi
    public record RezervasyonSatiri(Rezervasyon Rezervasyon, Masa Masa);

    public class RezervasyonListeViewModel
    {
        public DateTime Tarih { get; set; }
        public List<RezervasyonSatiri> Satirlar { get; set; } = new();
        public string TarihParametre => Tarih.ToString("yyyy-MM-dd");
    }
}
