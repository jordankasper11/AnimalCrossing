using System;
using System.Collections.Generic;
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
            this.HouseImageUrl = $"/images/houses/{villager.HouseFileName}";
        }
    }

    public class VillagerOption
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public bool Available { get; private set; }

        public VillagerOption(Villager villager)
        {
            this.Id = villager.Id;
            this.Name = villager.Name;
            this.Available = true;
        }

        public void SetAsUnvailable()
        {
            this.Available = false;
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

        public CurrentVillager CurrentVillager
        {
            get
            {
                if (this.RemainingVillagers?.FirstOrDefault() != null)
                    return new CurrentVillager(this.RemainingVillagers.First());

                return null;
            }
        }

        public List<VillagerOption> Options { get; private set; }

        private Dictionary<Villager, bool> Villagers { get; set; }

        private List<Villager> RemainingVillagers
        {
            get
            {
                return this.Villagers
                    .Where(v => !v.Value)
                    .Select(v => v.Key)
                    .ToList();
            }
        }

        private List<Villager> CompletedVillagers
        {
            get
            {
                return this.Villagers
                    .Where(v => v.Value)
                    .Select(v => v.Key)
                    .ToList();
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

            var villager = this.RemainingVillagers.First();

            if (name.Equals(villager.Name, StringComparison.OrdinalIgnoreCase))
            {
                this.CorrectGuesses++;

                MoveToNextVillager();
                SetOptions();

                return true;
            }
            else
            {
                this.WrongGuesses++;

                if (this.Options?.Any() == true)
                {
                    var option = this.Options.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                    if (option != null)
                        option.SetAsUnvailable();
                }

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

        private void SetOptions()
        {
            if (this.Mode == GameMode.MultipleChoice && this.RemainingVillagers?.Any() == true)
            {
                var villagers = this.Villagers.Keys
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
