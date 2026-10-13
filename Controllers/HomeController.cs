using Microsoft.AspNetCore.Mvc;
using Nokta.Models;
using Nokta.Services;
using Nokta.ViewModels;

namespace Nokta.Controllers
{
    public class HomeController : Controller
    {
        private readonly IKayipEsyaService _esyaServisi;
        private readonly IRezervasyonService _rezervasyonServisi;

        // Servisler constructor üzerinden enjekte edilir (Dependency Injection)
        public HomeController(IKayipEsyaService esyaServisi, IRezervasyonService rezervasyonServisi)
        {
            _esyaServisi = esyaServisi;
            _rezervasyonServisi = rezervasyonServisi;
        }

        // GET: /  (Pano)
        public IActionResult Index()
        {
            var aktifSeans = SeansBilgi.Simdiki();
            var plan = _rezervasyonServisi.Plan(DateTime.Today, aktifSeans);

            var model = new PanoViewModel
            {
                KayipSayisi = _esyaServisi.Say(EsyaDurumu.Kayip),
                BulunanSayisi = _esyaServisi.Say(EsyaDurumu.Bulundu),
                TeslimSayisi = _esyaServisi.Say(EsyaDurumu.TeslimEdildi),
                AktifSeans = aktifSeans,
                ToplamMasa = plan.Count,
                BosMasa = plan.Count(m => !m.Dolu),
                SonIlanlar = _esyaServisi.SonEklenenler(4)
            };

            return View(model);
        }

        // GET: /Home/Hakkinda
        public IActionResult Hakkinda()
        {
            return View();
        }
    }
}
