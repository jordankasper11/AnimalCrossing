using AnimalCrossing.Web.Caching;
using AnimalCrossing.Web.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Repositories
{
    public class GameRepository
    {
        private VillagerRepository VillagerRepository { get; set; }

        private CacheManager CacheManager { get; set; }

        public GameRepository(VillagerRepository villagerRepository, CacheManager cacheManager)
        {
            this.VillagerRepository = villagerRepository;
            CacheManager = cacheManager;
        }

        public async Task<Game> Create(GameType gameType, GameMode gameMode)
        {
            var firstVillagerId = this.VillagerRepository.Villagers.OrderBy(v => Guid.NewGuid()).First().Id;
            var gameData = new GameData(gameType, gameMode, firstVillagerId);
            var game = new Game(gameData, this.VillagerRepository.Villagers);

            await Save(game);

            return game;
        }

        public async Task<Game> Get(Guid id)
        {
            var cacheKey = GetCacheKey(id);
            var gameData = await this.CacheManager.Get<GameData>(cacheKey);

            return new Game(gameData, this.VillagerRepository.Villagers);
        }

        public async Task Save(Game game)
        {
            var gameData = game.GameData;
            var cacheKey = GetCacheKey(gameData.Id);

            await CacheManager.Set(cacheKey, gameData, slidingExpiration: TimeSpan.FromHours(3));
        }

        public async Task Remove(Guid gameId)
        {
            var cacheKey = GetCacheKey(gameId);

            await CacheManager.Remove(cacheKey);
        }

        private string GetCacheKey(Guid id)
        {
            return $"Game:{id}";
        }
    }
}
