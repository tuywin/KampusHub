namespace Nokta.Models
{
    // GençOfis'te gün 3 seansa bölünür
    public enum Seans
    {
        Sabah,
        Ogle,
        Aksam
    }

    public static class SeansBilgi
    {
        public static string Ad(this Seans seans) => seans switch
        {
            Seans.Sabah => "Sabah",
            Seans.Ogle => "Öğle",
            _ => "Akşam"
        };

        public static string Saat(this Seans seans) => seans switch
        {
            Seans.Sabah => "09:00 – 12:30",
            Seans.Ogle => "13:00 – 16:30",
            _ => "17:00 – 20:30"
        };

        // Şu anki saate göre aktif seans
        public static Seans Simdiki()
        {
            var saat = DateTime.Now.Hour;
            if (saat < 13) return Seans.Sabah;
            if (saat < 17) return Seans.Ogle;
            return Seans.Aksam;
        }
    }

    public class Masa
    {
        public int Id { get; set; }
        public string Kod { get; set; } = "";       // A1, B2 ...
        public string Bolge { get; set; } = "";     // Sessiz Alan, Grup Masaları, Bilgisayarlı Alan
        public int Kapasite { get; set; }
        public bool Prizli { get; set; }
    }

    public class Rezervasyon
    {
        public int Id { get; set; }
        public int MasaId { get; set; }
        public DateTime Tarih { get; set; }
        public Seans Seans { get; set; }
        public string OgrenciAdi { get; set; } = "";
        public string OgrenciNo { get; set; } = "";

        // Gizlilik: listede öğrenci numarasının sadece son 3 hanesi görünür
        public string MaskeliNo => OgrenciNo.Length > 3
            ? new string('•', OgrenciNo.Length - 3) + OgrenciNo[^3..]
            : OgrenciNo;
    }

    // Bir masanın belirli tarih + seanstaki durumu
    public record MasaDurumu(Masa Masa, Rezervasyon? Rezervasyon)
    {
        public bool Dolu => Rezervasyon != null;
    }
}
