using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Entities
{
    public class GuessRequest
    {
        public Guid GameId { get; set; }

        public string Name { get; set; }
    }

    public class GuessResponse
    {
        public Game Game { get; set; }

        public bool Success {get;set;}

        public GuessResponse(Game game, bool success)
        {
            this.Game = game;
            this.Success = success;
        }
    }

    public class SkipRequest
    {
        public Guid GameId { get; set; }
    }

    public class CurrentVillager
    {
        public Guid Id { get; private set; }

        public string HouseImageUrl { get; private set; }

        public CurrentVillager(Villager villager)
        {
            this.Id = villager.Id;
            this.HouseImageUrl = $"/images/houses/{villager.HouseFileName}";
        }
    }

    public class Game
    {
        public Guid Id { get; set; }

        public CurrentVillager CurrentVillager
        {
            get
            {
                if (this.RemainingVillagers?.FirstOrDefault() != null)
                    return new CurrentVillager(this.RemainingVillagers.First());

                return null;
            }
        }

        public bool Completed
        {
            get
            {
                return !this.RemainingVillagers.Any();
            }
        }

        public int WrongGuesses { get; private set; }

        public int Skips { get; private set; }

        private List<Villager> RemainingVillagers { get; set; }

        private List<Villager> CompletedVillagers { get; set; }

        public Game()
        {
            this.RemainingVillagers = new List<Villager>();
            this.CompletedVillagers = new List<Villager>();
        }

        public Game(IEnumerable<Villager> villagers)
        {
            this.Id = Guid.NewGuid();
            this.RemainingVillagers = villagers.OrderBy(v => Guid.NewGuid()).ToList();
            this.CompletedVillagers = new List<Villager>();
        }

        public bool Guess(string name)
        {
            name = name.Trim();

            EnsureGameNotCompleted();

            var villager = this.RemainingVillagers.First();

            if (name.Equals(villager.Name, StringComparison.OrdinalIgnoreCase))
            {
                MoveToNextVillager();

                return true;
            }
            else
            {
                this.WrongGuesses++;

                return false;
            }
        }

        public void Skip()
        {
            EnsureGameNotCompleted();

            this.Skips++;

            MoveToNextVillager();
        }

        private void EnsureGameNotCompleted()
        {
            if (this.Completed)
                throw new InvalidOperationException("There are no villagers remaining in this game");
        }

        private void MoveToNextVillager()
        {
            var villager = this.RemainingVillagers.First();

            this.RemainingVillagers.Remove(villager);
            this.CompletedVillagers.Add(villager);
        }
    }
}
