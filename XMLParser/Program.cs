using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XMLParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        public static async Task App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XDocument doc = XDocument.Load("https://www.cbr.ru/scripts/XML_val.asp?d=0");
            var valutas = doc.Element("Valuta").Elements("Item").ToList();
            ConsoleKey? exitBtn = null;
            while (exitBtn != ConsoleKey.Spacebar)
            {
                Console.Clear();
                for (int i = 0; i < valutas.Count; i++)
                {
                    Console.WriteLine($"№{i}\t|\t{valutas[i].Element("Name")?.Value}");
                }
                var selectedVal = string.Empty;

                while (string.IsNullOrEmpty(selectedVal) || !selectedVal.All(char.IsDigit) || int.Parse(selectedVal) >= valutas.Count)
                {
                    Console.WriteLine("Введите номер желаемой валюты!");
                    selectedVal = Console.ReadLine();
                }

                var today = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                var kotirovka = XDocument.Load($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={today}&date_req2={today}&VAL_NM_RQ={valutas[int.Parse(selectedVal)].Element("ParentCode")?.Value}");
                var curs = kotirovka.Element("ValCurs")?.Element("Record")?.Element("Value")?.Value;
                if (string.IsNullOrEmpty(curs))
                    await Console.Out.WriteLineAsync($"Для этой валюты данных нету");


                else
                    await Console.Out.WriteLineAsync($"Сегодня {valutas[int.Parse(selectedVal)].Element("Name")?.Value} стоит {curs}");
                await Console.Out.WriteLineAsync("Нажмите любую кнопку чтобы продолжить. Пробел чтобы выйти.");
                exitBtn = Console.ReadKey().Key;
            }
        }
    }
}