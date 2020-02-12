using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Entities
{
    public class GuessResponse
    {
        public Game Game { get; set; }

        public bool Success { get; set; }

        public GuessResponse(Game game, bool success)
        {
            this.Game = game;
            this.Success = success;
        }
    }
}