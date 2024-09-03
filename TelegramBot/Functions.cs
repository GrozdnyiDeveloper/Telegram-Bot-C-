using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class Functions
    {
        public HttpResponseMessage SendRequest(string baseUri, string requestUri)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(requestUri).Result;
            return response;
        }

        public async Task<(string, bool)> Command(Message message, Dictionary<long, string> history)
        {
            var text = "";
            var isCommand = true;
            switch (message.Text.ToLower())
            {
                case "/start":
                    text = "Добро пожаловать в бота! Введите /help чтобы увидеть список доступных команд.";
                    break;

                case "/help":
                    text = "/start – начать общение с ботом\n" +
                        "/help – вывести справку о доступных командах\n" +
                        "/hello – вывести ваше имя и фамилию, ваш email и ссылку на github\n" +
                        "/inn – получить наименования и адреса компаний по ИНН\n" +
                        "/last – повторить последнее действие бота";
                    break;

                case "/hello":
                    text = "Улесов Иван\n" +
                        "Email: ylesov.ivan@gmail.com\n" +
                        "Github: https://github.com/GrozdnyiDeveloper";
                    break;

                case not null when message.Text.ToLower().Contains("/inn"):
                    foreach (var inn in message.Text.ToLower().Split(" "))
                    {
                        if (Int64.TryParse(inn, out _))
                        {
                            HttpResponseMessage response = SendRequest("https://api.checko.ru/v2/", $"company?key={ConfigurationManager.AppSettings["CheckoApiKey"]}&inn={inn}");
                            if (response.IsSuccessStatusCode)
                            {
                                string jsonString = await response.Content.ReadAsStringAsync();
                                var json = JObject.Parse(jsonString);
                                string adresses = "";
                                foreach (var obj in json.SelectTokens("data.Подразд.Филиал[*]"))
                                {
                                    adresses += "* Название: " + obj.SelectToken("НаимПолн") + ", Адрес: " + obj.SelectToken("Адрес") + "; \n";
                                }
                                adresses = adresses == "" ? "* Отсутствуют" : adresses;
                                text += $"Введённый ИНН: {json.SelectToken("data.ИНН")}\n" +
                                    $"Полное наименование: {json.SelectToken("data.НаимПолн")}\n" +
                                    $"Юридический адрес: {json.SelectToken("data.ЮрАдрес.АдресРФ")}\n" +
                                    $"Филиалы: \n{adresses}\n\n";
                            }
                            else
                            {
                                text += "Произошла ошибка при получении запроса от API.\n" +
                                    $"Полученный ответ: {(int)response.StatusCode} {response.ReasonPhrase}\n\n";
                            }
                        }
                    }
                    text = (text == "" ? "Номер ИНН компании не был введён.\n" +
                        "Повторите запрос, введя ИНН после команды (/inn <ИНН номер>)." : text);
                    break;

                case "/last":
                    message.Text = history[message.From.Id];
                    (text, isCommand) = await Command(message, history);
                    return (text, isCommand);

                default:
                    text = "Это сообщение не является одной из предусмотренных команд.\n" +
                        "Введите /help чтобы увидеть список доступных команд.";
                    isCommand = false;
                    break;
            }
            return (text, isCommand);
        }
    }
}
