using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnimalCrossingTest.Web.Entities
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
}
