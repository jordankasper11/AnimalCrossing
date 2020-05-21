using AnimalCrossing.Web.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Repositories
{
    public class VillagerRepository
    {
        public ReadOnlyCollection<Villager> Villagers { get; private set; }

        public VillagerRepository(string filePath)
        {
            this.Villagers = GetVillagers(filePath);
        }

        private ReadOnlyCollection<Villager> GetVillagers(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var villagers = JsonSerializer.Deserialize<List<Villager>>(json);

            return villagers.AsReadOnly();
        }
    }
}
