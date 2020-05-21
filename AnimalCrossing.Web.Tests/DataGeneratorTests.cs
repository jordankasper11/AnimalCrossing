using AnimalCrossing.Web.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Tests
{
    [TestClass]
    public class DataGeneratorTests
    {
        [TestMethod]
        public async Task GetVillagersJson()
        {
            var villagers = new List<Villager>();
            var directoryInfo = new DirectoryInfo(@"C:\Projects\villagerdb-master\data\villagers");

            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                var contents = await File.ReadAllTextAsync(fileInfo.FullName);
                var json = JsonDocument.Parse(contents);

                if (json.RootElement.TryGetProperty("games", out JsonElement games) && games.TryGetProperty("nh", out JsonElement newHorizons))
                {
                    var id = json.RootElement.GetProperty("id").GetString();
                    var villager = new Villager();

                    villager.Id = Guid.NewGuid();
                    villager.Name = json.RootElement.GetProperty("name").GetString();
                    villager.Species = json.RootElement.GetProperty("species").GetString();
                    villager.Gender = json.RootElement.GetProperty("gender").GetString();
                    villager.Birthday = json.RootElement.GetProperty("birthday").GetString();
                    villager.Personality = newHorizons.GetProperty("personality").GetString();
                    villager.Catchphrase = newHorizons.GetProperty("phrase").GetString();
                    villager.ImageFileName = await DownloadFile($"https://villagers.club/assets/villagers/medium/{id.ToLower()}.png", $@"C:\Projects\AnimalCrossing\AnimalCrossing.Web\images", $"villager-{Guid.NewGuid()}.png");
                    villager.HouseFileName = await CopyHouseFile(villager.Name, @"C:\Projects\AC Houses", $@"C:\Projects\AnimalCrossing\AnimalCrossing.Web\images", $"house-{Guid.NewGuid()}.png");

                    villagers.Add(villager);
                }
            }

            var output = JsonSerializer.Serialize(villagers);

            Assert.IsTrue(!String.IsNullOrWhiteSpace(output));

            File.WriteAllText(@"C:\Projects\AnimalCrossing\AnimalCrossing.Web\data\villagers.json", output);
        }

        private async Task<string> DownloadFile(string sourceUrl, string destinationPath, string fileName)
        {
            var filePath = $@"{ destinationPath}\{fileName}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(sourceUrl);

                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await responseStream.CopyToAsync(fileStream);
                }
            }

            return fileName;
        }

        private async Task<string> CopyHouseFile(string name, string sourcePath, string destinationPath, string fileName)
        {
            var directoryInfo = new DirectoryInfo(sourcePath);
            var fileInfo = directoryInfo.GetFiles().FirstOrDefault(f => f.Name.EndsWith($"_{name}.png", StringComparison.OrdinalIgnoreCase));

            if (fileInfo == null)
                throw new InvalidOperationException($"Missing file for {name}");

            fileInfo.CopyTo($@"{destinationPath.TrimEnd('\\')}\{fileName}");

            return fileName;
        }
    }
}
