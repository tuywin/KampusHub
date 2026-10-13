using Nokta.Models;

namespace Nokta.Services
{
    public interface IRezervasyonService
    {
        int RezervasyonGunSayisi { get; }
        List<Masa> Masalar();
        Masa? MasaGetir(int id);
        List<MasaDurumu> Plan(DateTime tarih, Seans seans);
        List<Rezervasyon> GununRezervasyonlari(DateTime tarih);
        bool Olustur(Rezervasyon rezervasyon, out string? hata);
        bool Iptal(int id);
    }

    // GençOfis masalarını ve rezervasyonları bellekte tutan servis
    public class RezervasyonService : IRezervasyonService
    {
        private readonly List<Masa> _masalar = new();
        private readonly List<Rezervasyon> _rezervasyonlar = new();
        private readonly object _kilit = new();
        private int _sonId;

        // Bugünden itibaren kaç gün sonrası için rezervasyon yapılabilir
        public int RezervasyonGunSayisi => 7;

        public RezervasyonService()
        {
            var id = 1;
            for (var i = 1; i <= 8; i++)
                _masalar.Add(new Masa { Id = id++, Kod = $"A{i}", Bolge = "Sessiz Alan", Kapasite = 1, Prizli = i % 2 == 0 });
            for (var i = 1; i <= 4; i++)
                _masalar.Add(new Masa { Id = id++, Kod = $"B{i}", Bolge = "Grup Masaları", Kapasite = i <= 2 ? 4 : 6, Prizli = true });
            for (var i = 1; i <= 4; i++)
                _masalar.Add(new Masa { Id = id++, Kod = $"C{i}", Bolge = "Bilgisayarlı Alan", Kapasite = 1, Prizli = true });

            // Örnek rezervasyonlar (bugün ve yarın)
            var bugun = DateTime.Today;
            OrnekEkle(1, bugun, Seans.Sabah, "Ayşe Yılmaz", "220101045");
            OrnekEkle(2, bugun, Seans.Sabah, "Can Demir", "220101112");
            OrnekEkle(4, bugun, Seans.Sabah, "Ece Kaya", "210104077");
            OrnekEkle(9, bugun, Seans.Sabah, "Proje Grubu 4", "230102003");
            OrnekEkle(13, bugun, Seans.Sabah, "Mehmet Arslan", "220105066");
            OrnekEkle(3, bugun, Seans.Ogle, "Selin Şahin", "230101019");
            OrnekEkle(10, bugun, Seans.Ogle, "Robotik Takımı", "210103041");
            OrnekEkle(14, bugun, Seans.Ogle, "Oğuz Çelik", "220104090");
            OrnekEkle(5, bugun, Seans.Aksam, "Zeynep Aydın", "230101088");
            OrnekEkle(1, bugun.AddDays(1), Seans.Sabah, "Ayşe Yılmaz", "220101045");
            OrnekEkle(11, bugun.AddDays(1), Seans.Ogle, "Girişimcilik Ekibi", "220106011");
        }

        private void OrnekEkle(int masaId, DateTime tarih, Seans seans, string ad, string no)
        {
            _rezervasyonlar.Add(new Rezervasyon
            {
                Id = ++_sonId, MasaId = masaId, Tarih = tarih.Date, Seans = seans, OgrenciAdi = ad, OgrenciNo = no
            });
        }

        public List<Masa> Masalar() => _masalar.ToList();

        public Masa? MasaGetir(int id) => _masalar.FirstOrDefault(m => m.Id == id);

        public List<MasaDurumu> Plan(DateTime tarih, Seans seans)
        {
            lock (_kilit)
            {
                return _masalar
                    .Select(m => new MasaDurumu(m, _rezervasyonlar.FirstOrDefault(r =>
                        r.MasaId == m.Id && r.Tarih == tarih.Date && r.Seans == seans)))
                    .ToList();
            }
        }

        public List<Rezervasyon> GununRezervasyonlari(DateTime tarih)
        {
            lock (_kilit)
            {
                return _rezervasyonlar
                    .Where(r => r.Tarih == tarih.Date)
                    .OrderBy(r => r.Seans).ThenBy(r => r.MasaId)
                    .ToList();
            }
        }

        public bool Olustur(Rezervasyon rezervasyon, out string? hata)
        {
            lock (_kilit)
            {
                var tarih = rezervasyon.Tarih.Date;

                if (tarih < DateTime.Today || tarih >= DateTime.Today.AddDays(RezervasyonGunSayisi))
                {
                    hata = $"Rezervasyon yalnızca bugünden itibaren {RezervasyonGunSayisi} gün için yapılabilir.";
                    return false;
                }

                if (MasaGetir(rezervasyon.MasaId) == null)
                {
                    hata = "Seçilen masa bulunamadı.";
                    return false;
                }

                if (_rezervasyonlar.Any(r => r.MasaId == rezervasyon.MasaId && r.Tarih == tarih && r.Seans == rezervasyon.Seans))
                {
                    hata = "Bu masa seçtiğiniz seansta az önce doldu. Lütfen başka bir masa seçin.";
                    return false;
                }

                if (_rezervasyonlar.Any(r => r.OgrenciNo == rezervasyon.OgrenciNo && r.Tarih == tarih && r.Seans == rezervasyon.Seans))
                {
                    hata = "Bu öğrenci numarasıyla aynı seansta zaten bir rezervasyon var.";
                    return false;
                }

                rezervasyon.Id = ++_sonId;
                rezervasyon.Tarih = tarih;
                _rezervasyonlar.Add(rezervasyon);
                hata = null;
                return true;
            }
        }

        public bool Iptal(int id)
        {
            lock (_kilit)
            {
                return _rezervasyonlar.RemoveAll(r => r.Id == id) > 0;
            }
        }
    }
}
