using Microsoft.AspNetCore.Mvc;
using Nokta.Models;
using Nokta.Services;
using Nokta.ViewModels;

namespace Nokta.Controllers
{
    // GençOfis masa rezervasyonu
    public class RezervasyonController : Controller
    {
        private readonly IRezervasyonService _servis;

        public RezervasyonController(IRezervasyonService servis)
        {
            _servis = servis;
        }

        // Gelen tarihi izin verilen aralığa (bugün .. bugün+6) çeker
        private DateTime GecerliTarih(DateTime? tarih)
        {
            var t = (tarih ?? DateTime.Today).Date;
            var son = DateTime.Today.AddDays(_servis.RezervasyonGunSayisi - 1);
            if (t < DateTime.Today) return DateTime.Today;
            if (t > son) return son;
            return t;
        }

        // GET: /Rezervasyon?tarih=2026-10-14&seans=Ogle  -> masa planı
        public IActionResult Index(DateTime? tarih, Seans? seans)
        {
            var gun = GecerliTarih(tarih);
            var secilenSeans = seans ?? SeansBilgi.Simdiki();

            var model = new MasaPlaniViewModel
            {
                Tarih = gun,
                Seans = secilenSeans,
                Masalar = _servis.Plan(gun, secilenSeans),
                Gunler = Enumerable.Range(0, _servis.RezervasyonGunSayisi)
                                   .Select(i => DateTime.Today.AddDays(i))
                                   .ToList()
            };
            return View(model);
        }

        // GET: /Rezervasyon/Olustur?masaId=3&tarih=2026-10-14&seans=Sabah
        [HttpGet]
        public IActionResult Olustur(int masaId, DateTime tarih, Seans seans)
        {
            var masa = _servis.MasaGetir(masaId);
            if (masa == null)
            {
                return NotFound();
            }

            var gun = GecerliTarih(tarih);
            var plan = _servis.Plan(gun, seans);
            if (plan.Any(m => m.Masa.Id == masaId && m.Dolu))
            {
                TempData["Hata"] = $"{masa.Kod} masası bu seansta dolu.";
                return RedirectToAction(nameof(Index), new { tarih = gun.ToString("yyyy-MM-dd"), seans });
            }

            return View(new RezervasyonFormViewModel
            {
                MasaId = masaId,
                Tarih = gun,
                Seans = seans,
                Masa = masa
            });
        }

        // POST: /Rezervasyon/Olustur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Olustur(RezervasyonFormViewModel model)
        {
            model.Masa = _servis.MasaGetir(model.MasaId);
            if (model.Masa == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var rezervasyon = new Rezervasyon
            {
                MasaId = model.MasaId,
                Tarih = model.Tarih,
                Seans = model.Seans,
                OgrenciAdi = model.OgrenciAdi.Trim(),
                OgrenciNo = model.OgrenciNo
            };

            if (!_servis.Olustur(rezervasyon, out var hata))
            {
                ModelState.AddModelError(string.Empty, hata ?? "Rezervasyon oluşturulamadı.");
                return View(model);
            }

            TempData["Mesaj"] = $"{model.Masa.Kod} masası {model.Tarih:dd MMMM} {model.Seans.Ad()} seansı için ayrıldı.";
            return RedirectToAction(nameof(Liste), new { tarih = model.Tarih.ToString("yyyy-MM-dd") });
        }

        // GET: /Rezervasyon/Liste?tarih=2026-10-14  -> günün rezervasyonları
        public IActionResult Liste(DateTime? tarih)
        {
            var gun = GecerliTarih(tarih);
            var masalar = _servis.Masalar().ToDictionary(m => m.Id);

            var model = new RezervasyonListeViewModel
            {
                Tarih = gun,
                Satirlar = _servis.GununRezervasyonlari(gun)
                    .Where(r => masalar.ContainsKey(r.MasaId))
                    .Select(r => new RezervasyonSatiri(r, masalar[r.MasaId]))
                    .ToList()
            };
            return View(model);
        }

        // POST: /Rezervasyon/Iptal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Iptal(int id, DateTime tarih)
        {
            if (_servis.Iptal(id))
                TempData["Mesaj"] = "Rezervasyon iptal edildi.";
            else
                TempData["Hata"] = "Rezervasyon bulunamadı.";
            return RedirectToAction(nameof(Liste), new { tarih = tarih.ToString("yyyy-MM-dd") });
        }
    }
}
