using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Tests
{
    public class Villager
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string HouseFileName { get; set; }

        public Villager()
        {
        }

        public Villager(Guid id, string name, string url, string houseFileName)
        {
            this.Id = id;
            this.Name = name;
            this.Url = url;
            this.HouseFileName = houseFileName;
        }
    }

    [TestClass]
    public class DataGeneratorTests
    {
        [TestMethod]
        public async Task GetVillagersJson()
        {
            var contents = await File.ReadAllTextAsync("C:\\Projects\\AnimalCrossing\\Source.txt");
            var hyperlinks = Regex.Matches(contents, "<a href=\"(?<VillagerUrl>[^\"]+)\">(?:<strong>)?(?<VillagerName>[^<]+)(?:</strong>)?</a>", RegexOptions.IgnoreCase);
            var houseImages = Regex.Matches(contents, "(?<=<img src=\")[^\"]+\\.jpe?g", RegexOptions.IgnoreCase);
            var villagers = new List<Villager>();

            for (var i = 0; i < hyperlinks.Count; i++)
            {
                var hyperLink = hyperlinks[i];
                var houseImage = houseImages[i];
                var id = Guid.NewGuid();
                var name = hyperLink.Groups["VillagerName"].Value;
                var url = hyperLink.Groups["VillagerUrl"].Value;
                var houseFileName = await SaveHouseImage(id, houseImage.Value);

                villagers.Add(new Villager(id, name, url, houseFileName));
            }

            var json = JsonSerializer.Serialize(villagers);

            Assert.IsTrue(!String.IsNullOrWhiteSpace(json));
        }

        private async Task<string> SaveHouseImage(Guid id, string url)
        {
            var fileName = $"{id}-house.jpg";
            var filePath = $"C:\\Projects\\AnimalCrossing\\images\\{fileName}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await responseStream.CopyToAsync(fileStream);
                }
            }

            return fileName;
        }
    }
}
