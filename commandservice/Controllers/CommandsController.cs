using AutoMapper;
using commandservice.Data;
using commandservice.Dtos;
using commandservice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Windows.Input;

namespace commandservice.Controllers
{
    [Route("api/c/platforms/{PlatformId}/[Controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public  ActionResult < IEnumerable < CommandReadDto >> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform : {platformId}");
            if (_repository.PlatformExists(platformId))
            {
                var commands = _repository.GetCommandsForPlatform(platformId);
                return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
            }
            else
            {
                return NotFound("Platform Not Found");
            }
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _repository.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform : {platformId}");
            if (_repository.PlatformExists(platformId))
            {
                var command = _mapper.Map<Command>(commandDto);
                _repository.CreateCommand(platformId, command);
                _repository.SaveChanges();
                
                var commandReadDto = _mapper.Map<CommandReadDto>(command);
                return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id },commandReadDto );
            }
            else
            {
                return NotFound("Platform Not Found");
            }
        }
    }
}
