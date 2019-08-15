using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Treasury.Models
{
    public class JsonCurrencyDataService
    {
        private readonly string currencyDataPath = @"Resources\SavedData\currencyData.json";
        private readonly string treasuryDataPath = @"Resources\SavedData\treasureData.json";
        private readonly string selectedCurrencyDataPath = @"Resources\SavedData\selectedCurrData.json";

        /// Currency data
        public IEnumerable<string> GetCurrencyData()
        {
            if (!File.Exists(currencyDataPath))
            {
                File.Create(currencyDataPath).Close();
            }
            var serializedContacts = File.ReadAllText(currencyDataPath);
            var deserializedContacts = JsonConvert.DeserializeObject<IEnumerable<string>>(serializedContacts);

            if (deserializedContacts == null)
                return new List<string>()
                {
                    "$ - USD",
                    "£ - GBP",
                    "€ - Euro",
                    "₪ - New Israeli Shekel",
                    "¥ - Yen",
                    "₩ - South Korean Won",
                    "₺ - Turkish lira",
                    "zł - Polish złoty"
                };

            return deserializedContacts;
        }
        public void SaveCurrencyData(IEnumerable<string> data)
        {
            var serializedContact = JsonConvert.SerializeObject(data);
                File.WriteAllText(currencyDataPath, serializedContact);
        }
        /////////////////////////
        /// Treasury data
        public IEnumerable<MyTreasuryInfo> GetTreasuryData()
        {
            if (!File.Exists(treasuryDataPath))
            {
                File.Create(treasuryDataPath).Close();
            }
            var serializedContacts = File.ReadAllText(treasuryDataPath);
            var deserializedContacts = JsonConvert.DeserializeObject<IEnumerable<MyTreasuryInfo>>(serializedContacts);

            if (deserializedContacts == null)
                return new List<MyTreasuryInfo>();

            return deserializedContacts;
        }
        public void SaveTreasuryData(IEnumerable<MyTreasuryInfo> data)
        {
            var serializedContact = JsonConvert.SerializeObject(data);
            File.WriteAllText(treasuryDataPath, serializedContact);
        }
        /////////////////////////
        /// Selected currency data
        public string GetSelectedCurrencyData()
        {
            if (!File.Exists(selectedCurrencyDataPath))
            {
                File.Create(selectedCurrencyDataPath).Close();
            }
            var serializedContacts = File.ReadAllText(selectedCurrencyDataPath);
            var deserializedContacts = JsonConvert.DeserializeObject<string>(serializedContacts);

            if (deserializedContacts == null)
                return string.Empty;

            return deserializedContacts;
        }
        public void SaveSelectedCurrencyData(string data)
        {
            var serializedContact = JsonConvert.SerializeObject(data);
            File.WriteAllText(selectedCurrencyDataPath, serializedContact);
        }
    }
}
