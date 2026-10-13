using Microsoft.AspNetCore.Mvc;
using Nokta.Models;
using Nokta.Services;
using Nokta.ViewModels;

namespace Nokta.Controllers
{
    public class KayipEsyaController : Controller
    {
        private readonly IKayipEsyaService _servis;

        public KayipEsyaController(IKayipEsyaService servis)
        {
            _servis = servis;
        }

        // GET: /KayipEsya?durum=Bulundu&kategori=Elektronik&arama=şarj
        public IActionResult Index(string? arama, EsyaDurumu? durum, string? kategori)
        {
            var model = new KayipEsyaListeViewModel
            {
                Ilanlar = _servis.Ara(arama, durum, kategori),
                Arama = arama,
                Durum = durum,
                Kategori = kategori,
                Kategoriler = _servis.Kategoriler,
                KayipSayisi = _servis.Say(EsyaDurumu.Kayip),
                BulunanSayisi = _servis.Say(EsyaDurumu.Bulundu)
            };
            return View(model);
        }

        // GET: /KayipEsya/Detay/3
        public IActionResult Detay(int id)
        {
            var ilan = _servis.Getir(id);
            if (ilan == null)
            {
                return NotFound();
            }
            return View(ilan);
        }

        // GET: /KayipEsya/Ekle  -> boş form
        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new KayipEsya { Tarih = DateTime.Today, Durum = EsyaDurumu.Kayip });
        }

        // POST: /KayipEsya/Ekle  -> formdan gelen ilanı kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(KayipEsya ilan)
        {
            // Data Annotation kurallarına ek olarak kendi kuralımız
            if (ilan.Tarih.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(KayipEsya.Tarih), "İleri bir tarih seçilemez.");
            }
            if (ilan.Durum == EsyaDurumu.TeslimEdildi)
            {
                ModelState.AddModelError(nameof(KayipEsya.Durum), "Yeni ilan 'Kayıp' veya 'Bulundu' olmalıdır.");
            }

            if (!ModelState.IsValid)
            {
                return View(ilan);   // hatalarla birlikte formu tekrar göster
            }

            _servis.Ekle(ilan);
            TempData["Mesaj"] = "İlanınız yayınlandı.";
            return RedirectToAction(nameof(Detay), new { id = ilan.Id });   // Post-Redirect-Get
        }

        // POST: /KayipEsya/TeslimEt/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TeslimEt(int id)
        {
            if (!_servis.DurumGuncelle(id, EsyaDurumu.TeslimEdildi))
            {
                return NotFound();
            }
            TempData["Mesaj"] = "Eşya sahibine teslim edildi olarak işaretlendi.";
            return RedirectToAction(nameof(Detay), new { id });
        }
    }
}
