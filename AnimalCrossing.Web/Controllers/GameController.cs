using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalCrossing.Web.Entities;
using AnimalCrossing.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AnimalCrossing.Web.Controllers
{
    [Route("api/Game")]
    public class GameController : Controller
    {
        private readonly GameRepository _gameRepository;
        private readonly VillagerRepository _villagerRepository;

        public GameController(GameRepository gameRepository, VillagerRepository villagerRepository)
        {
            _gameRepository = gameRepository;
            _villagerRepository = villagerRepository;
        }

        [HttpGet]
        public ActionResult<Game> Create([FromQuery]GameMode mode)
        {
            var game = _gameRepository.Create(mode, _villagerRepository.Villagers);

            return Ok(game);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<Game> Get(Guid id)
        {
            var game = _gameRepository.Get(id);

            if (game != null)
                return Ok(game);

            return BadRequest();
        }

        [HttpPost("Guess")]
        public ActionResult<GuessResponse> Guess([FromBody]GuessRequest guessRequest)
        {
            var game = _gameRepository.Get(guessRequest.GameId);

            if (game != null)
            {
                var success = game.Guess(guessRequest.Name);
                var guessResponse = new GuessResponse(game, success);

                return Ok(guessResponse);
            }

            return BadRequest();
        }

        [HttpPost("Skip")]
        public ActionResult<Game> Skip([FromBody]SkipRequest skipRequest)
        {
            var game = _gameRepository.Get(skipRequest.GameId);

            if (game != null)
            {
                game.Skip();

                return Ok(game);
            }

            return BadRequest();
        }
    }
}