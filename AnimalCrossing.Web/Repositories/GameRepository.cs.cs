using AnimalCrossing.Web.Entities;
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
        private IMemoryCache _cache;

        public GameRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Game Create(IEnumerable<Villager> villagers)
        {
            var game = new Game(villagers);
            var cacheKey = GetCacheKey(game.Id);

            _cache.Set(cacheKey, game, new MemoryCacheEntryOptions() { SlidingExpiration = new TimeSpan(2, 0, 0, 0) });

            return game;
        }

        public Game Get(Guid id)
        {
            var cacheKey = GetCacheKey(id);
            var game = _cache.Get<Game>(cacheKey);

            return game;
        }

        private string GetCacheKey(Guid id)
        {
            return $"Game:{id}";
        }
    }
}
