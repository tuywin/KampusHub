using Nokta.Models;

namespace Nokta.Services
{
    public interface IKayipEsyaService
    {
        IReadOnlyList<string> Kategoriler { get; }
        List<KayipEsya> Ara(string? arama, EsyaDurumu? durum, string? kategori);
        List<KayipEsya> SonEklenenler(int adet);
        KayipEsya? Getir(int id);
        void Ekle(KayipEsya esya);
        bool DurumGuncelle(int id, EsyaDurumu yeniDurum);
        int Say(EsyaDurumu durum);
    }

    // Veritabanı gelene kadar ilanları bellekte tutan servis.
    // Sonraki haftalarda EF Core kullanan bir sınıfla değiştirilecek; Controller'lar değişmeyecek.
    public class KayipEsyaService : IKayipEsyaService
    {
        private readonly List<KayipEsya> _ilanlar;
        private readonly object _kilit = new();
        private int _sonId;

        public IReadOnlyList<string> Kategoriler { get; } = new[]
        {
            "Elektronik", "Kimlik ve Kart", "Anahtar", "Çanta ve Cüzdan", "Giyim", "Kırtasiye", "Diğer"
        };

        public KayipEsyaService()
        {
            var bugun = DateTime.Today;
            _ilanlar = new List<KayipEsya>
            {
                new() { Id = 1, Baslik = "Siyah deri cüzdan", Kategori = "Çanta ve Cüzdan", Durum = EsyaDurumu.Kayip,
                    Aciklama = "İçinde öğrenci kartı ve birkaç fotoğraf var. Bulan olursa çok sevinirim.",
                    Konum = "Merkez Kütüphane 2. kat", Tarih = bugun, IlanSahibi = "Deniz K.", Eposta = "deniz.k@ornek.edu.tr" },
                new() { Id = 2, Baslik = "AirPods kılıfı (beyaz)", Kategori = "Elektronik", Durum = EsyaDurumu.Bulundu,
                    Aciklama = "Kılıfın üzerinde küçük bir yıldız çıkartması var. Güvenlik masasına bırakıldı.",
                    Konum = "Yemekhane girişi", Tarih = bugun.AddDays(-1), IlanSahibi = "Ece Y.", Eposta = "ece.y@ornek.edu.tr" },
                new() { Id = 3, Baslik = "Mavi Casio hesap makinesi", Kategori = "Kırtasiye", Durum = EsyaDurumu.Kayip,
                    Aciklama = "fx-991 model, arkasında adım yazılı etiket var. Final haftası lazım!",
                    Konum = "Mühendislik Fakültesi B-204", Tarih = bugun.AddDays(-2), IlanSahibi = "Mert A.", Eposta = "mert.a@ornek.edu.tr" },
                new() { Id = 4, Baslik = "Anahtarlık (3 anahtar)", Kategori = "Anahtar", Durum = EsyaDurumu.Bulundu,
                    Aciklama = "Kırmızı bir Galata Kulesi anahtarlığı takılı. Bilgi işlem ofisinde duruyor.",
                    Konum = "Otobüs durağı, Ana Kapı", Tarih = bugun.AddDays(-2), IlanSahibi = "Selin T.", Eposta = "selin.t@ornek.edu.tr" },
                new() { Id = 5, Baslik = "Gri kapüşonlu sweatshirt", Kategori = "Giyim", Durum = EsyaDurumu.Kayip,
                    Aciklama = "M beden, göğsünde üniversite logosu var. Spor salonu soyunma odasında unuttum.",
                    Konum = "Spor Kompleksi", Tarih = bugun.AddDays(-4), IlanSahibi = "Burak Ö.", Eposta = "burak.o@ornek.edu.tr" },
                new() { Id = 6, Baslik = "Öğrenci kimlik kartı", Kategori = "Kimlik ve Kart", Durum = EsyaDurumu.TeslimEdildi,
                    Aciklama = "Sahibine ulaşıldı ve teslim edildi.",
                    Konum = "GençOfis sessiz alan", Tarih = bugun.AddDays(-5), IlanSahibi = "Zeynep A.", Eposta = "zeynep.a@ornek.edu.tr" },
                new() { Id = 7, Baslik = "Laptop şarj aleti (USB-C)", Kategori = "Elektronik", Durum = EsyaDurumu.Bulundu,
                    Aciklama = "65W, Lenovo marka. GençOfis C3 masasının prizinde takılı kalmıştı.",
                    Konum = "GençOfis Bilgisayarlı Alan", Tarih = bugun.AddDays(-1), IlanSahibi = "Ahmet S.", Eposta = "ahmet.s@ornek.edu.tr" },
                new() { Id = 8, Baslik = "Yeşil sırt çantası", Kategori = "Çanta ve Cüzdan", Durum = EsyaDurumu.Kayip,
                    Aciklama = "İçinde ders notlarım ve matara var. Ödüllüdür.",
                    Konum = "Amfi 3", Tarih = bugun.AddDays(-6), IlanSahibi = "Elif D.", Eposta = "elif.d@ornek.edu.tr" }
            };
            _sonId = _ilanlar.Max(i => i.Id);
        }

        public List<KayipEsya> Ara(string? arama, EsyaDurumu? durum, string? kategori)
        {
            lock (_kilit)
            {
                IEnumerable<KayipEsya> sonuc = _ilanlar;

                if (durum.HasValue)
                    sonuc = sonuc.Where(i => i.Durum == durum.Value);

                if (!string.IsNullOrEmpty(kategori))
                    sonuc = sonuc.Where(i => i.Kategori == kategori);

                if (!string.IsNullOrWhiteSpace(arama))
                {
                    var a = arama.Trim();
                    sonuc = sonuc.Where(i =>
                        i.Baslik.Contains(a, StringComparison.CurrentCultureIgnoreCase) ||
                        i.Konum.Contains(a, StringComparison.CurrentCultureIgnoreCase) ||
                        i.Aciklama.Contains(a, StringComparison.CurrentCultureIgnoreCase));
                }

                return sonuc.OrderByDescending(i => i.Tarih).ThenByDescending(i => i.Id).ToList();
            }
        }

        public List<KayipEsya> SonEklenenler(int adet)
        {
            lock (_kilit)
            {
                return _ilanlar
                    .Where(i => i.Durum != EsyaDurumu.TeslimEdildi)
                    .OrderByDescending(i => i.Tarih).ThenByDescending(i => i.Id)
                    .Take(adet).ToList();
            }
        }

        public KayipEsya? Getir(int id)
        {
            lock (_kilit) { return _ilanlar.FirstOrDefault(i => i.Id == id); }
        }

        public void Ekle(KayipEsya esya)
        {
            lock (_kilit)
            {
                esya.Id = ++_sonId;
                _ilanlar.Add(esya);
            }
        }

        public bool DurumGuncelle(int id, EsyaDurumu yeniDurum)
        {
            lock (_kilit)
            {
                var ilan = _ilanlar.FirstOrDefault(i => i.Id == id);
                if (ilan == null) return false;
                ilan.Durum = yeniDurum;
                return true;
            }
        }

        public int Say(EsyaDurumu durum)
        {
            lock (_kilit) { return _ilanlar.Count(i => i.Durum == durum); }
        }
    }
}
