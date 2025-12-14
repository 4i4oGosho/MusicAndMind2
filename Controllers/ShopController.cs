using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Models;

namespace MusicAndMind2.Controllers
{
    public class ShopController : Controller
    {
        public static readonly List<Product> Products = new()
        {
            new Product
            {
                Id = 1,
                Name = "Тибетска купа (432 Hz)",
                Description = "Кристална купа, чийто звук насърчава дълбока релаксация и вътрешен баланс.",
                Price = 49.99m,
                ImageUrl = "/shop/bowl1.png",
                SoundUrl = "/shop/audio/bowl1.mp3",
                FrequencyInfo = "Подпомага алфа мозъчни вълни (8–12 Hz).",
                Category = "Купи",
                LongDescription = "Тази тибетска купа е изработена ръчно в подножието на Хималаите и създава меки, дълбоки вибрации, които хармонизират тялото и ума. Звукът ѝ подпомага баланса между енергийните центрове, отпуска нервната система и помага за концентрация по време на медитация или йога. Идеална за звукови терапии, релаксация и лична употреба."
            },
            new Product
            {
                Id = 2,
                Name = "Камертон (528 Hz)",
                Description = "Камертон за енергийно пречистване и „честотата на любовта“.",
                Price = 29.99m,
                ImageUrl = "/shop/fork1.png",
                SoundUrl = "/shop/audio/fork1.mp3",
                FrequencyInfo = "528 Hz се асоциира с усещане за мир и вътрешна хармония.",
                Category = "Камертон",
                LongDescription = "Този камертон е настроен на 528 Hz — честота, често наричана „честотата на любовта“. Тя се използва в звукотерапията за регенерация на клетките и намаляване на стреса. Изключително подходящ за енергийни практики, дихателни сесии и медитации, насочени към сърдечната чакра."
            },
            new Product
            {
                Id = 3,
                Name = "Гонг – Златен резонанс",
                Description = "Малък гонг с богат, вибрационен тон, подходящ за дълбоки медитации.",
                Price = 79.99m,
                ImageUrl = "/shop/gong1.png",
                SoundUrl = "/shop/audio/gong1.mp3",
                FrequencyInfo = "Тета честоти (4–7 Hz), подходящи за сън и релакс.",
                Category = "Гонгове",
                LongDescription = "Златният гонг е изработен от висококачествен бронз и произвежда дълбок, хармоничен тон, който прониква през съзнанието и създава усещане за пространственост и покой. Идеален за медитации, звукотерапия и дълбока релаксация."
            },
            new Product
            {
                Id = 4,
                Name = "Калимба – Звуков поток",
                Description = "Инструмент с топъл, дървен тон, който стимулира креативността.",
                Price = 39.99m,
                ImageUrl = "/shop/kalimba1.png",
                SoundUrl = "/shop/audio/kalimba1.mp3",
                FrequencyInfo = "Стимулира бета вълни (13–30 Hz) – фокус и вдъхновение.",
                Category = "Калимби",
                LongDescription = "Калимбата е африкански музикален инструмент с дървен резонатор и метални езичета, които създават меки, хармонични звуци. Чудесен избор за начинаещи и напреднали, които търсят баланс между музика и медитация."
            },
            new Product
            {
                Id = 5,
                Name = "Кристален камертон (639 Hz)",
                Description = "Камертон, свързан със състрадание и хармония в отношенията.",
                Price = 34.99m,
                ImageUrl = "/shop/fork2.png",
                SoundUrl = "/shop/audio/fork2.mp3",
                FrequencyInfo = "639 Hz – честота на връзката и сърдечната чакра.",
                Category = "Камертони",
                LongDescription = "Кристалният камертон с честота 639 Hz се използва за подобряване на комуникацията, доверието и любовта в човешките отношения. Неговата вибрация активира сърдечната чакра и подпомага изцелението на емоционални блокажи. Идеален за звукотерапевти, медитации и партньорски практики."
            },
            new Product
            {
                Id = 6,
                Name = "Шамански барабан (7 Hz)",
                Description = "Ръчно изработен барабан, който резонира с пулса на земята.",
                Price = 89.99m,
                ImageUrl = "/shop/drum1.png",
                SoundUrl = "/shop/audio/drum1.mp3",
                FrequencyInfo = "7 Hz – тета резонанс за дълбока медитация.",
                Category = "Барабани",
                LongDescription = "Шаманският барабан е инструмент, който свързва човека със земните честоти. Изработен от естествени материали, той излъчва дълбоки вибрации, които успокояват ума и навлизат в състояние на транс. Подходящ за медитации, шамански церемонии и терапевтични сесии."
            },
            new Product
            {
                Id = 7,
                Name = "Пеещ кристал (963 Hz)",
                Description = "Фин инструмент от кварцов кристал за духовна връзка и яснота.",
                Price = 69.99m,
                ImageUrl = "/shop/crystal1.png",
                SoundUrl = "/shop/audio/crystal1.mp3",
                FrequencyInfo = "963 Hz – активира връзката с висшето съзнание.",
                Category = "Кристали",
                LongDescription = "Пеещият кристал създава изключително чист звук, който събужда вътрешното съзнание и балансира енергията на коронната чакра. Честотата 963 Hz се свързва с просветление, интуиция и вътрешен мир. Препоръчва се за напреднали практики в медитацията и звукотерапията."
            },
            new Product
            {
                Id = 8,
                Name = "Звукова пирамида (888 Hz)",
                Description = "Медна пирамида, която излъчва стабилизираща вибрация.",
                Price = 59.99m,
                ImageUrl = "/shop/pyramid1.png",
                SoundUrl = "/shop/audio/pyramid1.mp3",
                FrequencyInfo = "888 Hz – баланс между материя и дух.",
                Category = "Пирамиди",
                LongDescription = "Звуковата пирамида излъчва мек и дълбок тон, който стабилизира енергийните центрове и хармонизира околната среда. Символизира равновесието между земното и духовното. Използва се за прочистване на пространство и фокусиране на ума."
            },
            new Product
            {
                Id = 9,
                Name = "Дигитална арфа „Аура“ (528 Hz)",
                Description = "Модерна електронна арфа с честоти за фокус и релаксация.",
                Price = 119.99m,
                ImageUrl = "/shop/harp1.png",
                SoundUrl = "/shop/audio/harp1.mp3",
                FrequencyInfo = "528 Hz – честота на изцеление и любов.",
                Category = "Електронни инструменти",
                LongDescription = "„Аура“ е иновативна дигитална арфа, която комбинира електронна музика и честотна терапия. Проектирана за фокус, медитация и креативност, тя включва предварително зададени честоти за релакс и хармонизиране. Съвършена за модерния практикуващ mindfulness."
            }
        };

        [HttpGet]
        public IActionResult Index() => View(Products);

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Checkout(int productId, string paymentMethod)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(paymentMethod))
                paymentMethod = "card";

            var paymentText = paymentMethod == "cod" ? "Наложен платеж" : "Карта";

            TempData["ProductName"] = product.Name;
            TempData["Price"] = product.Price.ToString("0.00");
            TempData["Payment"] = paymentText;

            return RedirectToAction(nameof(Success));
        }

        [HttpGet]
        public IActionResult Success()
        {
            if (TempData["ProductName"] == null)
                return RedirectToAction(nameof(Index));

            ViewBag.ProductName = TempData["ProductName"];
            ViewBag.Price = TempData["Price"];
            ViewBag.Payment = TempData["Payment"];

            return View();
        }
    }
}
