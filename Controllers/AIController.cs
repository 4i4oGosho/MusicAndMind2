using Microsoft.AspNetCore.Mvc;
using MusicAndMind2.Data;

namespace MusicAndMind2.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ask")]
        public IActionResult Ask([FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Ok("Здравей 🌸 Аз съм AI асистентът на Music & Mind. Мога да ти помогна да откриеш подходящ продукт за спокойствие, фокус или хармония.");
            }

            var originalMessage = message.Trim();
            var msg = originalMessage.ToLower();

            var availableProducts = _context.Products
                .Where(p => p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            // Поздрави
            if (msg == "здравей" || msg == "здрасти" || msg == "хей" || msg == "hello" || msg == "hi" ||
                msg.Contains("добър ден") || msg.Contains("добър вечер") || msg.Contains("добро утро"))
            {
                return Ok("Здравей и добре дошъл в Music & Mind 🌸 Радвам се, че си тук. Аз съм твоят AI асистент и с удоволствие ще ти помогна да откриеш най-подходящите продукти за спокойствие, фокус и вътрешен баланс.");
            }

            // Благодарност
            if (msg.Contains("благодаря") || msg.Contains("мерси"))
            {
                return Ok("Винаги си добре дошъл 🌷 Ако искаш, мога да ти помогна да избереш продукт според това дали търсиш спокойствие, по-добър фокус или специална категория.");
            }

            // Кой си ти
            if (msg.Contains("кой си") || msg.Contains("какво си") || msg.Contains("кой си ти"))
            {
                return Ok("Аз съм AI асистентът на Music & Mind 🤖✨ Тук съм, за да ти помагам с насоки за продуктите в магазина и да направя избора ти по-лесен и приятен.");
            }

            // Какви продукти има
            if (msg.Contains("какви продукти") ||
                msg.Contains("какво има в магазина") ||
                msg.Contains("какво има") ||
                msg.Contains("какво продавате") ||
                msg.Contains("какви неща") ||
                msg.Contains("покажи продукти") ||
                msg.Contains("продукти имате"))
            {
                if (!availableProducts.Any())
                    return Ok("В момента няма налични продукти в магазина, но скоро може да бъдат добавени нови 🌸");

                var names = availableProducts
                    .Take(6)
                    .Select(p => p.Name)
                    .ToList();

                return Ok("Разбира се 😊 В момента в магазина можеш да откриеш продукти като: " + string.Join(", ", names) + ". Ако искаш, мога да ти помогна да избереш нещо конкретно за спокойствие, фокус или по категория.");
            }

            // Категории
            if (msg.Contains("категории") || msg.Contains("категория"))
            {
                var categories = availableProducts
                    .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                    .Select(p => p.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                if (!categories.Any())
                    return Ok("В момента към продуктите още няма добавени категории.");

                return Ok("Наличните категории в магазина са: " + string.Join(", ", categories) + ". Кажи ми коя те интересува и ще те насоча 😊");
            }

            // Спокойствие / релакс / антистрес / сън
            if (msg.Contains("спокой") ||
                msg.Contains("релакс") ||
                msg.Contains("отпус") ||
                msg.Contains("сън") ||
                msg.Contains("антистрес") ||
                msg.Contains("стрес") ||
                msg.Contains("почивка"))
            {
                var relaxingProducts = availableProducts
                    .Where(p =>
                        (!string.IsNullOrWhiteSpace(p.Name) && (
                            p.Name.ToLower().Contains("relax") ||
                            p.Name.ToLower().Contains("calm") ||
                            p.Name.ToLower().Contains("sleep"))) ||
                        (!string.IsNullOrWhiteSpace(p.Description) && (
                            p.Description.ToLower().Contains("релакс") ||
                            p.Description.ToLower().Contains("спокой") ||
                            p.Description.ToLower().Contains("сън") ||
                            p.Description.ToLower().Contains("стрес") ||
                            p.Description.ToLower().Contains("отпуск"))) ||
                        (!string.IsNullOrWhiteSpace(p.LongDescription) && (
                            p.LongDescription.ToLower().Contains("релакс") ||
                            p.LongDescription.ToLower().Contains("спокой") ||
                            p.LongDescription.ToLower().Contains("сън") ||
                            p.LongDescription.ToLower().Contains("стрес"))) ||
                        (!string.IsNullOrWhiteSpace(p.FrequencyInfo) && (
                            p.FrequencyInfo.ToLower().Contains("релакс") ||
                            p.FrequencyInfo.ToLower().Contains("спокой") ||
                            p.FrequencyInfo.ToLower().Contains("сън")))
                    )
                    .Take(3)
                    .ToList();

                if (relaxingProducts.Any())
                {
                    return Ok("За спокойствие и отпускане бих ти препоръчал: " +
                              string.Join(", ", relaxingProducts.Select(p => p.Name)) +
                              ". Ако искаш, мога да ти помогна да избереш най-подходящия от тях 🌙");
                }

                return Ok("Ако търсиш спокойствие, препоръчвам да разгледаш продуктите с релаксиращо описание, честоти за отпускане и насоченост към антистрес или сън 🌸");
            }

            // Фокус / концентрация / учене
            if (msg.Contains("фокус") ||
                msg.Contains("концентрац") ||
                msg.Contains("учене") ||
                msg.Contains("вниман") ||
                msg.Contains("работа") ||
                msg.Contains("продуктив"))
            {
                var focusProducts = availableProducts
                    .Where(p =>
                        (!string.IsNullOrWhiteSpace(p.Description) && (
                            p.Description.ToLower().Contains("фокус") ||
                            p.Description.ToLower().Contains("концентрац") ||
                            p.Description.ToLower().Contains("учене") ||
                            p.Description.ToLower().Contains("внимание"))) ||
                        (!string.IsNullOrWhiteSpace(p.LongDescription) && (
                            p.LongDescription.ToLower().Contains("фокус") ||
                            p.LongDescription.ToLower().Contains("концентрац") ||
                            p.LongDescription.ToLower().Contains("учене"))) ||
                        (!string.IsNullOrWhiteSpace(p.FrequencyInfo) && (
                            p.FrequencyInfo.ToLower().Contains("фокус") ||
                            p.FrequencyInfo.ToLower().Contains("концентрац") ||
                            p.FrequencyInfo.ToLower().Contains("бета")))
                    )
                    .Take(3)
                    .ToList();

                if (focusProducts.Any())
                {
                    return Ok("За фокус и концентрация можеш да разгледаш: " +
                              string.Join(", ", focusProducts.Select(p => p.Name)) +
                              ". Те звучат като добър избор за учене и продуктивност 🧠");
                }

                return Ok("Ако търсиш нещо за фокус, насочи се към продукти, свързани с концентрация, внимание и бета честоти 🧠");
            }

            // Препоръка
            if (msg.Contains("какво да избера") ||
                msg.Contains("кой продукт") ||
                msg.Contains("препоръч") ||
                msg.Contains("какво препоръчваш"))
            {
                var recommended = availableProducts
                    .FirstOrDefault();

                if (recommended != null)
                {
                    return Ok($"Бих ти препоръчал да започнеш с {recommended.Name}. Ако ми кажеш дали търсиш спокойствие, по-добър фокус или нещо от конкретна категория, ще ти дам по-точна препоръка ✨");
                }

                return Ok("С удоволствие бих препоръчал продукт, но в момента не откривам налични артикули в магазина.");
            }

            // Най-евтин / цена
            if (msg.Contains("най-евтин") || msg.Contains("евтин") || msg.Contains("цена"))
            {
                var cheapest = availableProducts
                    .OrderBy(p => p.Price)
                    .FirstOrDefault();

                if (cheapest != null)
                {
                    return Ok($"Най-достъпният продукт в момента е {cheapest.Name} на цена {cheapest.Price:F2} €.");
                }

                return Ok("В момента не успях да намеря информация за цените.");
            }

            // Има ли нещо...
            if (msg.Contains("имате ли") || msg.Contains("има ли"))
            {
                return Ok("Кажи ми какво точно търсиш — например нещо за спокойствие, сън, фокус или определена категория — и ще се опитам да те насоча възможно най-добре 🌸");
            }

            // Общ приятелски fallback
            return Ok("С удоволствие ще ти помогна 😊 Можеш да ме попиташ например: „Какви продукти има?“, „Имате ли нещо за спокойствие?“, „Какво препоръчваш за фокус?“ или „Какви категории има в магазина?“");
        }
    }
}