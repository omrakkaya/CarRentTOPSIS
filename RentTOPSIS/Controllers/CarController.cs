using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RentTOPSIS.Context;
using RentTOPSIS.Models;
using RentTOPSIS.Models.ViewModels;
using System;

namespace RentTOPSIS.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public CarController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;

        }

        public IActionResult Index()
        {
            var avaliableCars = _context.Arabalar.ToList();

            return View(avaliableCars);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(Arabalar arabalar)
        {
            
                _context.Arabalar.Add(arabalar);
                _context.SaveChanges();
                return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var car = _context.Arabalar.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var car = _context.Arabalar.Find(id);
            if (car != null)
            {
                _context.Arabalar.Remove(car);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var car = _context.Arabalar.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(int id, Arabalar updatedCar)
        {
            if (id != updatedCar.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var car = _context.Arabalar.Find(id);
                if (car == null)
                {
                    return NotFound();
                }

        
                _context.Entry(car).CurrentValues.SetValues(updatedCar);

               

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(updatedCar);
        }



        public IActionResult Normalize()
        {
            var arabalarList = _context.Arabalar.ToList();
            var arabalarViewModelList = arabalarList.Select(a => ConvertToViewModel(a)).ToList();

            var normalizedOptions = NormalizeOptions(arabalarViewModelList);

            return PartialView(normalizedOptions);
            //return View(normalizedOptions);
        }

       
        [HttpPost]
        public IActionResult CalculateWeightedNormalize(ArabalarWeights weights)
        {
            var arabalarList = _context.Arabalar.ToList();
            var arabalarViewModelList = arabalarList.Select(a => ConvertToViewModel(a)).ToList();
            var normalizedOptions = NormalizeOptions(arabalarViewModelList);
            var weightedNormalizedOptions = WeightedNormalizeOptions(normalizedOptions, weights);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));
            _memoryCache.Set("weightedNormalizedOptions", weightedNormalizedOptions, cacheEntryOptions);

            return View("WeightedNormalizeResult", weightedNormalizedOptions);
        }

        [HttpGet]
        public IActionResult CalculateWeightedNormalize()
        {
            ArabalarWeights model = new ArabalarWeights();
            return View(model);
        }

        [HttpGet]
        public IActionResult CalculateSeparationMeasures()
        {
            if (_memoryCache.TryGetValue("weightedNormalizedOptions", out List<ArabalarViewModel> weightedNormalizedOptions))
            {
                // Cache hit: Use the cached data
                var idealSolution = FindIdealSolution(weightedNormalizedOptions);
                var negativeIdealSolution = FindNegativeIdealSolution(weightedNormalizedOptions);
                var separationMeasures = CalculateSeparation(weightedNormalizedOptions, idealSolution, negativeIdealSolution);

                return View("SeparationMeasuresResult", separationMeasures);
            }
            else
            {

                return RedirectToAction("CalculateWeightedNormalize"); 
            }
        }

        public IActionResult CalculateClosenessCoefficients()
        {
            if (!_memoryCache.TryGetValue("weightedNormalizedOptions", out List<ArabalarViewModel> weightedNormalizedOptions))
            {
                return View("Error", new ErrorViewModel { RequestId = "Data not found in cache" });
            }

            var idealSolution = FindIdealSolution(weightedNormalizedOptions);
            var negativeIdealSolution = FindNegativeIdealSolution(weightedNormalizedOptions);
            var separationMeasures = CalculateSeparation(weightedNormalizedOptions, idealSolution, negativeIdealSolution);
            var closenessCoefficients = CalculateClosenessCoefficientsResult(separationMeasures);

            return View("ClosenessCoefficients", closenessCoefficients);
        }



        //////////////////////Alt Kisimda Topsis Adimlarinin Methodlari Yer Alacak Ust Tarafta Ise Viewlar//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public ArabalarViewModel ConvertToViewModel(Arabalar arabalar)
        {
            return new ArabalarViewModel
            {
                Id = arabalar.Id,
                ArabaAdi = arabalar.Marka,
                GunlukUcret = arabalar.GunlukUcret,
                YakitEkonomisi = arabalar.YakitEkonomisi,
                BagajHacmi = arabalar.BagajHacmi,
                AracDonanimi = arabalar.AracDonanimi,
                KmSinirlamasi = arabalar.KmSinirlamasi
            };
        }
     
        public List<ArabalarViewModel> NormalizeOptions(List<ArabalarViewModel> options)
        {
            
            double sumSquaresGunlukUcret = Math.Sqrt(options.Sum(o => o.GunlukUcret * o.GunlukUcret));
            double sumSquaresYakitEkonomisi = Math.Sqrt(options.Sum(o => o.YakitEkonomisi * o.YakitEkonomisi));
            double sumSquaresBagajHacmi = Math.Sqrt(options.Sum(o => o.BagajHacmi * o.BagajHacmi));
            double sumSquaresAracDonanimi = Math.Sqrt(options.Sum(o => o.AracDonanimi * o.AracDonanimi));
            double sumSquaresKmSinirlamasi = Math.Sqrt(options.Sum(o => o.KmSinirlamasi * o.KmSinirlamasi));

            
            return options.Select(o => new ArabalarViewModel
            {
                ArabaAdi = o.ArabaAdi,
                GunlukUcret = o.GunlukUcret / sumSquaresGunlukUcret,
                YakitEkonomisi = o.YakitEkonomisi / sumSquaresYakitEkonomisi,
                BagajHacmi = o.BagajHacmi / sumSquaresBagajHacmi,
                AracDonanimi = o.AracDonanimi / sumSquaresAracDonanimi,
                KmSinirlamasi = o.KmSinirlamasi / sumSquaresKmSinirlamasi
            }).ToList();
        }

        public List<ArabalarViewModel> WeightedNormalizeOptions(List<ArabalarViewModel> normalizedOptions, ArabalarWeights weights)
        {
            return normalizedOptions.Select(o => new ArabalarViewModel
            {
                ArabaAdi = o.ArabaAdi,
                GunlukUcret = o.GunlukUcret * weights.GunlukUcretWeight, 
                YakitEkonomisi = o.YakitEkonomisi * weights.YakitEkonomisiWeight, 
                BagajHacmi = o.BagajHacmi * weights.BagajHacmiWeight, 
                AracDonanimi = o.AracDonanimi * weights.AracDonanimiWeight, 
                KmSinirlamasi = o.KmSinirlamasi * weights.KmSinirlamasiWeight 
            }).ToList();
        }

        public ArabalarViewModel FindIdealSolution(List<ArabalarViewModel> options)
        {
            return new ArabalarViewModel
            {
                GunlukUcret = options.Min(o => o.GunlukUcret), 
                YakitEkonomisi = options.Min(o => o.YakitEkonomisi), 
                BagajHacmi = options.Max(o => o.BagajHacmi),
                AracDonanimi = options.Max(o => o.AracDonanimi),
                KmSinirlamasi = options.Max(o => o.KmSinirlamasi)
            };
        }

        public ArabalarViewModel FindNegativeIdealSolution(List<ArabalarViewModel> options)
        {
            return new ArabalarViewModel
            {
                GunlukUcret = options.Max(o => o.GunlukUcret), 
                YakitEkonomisi = options.Max(o => o.YakitEkonomisi),
                BagajHacmi = options.Min(o => o.BagajHacmi),
                AracDonanimi = options.Min(o => o.AracDonanimi),
                KmSinirlamasi = options.Min(o => o.KmSinirlamasi)
            };
        }

        private List<SeparationMeasure> CalculateSeparation(List<ArabalarViewModel> options, ArabalarViewModel ideal, ArabalarViewModel negativeIdeal)
        {
            return options.Select(o => new SeparationMeasure
            {
                ArabaAdi = o.ArabaAdi,
                PositiveSeparation = Math.Sqrt(
                    Math.Pow(o.GunlukUcret - ideal.GunlukUcret, 2) +
                    Math.Pow(o.YakitEkonomisi - ideal.YakitEkonomisi, 2) +
                    Math.Pow(o.BagajHacmi - ideal.BagajHacmi, 2) +
                    Math.Pow(o.AracDonanimi - ideal.AracDonanimi, 2) +
                    Math.Pow(o.KmSinirlamasi - ideal.KmSinirlamasi, 2)),
                NegativeSeparation = Math.Sqrt(
                    Math.Pow(o.GunlukUcret - negativeIdeal.GunlukUcret, 2) +
                    Math.Pow(o.YakitEkonomisi - negativeIdeal.YakitEkonomisi, 2) +
                    Math.Pow(o.BagajHacmi - negativeIdeal.BagajHacmi, 2) +
                    Math.Pow(o.AracDonanimi - negativeIdeal.AracDonanimi, 2) +
                    Math.Pow(o.KmSinirlamasi - negativeIdeal.KmSinirlamasi, 2))
            }).ToList();
        }

        public List<ClosenessCoefficient> CalculateClosenessCoefficientsResult(List<SeparationMeasure> separationMeasures)
        {
            var closenessCoefficients = new List<ClosenessCoefficient>();

            foreach (var measure in separationMeasures)
            {
                var closenessCoefficient = new ClosenessCoefficient
                {
                    ArabaAdi = measure.ArabaAdi,
                    Coefficient = measure.NegativeSeparation / (measure.PositiveSeparation + measure.NegativeSeparation)
                };

                closenessCoefficients.Add(closenessCoefficient);
            }

            return closenessCoefficients;
        }

    }

}



