using System.ComponentModel.DataAnnotations;

namespace Nokta.Models
{
    public enum EsyaDurumu
    {
        Kayip,
        Bulundu,
        TeslimEdildi
    }

    public class KayipEsya
    {
        public int Id { get; set; }

        [Display(Name = "Eşya adı")]
        [Required(ErrorMessage = "Eşya adı zorunludur.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Eşya adı 3-60 karakter olmalıdır.")]
        public string Baslik { get; set; } = "";

        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "Kısa bir açıklama yazın.")]
        [StringLength(400, ErrorMessage = "Açıklama en fazla 400 karakter olabilir.")]
        public string Aciklama { get; set; } = "";

        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Bir kategori seçin.")]
        public string Kategori { get; set; } = "";

        [Display(Name = "İlan türü")]
        public EsyaDurumu Durum { get; set; } = EsyaDurumu.Kayip;

        [Display(Name = "Konum")]
        [Required(ErrorMessage = "Eşyanın kaybolduğu/bulunduğu yeri yazın.")]
        [StringLength(80)]
        public string Konum { get; set; } = "";

        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [Display(Name = "Adın")]
        [Required(ErrorMessage = "Adınızı yazın.")]
        [StringLength(50)]
        public string IlanSahibi { get; set; } = "";

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Eposta { get; set; } = "";

        // Hesaplanan alan: ilan kaç gün önce verildi
        public int GunOnce => (DateTime.Today - Tarih.Date).Days;
    }

    // Enum için görünüm yardımcıları (View'lerde @Model.Durum.Ad() şeklinde kullanılır)
    public static class EsyaDurumuBilgi
    {
        public static string Ad(this EsyaDurumu durum) => durum switch
        {
            EsyaDurumu.Kayip => "Kayıp",
            EsyaDurumu.Bulundu => "Bulundu",
            _ => "Teslim edildi"
        };

        public static string CssSinif(this EsyaDurumu durum) => durum switch
        {
            EsyaDurumu.Kayip => "rozet-kayip",
            EsyaDurumu.Bulundu => "rozet-bulundu",
            _ => "rozet-teslim"
        };
    }
}
