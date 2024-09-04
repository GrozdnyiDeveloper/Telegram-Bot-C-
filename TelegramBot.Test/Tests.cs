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

            var expected = ("����� ���������� � ����! ������� /help ����� ������� ������ ��������� ������.", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandHelp()
        {
            var message = new Message();
            message.Text = "/help";

            var expected = ("/start � ������ ������� � �����\n" +
                        "/help � ������� ������� � ��������� ��������\n" +
                        "/hello � ������� ���� ��� � �������, ��� email � ������ �� github\n" +
                        "/inn � �������� ������������ � ������ �������� �� ���\n" +
                        "/last � ��������� ��������� �������� ����\n" + "" +
                        "/okved - �������� ���� (�����) � ���� ������������ �������� �� ���", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandHello()
        {
            var message = new Message();
            message.Text = "/hello";

            var expected = ("������ ����\n" +
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

            var expected = ("����� ��� �������� �� ��� �����.\n" +
                "��������� ������, ����� ��� ����� ������� (/inn <��� �����> ��� /okved <��� �����>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnIncorrect()
        {
            var message = new Message();
            message.Text = "/inn number";

            var expected = ("����� ��� �������� �� ��� �����.\n" +
                "��������� ������, ����� ��� ����� ������� (/inn <��� �����> ��� /okved <��� �����>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnOne()
        {
            var message = new Message();
            message.Text = "/inn 5262143887";

            var expected = ("�������� ���: 5262143887\n������ ������������: �������� � ������������ ���������������� \"�������\"\n����������� �����: 603136, ������������� �������, �. ������ ��������, ��. �������, �. 227, ��. 12\n�������: \n* �����������\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandInnMultiple()
        {
            var message = new Message();
            message.Text = "/inn 7723836671 5262143887";

            var expected = ("�������� ���: 7723836671\n������ ������������: ����������� �������� \"���������� ����������� ������ ������������ ���������� \"������\"\n����������� �����: 115068, �. ������, ��. �������������, �. 5\n�������: \n* ��������: ������ ������������ �������� \"���������� ����������� ������ ������������ ���������� \"������\" - \"������� ���������������-����������� �����\", �����: 171110, �������� �������, �. ������ �������, �������� �����, �. 24, �.�; \n* ��������: ������ ������������ �������� \"���������� ����������� ������ ������������ ���������� \"������\" - \"���������\", �����: 681071, ����������� ����, ������������� �����, �. ������-1; \n* ��������: ������ ������������ �������� \"���������� ����������� ������ ������������ ���������� \"������\" - \"��������������� ���� ������������� �������� \"������\", �����: 603022, ������������� �������, �. ������ ��������, ��. ������ �����, �. 2, �.�; \n* ��������: ������ ������������ �������� \"���������� ����������� ������ ������������ ���������� \"������\" - \"������-����������� ������������� �����\", �����: 249166, ��������� �������, ��������� �����, �. ��������, �. 28; \n* ��������: ������ ������������ �������� \"���������� ����������� ������ ������������ ���������� \"������\" - \"������-��������� ����� ��������������� ���������� ����������, �����: 194021, �. �����-���������, ��. ��������, �. 7; \n\n\n�������� ���: 5262143887\n������ ������������: �������� � ������������ ���������������� \"�������\"\n����������� �����: 603136, ������������� �������, �. ������ ��������, ��. �������, �. 227, ��. 12\n�������: \n* �����������\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedNone()
        {
            var message = new Message();
            message.Text = "/okved";

            var expected = ("����� ��� �������� �� ��� �����.\n" +
                "��������� ������, ����� ��� ����� ������� (/inn <��� �����> ��� /okved <��� �����>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedIncorrect()
        {
            var message = new Message();
            message.Text = "/okved number";

            var expected = ("����� ��� �������� �� ��� �����.\n" +
                "��������� ������, ����� ��� ����� ������� (/inn <��� �����> ��� /okved <��� �����>).", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedOne()
        {
            var message = new Message();
            message.Text = "/okved 7723836671";

            var expected = ("�������� ���: 7723836671\n�������� ������������: 72.19 ������� ������������ � ���������� � ������� ������������ � ����������� ���� ������\n�������������� ������������: \n* 46.46 �������� ������� ���������������� ����������; \n* 46.90 �������� ������� ��������������������; \n* 41.20 ������������� ����� � ������� ������; \n* 33.13 ������ ������������ � ����������� ������������; \n* 95.12 ������ ����������������� ������������; \n* 26.30.17 ������������ �����- � ������������� ���������� ����������; \n* 25.40 ������������ ������ � �����������; \n* 26.70 ������������ ���������� ��������, ����- � ����������������; \n* 32.50 ������������ ����������� ������������ � ������������; \n* 85.23 ���������� ������ ������ ������������; \n* 86.21 ����� ��������� ��������; \n* 85.42 ����������� ���������������� ��������������; \n* 33.20 ������ ������������ ����� � ������������; \n* 58.14 ������� �������� � ������������� �������; \n* 56.10.1 ������������ ���������� � ���� � ������ ����������� �������������, ����������, ���������� �������� ������� � ����������������; \n* 73.11 ������������ ��������� ��������; \n* 63.11.1 ������������ �� �������� � ������������� ��� ������ � �������������� ��������; \n* 55.10 ������������ �������� � ������ ���� ��� ���������� ����������; \n* 61.10 ������������ � ������� ����� �� ���� ��������� ����������; \n* 58.19 ���� ������������ ������������ ������; \n\n\n", true);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandOkvedMultiple()
        {
            var message = new Message();
            message.Text = "/okved 7723836671 5262143887";

            var expected = ("�������� ���: 7723836671\n�������� ������������: 72.19 ������� ������������ � ���������� � ������� ������������ � ����������� ���� ������\n�������������� ������������: \n* 46.46 �������� ������� ���������������� ����������; \n* 46.90 �������� ������� ��������������������; \n* 41.20 ������������� ����� � ������� ������; \n* 33.13 ������ ������������ � ����������� ������������; \n* 95.12 ������ ����������������� ������������; \n* 26.30.17 ������������ �����- � ������������� ���������� ����������; \n* 25.40 ������������ ������ � �����������; \n* 26.70 ������������ ���������� ��������, ����- � ����������������; \n* 32.50 ������������ ����������� ������������ � ������������; \n* 85.23 ���������� ������ ������ ������������; \n* 86.21 ����� ��������� ��������; \n* 85.42 ����������� ���������������� ��������������; \n* 33.20 ������ ������������ ����� � ������������; \n* 58.14 ������� �������� � ������������� �������; \n* 56.10.1 ������������ ���������� � ���� � ������ ����������� �������������, ����������, ���������� �������� ������� � ����������������; \n* 73.11 ������������ ��������� ��������; \n* 63.11.1 ������������ �� �������� � ������������� ��� ������ � �������������� ��������; \n* 55.10 ������������ �������� � ������ ���� ��� ���������� ����������; \n* 61.10 ������������ � ������� ����� �� ���� ��������� ����������; \n* 58.19 ���� ������������ ������������ ������; \n\n\n�������� ���: 5262143887\n�������� ������������: 51.18.27 ������������ ������� �� ������� �������� ������� ��������, ��  ����������� � ������ �����������\n�������������� ������������: \n* 50.20.2 ����������� ������������ � ������ ������ ���������������� �������; \n* 50.20.1 ����������� ������������ � ������ �������� �����������; \n* 52.6 ��������� �������� ��� ���������; \n* 50.30.2 ��������� �������� �������������� ��������, ������ �  ����������������; \n* 50.20.3 �������������� ������ ����� ����� �� ������������ ������������  ���������������� �������; \n* 51.54.3 ������� �������� ������� �������������; \n* 51.47 ������� �������� ������� �������������������� ����������������  ��������; \n* 50.30.1 ������� �������� �������������� ��������, ������ � ����������������; \n* 51.18.2 ������������ �������, ������������������ �� ������� �������� ��������,  �� ����������� � ������ �����������; \n\n\n", true);
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

            var expected = ("����� ���������� � ����! ������� /help ����� ������� ������ ��������� ������.", true); 
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCommandIncorrect()
        {
            var message = new Message();
            message.Text = "/delete";

            var expected = ("��� ��������� �� �������� ����� �� ��������������� ������.\n" +
                        "������� /help ����� ������� ������ ��������� ������.", false);
            var result = _telegramBot.Command(message, _history).Result;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}