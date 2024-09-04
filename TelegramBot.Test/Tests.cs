using System.Configuration;
using Telegram.Bot.Types;

namespace TelegramBot.Test
{
    public class Tests
    {
        private Functions _telegramBot;
        private Dictionary<long, string> _history;
        [SetUp]
        public void Setup()
        {
            _telegramBot = new Functions();
            _history = new Dictionary<long, string>();
            ConfigurationManager.AppSettings.Set("TelegramApiKey", "7251183014:AAGfhjFLsdz9mm3wWX9TRt6apIPZ7zBwHf4");
            ConfigurationManager.AppSettings.Set("CheckoApiKey", "xU1fUZi7bBbyZRwL");
        }

        [Test]
        public void TestSendRequest()
        {
            var inn = "7842349892";

            var result = _telegramBot.SendRequest("https://api.checko.ru/v2/", $"company?key={ConfigurationManager.AppSettings["CheckoApiKey"]}&inn={inn}");

            Assert.That(result.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public void TestCommandStart()
        {
            var message = new Message();
            message.Text = "/start";

            var expected = ("Добро пожаловать в бота! Введите /help чтобы увидеть список доступных команд.", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandHelp()
        {
            var message = new Message();
            message.Text = "/help";

            var expected = ("/start – начать общение с ботом\n" +
                        "/help – вывести справку о доступных командах\n" +
                        "/hello – вывести ваше имя и фамилию, ваш email и ссылку на github\n" +
                        "/inn – получить наименования и адреса компаний по ИНН\n" +
                        "/last – повторить последнее действие бота\n" + "" +
                        "/okved - получить коды (ОКВЭД) и виды деятельности компании по ИНН", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandHello()
        {
            var message = new Message();
            message.Text = "/hello";

            var expected = ("Улесов Иван\n" +
                        "Email: ylesov.ivan@gmail.com\n" +
                        "Github: https://github.com/GrozdnyiDeveloper", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnNone()
        {
            var message = new Message();
            message.Text = "/inn";

            var expected = ("Номер ИНН компании не был введён.\n" +
                "Повторите запрос, введя ИНН после команды (/inn <ИНН номер> или /okved <ИНН номер>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnIncorrect()
        {
            var message = new Message();
            message.Text = "/inn number";

            var expected = ("Номер ИНН компании не был введён.\n" +
                "Повторите запрос, введя ИНН после команды (/inn <ИНН номер> или /okved <ИНН номер>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnOne()
        {
            var message = new Message();
            message.Text = "/inn 5262143887";

            var expected = ("Введённый ИНН: 5262143887\nПолное наименование: ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \"МЕГАТЕК\"\nЮридический адрес: 603136, Нижегородская область, г. Нижний Новгород, ул. Ванеева, д. 227, кв. 12\nФилиалы: \n* Отсутствуют\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnMultiple()
        {
            var message = new Message();
            message.Text = "/inn 7723836671 5262143887";

            var expected = ("Введённый ИНН: 7723836671\nПолное наименование: АКЦИОНЕРНОЕ ОБЩЕСТВО \"КОРПОРАЦИЯ КОСМИЧЕСКИХ СИСТЕМ СПЕЦИАЛЬНОГО НАЗНАЧЕНИЯ \"КОМЕТА\"\nЮридический адрес: 115068, г. Москва, ул. Велозаводская, д. 5\nФилиалы: \n* Название: ФИЛИАЛ АКЦИОНЕРНОГО ОБЩЕСТВА \"КОРПОРАЦИЯ КОСМИЧЕСКИХ СИСТЕМ СПЕЦИАЛЬНОГО НАЗНАЧЕНИЯ \"КОМЕТА\" - \"ОПЫТНЫЙ ПРОИЗВОДСТВЕННО-ТЕХНИЧЕСКИЙ ЦЕНТР\", Адрес: 171110, Тверская область, г. Вышний Волочек, Ржевский тракт, д. 24, к.а; \n* Название: ФИЛИАЛ АКЦИОНЕРНОГО ОБЩЕСТВА \"КОРПОРАЦИЯ КОСМИЧЕСКИХ СИСТЕМ СПЕЦИАЛЬНОГО НАЗНАЧЕНИЯ \"КОМЕТА\" - \"ВОСТОЧНЫЙ\", Адрес: 681071, Хабаровский край, Комсомольский район, с. Гайтер-1; \n* Название: ФИЛИАЛ АКЦИОНЕРНОГО ОБЩЕСТВА \"КОРПОРАЦИЯ КОСМИЧЕСКИХ СИСТЕМ СПЕЦИАЛЬНОГО НАЗНАЧЕНИЯ \"КОМЕТА\" - \"КОНСТРУКТОРСКОЕ БЮРО ИЗМЕРИТЕЛЬНЫХ ПРИБОРОВ \"КВАЗАР\", Адрес: 603022, Нижегородская область, г. Нижний Новгород, ул. Окский Съезд, д. 2, к.а; \n* Название: ФИЛИАЛ АКЦИОНЕРНОГО ОБЩЕСТВА \"КОРПОРАЦИЯ КОСМИЧЕСКИХ СИСТЕМ СПЕЦИАЛЬНОГО НАЗНАЧЕНИЯ \"КОМЕТА\" - \"НАУЧНО-ТЕХНИЧЕСКИЙ ВНЕДРЕНЧЕСКИЙ ЦЕНТР\", Адрес: 249166, Калужская область, Жуковский район, с. Курилово, д. 28; \n* Название: ФИЛИАЛ АКЦИОНЕРНОГО ОБЩЕСТВА \"КОРПОРАЦИЯ КОСМИЧЕСКИХ СИСТЕМ СПЕЦИАЛЬНОГО НАЗНАЧЕНИЯ \"КОМЕТА\" - \"НАУЧНО-ПРОЕКТНЫЙ ЦЕНТР ОПТОЭЛЕКТРОННЫХ КОМПЛЕКСОВ НАБЛЮДЕНИЯ, Адрес: 194021, г. Санкт-Петербург, ул. Шателена, д. 7; \n\n\nВведённый ИНН: 5262143887\nПолное наименование: ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \"МЕГАТЕК\"\nЮридический адрес: 603136, Нижегородская область, г. Нижний Новгород, ул. Ванеева, д. 227, кв. 12\nФилиалы: \n* Отсутствуют\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedNone()
        {
            var message = new Message();
            message.Text = "/okved";

            var expected = ("Номер ИНН компании не был введён.\n" +
                "Повторите запрос, введя ИНН после команды (/inn <ИНН номер> или /okved <ИНН номер>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedIncorrect()
        {
            var message = new Message();
            message.Text = "/okved number";

            var expected = ("Номер ИНН компании не был введён.\n" +
                "Повторите запрос, введя ИНН после команды (/inn <ИНН номер> или /okved <ИНН номер>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedOne()
        {
            var message = new Message();
            message.Text = "/okved 7723836671";

            var expected = ("Введённый ИНН: 7723836671\nОсновная деятельность: 72.19 Научные исследования и разработки в области естественных и технических наук прочие\nДополнительная деятельность: \n* 46.46 Торговля оптовая фармацевтической продукцией; \n* 46.90 Торговля оптовая неспециализированная; \n* 41.20 Строительство жилых и нежилых зданий; \n* 33.13 Ремонт электронного и оптического оборудования; \n* 95.12 Ремонт коммуникационного оборудования; \n* 26.30.17 Производство радио- и телевизионной передающей аппаратуры; \n* 25.40 Производство оружия и боеприпасов; \n* 26.70 Производство оптических приборов, фото- и кинооборудования; \n* 32.50 Производство медицинских инструментов и оборудования; \n* 85.23 Подготовка кадров высшей квалификации; \n* 86.21 Общая врачебная практика; \n* 85.42 Образование профессиональное дополнительное; \n* 33.20 Монтаж промышленных машин и оборудования; \n* 58.14 Издание журналов и периодических изданий; \n* 56.10.1 Деятельность ресторанов и кафе с полным ресторанным обслуживанием, кафетериев, ресторанов быстрого питания и самообслуживания; \n* 73.11 Деятельность рекламных агентств; \n* 63.11.1 Деятельность по созданию и использованию баз данных и информационных ресурсов; \n* 55.10 Деятельность гостиниц и прочих мест для временного проживания; \n* 61.10 Деятельность в области связи на базе проводных технологий; \n* 58.19 Виды издательской деятельности прочие; \n\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedMultiple()
        {
            var message = new Message();
            message.Text = "/okved 7723836671 5262143887";

            var expected = ("Введённый ИНН: 7723836671\nОсновная деятельность: 72.19 Научные исследования и разработки в области естественных и технических наук прочие\nДополнительная деятельность: \n* 46.46 Торговля оптовая фармацевтической продукцией; \n* 46.90 Торговля оптовая неспециализированная; \n* 41.20 Строительство жилых и нежилых зданий; \n* 33.13 Ремонт электронного и оптического оборудования; \n* 95.12 Ремонт коммуникационного оборудования; \n* 26.30.17 Производство радио- и телевизионной передающей аппаратуры; \n* 25.40 Производство оружия и боеприпасов; \n* 26.70 Производство оптических приборов, фото- и кинооборудования; \n* 32.50 Производство медицинских инструментов и оборудования; \n* 85.23 Подготовка кадров высшей квалификации; \n* 86.21 Общая врачебная практика; \n* 85.42 Образование профессиональное дополнительное; \n* 33.20 Монтаж промышленных машин и оборудования; \n* 58.14 Издание журналов и периодических изданий; \n* 56.10.1 Деятельность ресторанов и кафе с полным ресторанным обслуживанием, кафетериев, ресторанов быстрого питания и самообслуживания; \n* 73.11 Деятельность рекламных агентств; \n* 63.11.1 Деятельность по созданию и использованию баз данных и информационных ресурсов; \n* 55.10 Деятельность гостиниц и прочих мест для временного проживания; \n* 61.10 Деятельность в области связи на базе проводных технологий; \n* 58.19 Виды издательской деятельности прочие; \n\n\nВведённый ИНН: 5262143887\nОсновная деятельность: 51.18.27 Деятельность агентов по оптовой торговле прочими товарами, не  включенными в другие группировки\nДополнительная деятельность: \n* 50.20.2 Техническое обслуживание и ремонт прочих автотранспортных средств; \n* 50.20.1 Техническое обслуживание и ремонт легковых автомобилей; \n* 52.6 Розничная торговля вне магазинов; \n* 50.30.2 Розничная торговля автомобильными деталями, узлами и  принадлежностями; \n* 50.20.3 Предоставление прочих видов услуг по техническому обслуживанию  автотранспортных средств; \n* 51.54.3 Оптовая торговля ручными инструментами; \n* 51.47 Оптовая торговля прочими непродовольственными потребительскими  товарами; \n* 50.30.1 Оптовая торговля автомобильными деталями, узлами и принадлежностями; \n* 51.18.2 Деятельность агентов, специализирующихся на оптовой торговле товарами,  не включенными в другие группировки; \n\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandLast()
        {
            var message = new Message();
            message.Text = "/last";
            message.From = new User();
            message.From.Id = 1;
            _history[1] = "/start";

            var expected = ("Добро пожаловать в бота! Введите /help чтобы увидеть список доступных команд.", true); 
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandIncorrect()
        {
            var message = new Message();
            message.Text = "/delete";

            var expected = ("Это сообщение не является одной из предусмотренных команд.\n" +
                        "Введите /help чтобы увидеть список доступных команд.", false);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}