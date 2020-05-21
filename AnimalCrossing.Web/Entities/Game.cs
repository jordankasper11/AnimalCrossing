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

    public class CurrentVillager
    {
        public Guid Id { get; private set; }

        public string HouseFileName { get; private set; }

        public CurrentVillager(Villager villager)
        {
            this.Id = villager.Id;
            this.HouseFileName = villager.HouseFileName;
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
        public Guid Id => this.GameData.Id;

        public GameMode Mode => this.GameData.Mode;

        public bool Completed => !this.RemainingVillagers.Any();

        public int CorrectGuesses => this.GameData.CorrectGuesses;

        public int WrongGuesses => this.GameData.WrongGuesses;

        public int Skips => this.GameData.Skips;

        public int Remaining => this.RemainingVillagers.Count;

        public Villager PreviousVillager => this.GameData.CompletedVillagerIds?.Any() == true ? this.Villagers.First(v => v.Id == this.GameData.CompletedVillagerIds.Last()) : null;

        public CurrentVillager CurrentVillager => this.GameData.CurrentVillagerId != null ? new CurrentVillager(this.Villagers.First(v => v.Id == this.GameData.CurrentVillagerId.Value)) : null;

        public List<VillagerOption> Options
        {
            get
            {
                if (!this.Completed && this.Mode == GameMode.MultipleChoice)
                {
                    var villagers = this.Villagers
                        .Where(v => v.Id != this.GameData.CurrentVillagerId.Value)
                        .OrderBy(v => Guid.NewGuid())
                        .Take(9)
                        .ToList();

                    var currentVillager = this.Villagers.First(v => v.Id == this.GameData.CurrentVillagerId.Value);

                    villagers.Add(currentVillager);

                    return villagers
                        .OrderBy(v => v.Name)
                        .Select(v => new VillagerOption(v))
                        .ToList();
                }

                return null;
            }
        }

        internal GameData GameData { get; private set; }

        private List<Villager> Villagers { get; set; }

        private ReadOnlyCollection<Villager> RemainingVillagers
        {
            get
            {
                return this.Villagers
                    .Where(v => !this.GameData.CompletedVillagerIds.Contains(v.Id))
                    .ToList()
                    .AsReadOnly();
            }
        }

        private ReadOnlyCollection<Villager> CompletedVillagers
        {
            get
            {
                return this.Villagers
                    .Where(v => this.GameData.CompletedVillagerIds.Contains(v.Id))
                    .ToList()
                    .AsReadOnly();
            }
        }

        public Game(GameData gameData, IEnumerable<Villager> villagers)
        {
            this.GameData = gameData;
            this.Villagers = villagers.ToList();
        }

        public bool Guess(string name)
        {
            EnsureGameNotCompleted();

            name = name.Trim();

            var success = false;
            var villager = this.Villagers.First(v => v.Id == this.GameData.CurrentVillagerId);

            if (name.Equals(villager.Name, StringComparison.OrdinalIgnoreCase))
            {
                this.GameData.CorrectGuesses++;

                success = true;
            }
            else
                this.GameData.WrongGuesses++;

            if (success || this.Mode == GameMode.MultipleChoice)
                MoveToNextVillager();

            return success;
        }

        public void Skip()
        {
            EnsureGameNotCompleted();

            this.GameData.Skips++;

            MoveToNextVillager();
        }

        private void EnsureGameNotCompleted()
        {
            if (this.Completed)
                throw new InvalidOperationException("There are no villagers remaining in this game");
        }

        private void MoveToNextVillager()
        {
            this.GameData.CompletedVillagerIds.Add(this.CurrentVillager.Id);
            this.GameData.CurrentVillagerId = this.RemainingVillagers.OrderBy(v => Guid.NewGuid()).FirstOrDefault()?.Id;
        }
    }

    public class GameData
    {
        public Guid Id { get; set; }

        public GameMode Mode { get; set; }

        public Guid? CurrentVillagerId { get; set; }

        public List<Guid> CompletedVillagerIds { get; set; }

        public int CorrectGuesses { get; set; }

        public int WrongGuesses { get; set; }

        public int Skips { get; set; }

        public GameData()
        {
            this.Id = Guid.NewGuid();
            this.CompletedVillagerIds = new List<Guid>();
        }

        public GameData(GameMode mode, Guid currentVillagerId) : this()
        {
            this.Mode = mode;
            this.CurrentVillagerId = currentVillagerId;
        }
    }
}
