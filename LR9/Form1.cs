using System;
using RestSharp;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LR9
{
    public partial class Form1 : Form
    {
        // Словарь для хранения текущих курсов валют относительно базовой валюты (RUB)
        private Dictionary<string, double> exchangeRates = new Dictionary<string, double>();

        // Массив доступных валют, которые можно выбрать для конвертации
        private string[] currencies = { "RUB", "BYN", "USD", "CNY", "TRY" };

        // API-ключ для доступа к сервису обменных курсов
        private const string apiKey = "ce86080dbc8b37458e073e59791aa386";

        // URL API для получения курсов валют
        private const string apiUrl = "http://api.exchangeratesapi.io/v1/latest";

        // Конструктор формы
        public Form1()
        {
            InitializeComponent();
        }

        // Метод, выполняющийся при загрузке формы
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Загружаем начальные курсы валют
            await GetExchangeRatesAsync();

            // Устанавливаем интервал таймера для обновления курсов каждые 10 минут (600 000 миллисекунд)
            timer1.Interval = 600000;

            // Запускаем таймер
            timer1.Start();
        }

        // Обработчик события таймера для автоматического обновления курсов
        private async void timer_Tick(object sender, EventArgs e)
        {
            // Обновляем курсы валют, когда срабатывает таймер
            await GetExchangeRatesAsync();
        }

        // Метод для конвертации валюты
        private void ConvertCurrency()
        {
            // Попытка преобразовать текст в число
            if (!double.TryParse(textBox1.Text, out double amount))
            {
                // Если преобразование не удалось, выводим сообщение об ошибке
                MessageBox.Show("Пожалуйста, введите корректное числовое значение в поле суммы.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Прерываем выполнение метода, если ввод некорректен
            }

            // Получаем выбранные валюты из выпадающих списков
            string fromCurrency = comboBox1.SelectedItem?.ToString();
            string toCurrency = comboBox2.SelectedItem?.ToString();

            // Проверка, что пользователь выбрал обе валюты
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
            {
                MessageBox.Show("Пожалуйста, выберите валюты для конвертации.", "Ошибка выбора", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка наличия курсов для выбранных валют
            if (exchangeRates.ContainsKey(fromCurrency) && exchangeRates.ContainsKey(toCurrency))
            {
                // Получаем курсы для выбранных валют
                double fromRate = exchangeRates[fromCurrency];
                double toRate = exchangeRates[toCurrency];

                // Конвертируем сумму на основе соотношения курсов
                double result = amount * (toRate / fromRate);

                // Отображаем результат конвертации в текстовом поле
                textBox2.Text = $"{amount} {fromCurrency} = {result:F2} {toCurrency}";
            }
            else
            {
                // Если курсы не найдены, выводим сообщение об ошибке
                MessageBox.Show("Выберите корректные валюты", "Ошибка валют", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Метод для обновления отображения курсов валют на форме
        private async void UpdateExchangeRatesDisplay()
        {
            //меняем цвет на красный на 1 секунду
            lblRubRate.ForeColor = Color.Red;
            lblBynRate.ForeColor = Color.Red;
            lblUsdRate.ForeColor = Color.Red;
            lblCnyRate.ForeColor = Color.Red;
            lblTryRate.ForeColor = Color.Red;

            // Отображаем курсы валют по отношению к RUB
            lblRubRate.Text = $"{exchangeRates["RUB"]}";
            lblBynRate.Text = $"{exchangeRates["BYN"]}";
            lblUsdRate.Text = $"{exchangeRates["USD"]}";
            lblCnyRate.Text = $"{exchangeRates["CNY"]}";
            lblTryRate.Text = $"{exchangeRates["TRY"]}";

            // Асинхронная пауза на 1 секунду
            await Task.Delay(1000);

            // Возвращаем цвет к стандартному (например, черному)
            lblRubRate.ForeColor = Color.Black;
            lblBynRate.ForeColor = Color.Black;
            lblUsdRate.ForeColor = Color.Black;
            lblCnyRate.ForeColor = Color.Black;
            lblTryRate.ForeColor = Color.Black;

        }

        // Асинхронный метод для получения курсов валют с API
        private async Task GetExchangeRatesAsync()
        {
            // Создаем HTTP-клиент для обращения к API
            var client = new RestClient(apiUrl);
            var request = new RestRequest();

            // Добавляем параметры запроса: API-ключ, валюты и базовую валюту (RUB)
            request.AddParameter("access_key", apiKey);
            request.AddParameter("symbols", string.Join(",", currencies));
            request.AddParameter("base", "EUR");

            // Выполняем запрос и получаем ответ от API
            var response = await client.ExecuteAsync<ExchangeRatesResponse>(request);

            // Проверяем успешность запроса и наличие данных
            if (response.IsSuccessful && response.Data != null)
            {
                // Обновляем словарь exchangeRates новыми курсами из ответа
                exchangeRates = response.Data.Rates;

                // Обновляем отображение курсов на форме
                UpdateExchangeRatesDisplay();
            }
            else
            {
                // Если запрос неудачен, выводим сообщение об ошибке
                MessageBox.Show("Ошибка при получении курсов валют");
            }
        }

        // Обработчик события кнопки для выполнения конвертации валют
        private async void btnConvert_Click(object sender, EventArgs e)
        {
            // Вызываем метод конвертации валют
            ConvertCurrency();
        }
    }
}
