using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Entities
{
    public class Villager
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Species { get; set; }

        public string Gender { get; set; }

        public string Personality { get; set; }

        public string Birthday { get; set; }

        public string Catchphrase { get; set; }

        public string ImageFileName { get; set; }

        public string HouseFileName { get; set; }
    }
}
