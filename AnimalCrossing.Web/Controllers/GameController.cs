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
        private GameRepository GameRepository { get; set; }

        public GameController(GameRepository gameRepository)
        {
            this.GameRepository = gameRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Game>> Create([FromQuery]GameMode mode, [FromQuery]Guid? previousGameId = null)
        {
            var game = await GameRepository.Create(mode);

            if (previousGameId != null)
                await GameRepository.Remove(previousGameId.Value);

            return Ok(game);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Game>> Get(Guid id)
        {
            var game = await GameRepository.Get(id);

            if (game != null)
                return Ok(game);

            return BadRequest();
        }

        [HttpPost("Guess")]
        public async Task<ActionResult<GuessResponse>> Guess([FromBody]GuessRequest guessRequest)
        {
            var game = await GameRepository.Get(guessRequest.GameId);

            if (game != null)
            {
                var success = game.Guess(guessRequest.Name);
                var guessResponse = new GuessResponse(game, success);

                await GameRepository.Save(game);

                return Ok(guessResponse);
            }

            return BadRequest();
        }

        [HttpPost("Skip")]
        public async Task<ActionResult<Game>> Skip([FromBody]SkipRequest skipRequest)
        {
            var game = await GameRepository.Get(skipRequest.GameId);

            if (game != null)
            {
                game.Skip();

                await GameRepository.Save(game);

                return Ok(game);
            }

            return BadRequest();
        }
    }
}