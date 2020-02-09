using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Entities
{
    public class Villager : ICloneable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string HouseImageUrl
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.HouseFileName))
                    return $"/images/houses/{this.HouseFileName}";

                return null;
            }
        }

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

        public object Clone()
        {
            var villager = new Villager();

            villager.Id = this.Id;
            villager.Name = this.Name;
            villager.Url = this.Url;
            villager.HouseFileName = this.HouseFileName;

            return villager;
        }
    }
}
