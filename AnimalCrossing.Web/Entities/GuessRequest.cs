using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Entities
{
    public class GuessRequest
    {
        public Guid GameId { get; set; }

        public string Name { get; set; }
    }
}
