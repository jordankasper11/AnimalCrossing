using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Entities
{
    public enum GameMode
    {
        Guess,
        MultipleChoice
    }

    public class GuessRequest
    {
        public Guid GameId { get; set; }

        public string Name { get; set; }
    }

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
            this.HouseImageUrl = villager.HouseImageUrl;
        }
    }

    public class VillagerOption
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public VillagerOption(Villager villager)
        {
            this.Id = villager.Id;
            this.Name = villager.Name;
        }
    }

    public class Game
    {
        public Guid Id { get; set; }

        public GameMode Mode { get; set; }

        public bool Completed
        {
            get
            {
                return !this.RemainingVillagers.Any();
            }
        }

        public int CorrectGuesses { get; private set; }

        public int WrongGuesses { get; private set; }

        public int Skips { get; private set; }

        public int Remaining
        {
            get
            {
                return this.RemainingVillagers.Count;
            }
        }

        public Villager PreviousVillager
        {
            get
            {
                return this.CompletedVillagers?.LastOrDefault();
            }
        }

        public CurrentVillager CurrentVillager
        {
            get
            {
                var villager = this.RemainingVillagers?.FirstOrDefault();
                
                if (villager != null)
                    return new CurrentVillager(villager);

                return null;
            }
        }

        public List<VillagerOption> Options { get; private set; }

        private Dictionary<Villager, bool> Villagers { get; set; }

        private ReadOnlyCollection<Villager> RemainingVillagers
        {
            get
            {
                return this.Villagers
                    .Where(v => !v.Value)
                    .Select(v => v.Key)
                    .ToList()
                    .AsReadOnly();
            }
        }

        private ReadOnlyCollection<Villager> CompletedVillagers
        {
            get
            {
                return this.Villagers
                    .Where(v => v.Value)
                    .Select(v => v.Key)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public Game(GameMode gameMode, IEnumerable<Villager> villagers)
        {
            this.Id = Guid.NewGuid();
            this.Mode = gameMode;

            this.Villagers = villagers
                .Select(v => (Villager)v.Clone())
                .OrderBy(v => Guid.NewGuid())
                .ToDictionary(v => v, v => false);

            SetOptions();
        }

        public bool Guess(string name)
        {
            name = name.Trim();

            EnsureGameNotCompleted();

            var success = false;
            var villager = this.RemainingVillagers.First();            

            if (name.Equals(villager.Name, StringComparison.OrdinalIgnoreCase))
            {
                this.CorrectGuesses++;

                success = true;
            }
            else
                this.WrongGuesses++;

            MoveToNextVillager();

            return success;
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

            this.Villagers[villager] = true;

            SetOptions();
        }

        private void SetOptions()
        {
            if (this.Mode == GameMode.MultipleChoice && this.RemainingVillagers?.Any() == true)
            {
                var villagers = this.Villagers.Keys
                    .Where(v => v != this.RemainingVillagers.First())
                    .OrderBy(v => Guid.NewGuid())
                    .Take(9)
                    .ToList();

                villagers.Add(this.RemainingVillagers.First());

                this.Options = villagers
                    .OrderBy(v => v.Name)
                    .Select(v => new VillagerOption(v))
                    .ToList();
            }
            else
                this.Options = null;
        }
    }
}
