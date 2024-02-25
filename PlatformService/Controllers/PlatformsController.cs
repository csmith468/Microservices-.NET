using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.HTTP;

namespace PlatformService.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase 
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() 
        {
            Console.WriteLine("--> Getting platforms");
            var platforms = _repo.GetAllPlatforms();
            // should check if null
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        // Name allows us to give the route later
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id) 
        {
            Console.WriteLine("--> Getting platform id " + id.ToString());
            var platform = _repo.GetPlatformById(id);
            if (platform != null) return Ok(_mapper.Map<PlatformReadDto>(platform));
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto) 
        {
            Console.WriteLine("--> Creating platform.");
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repo.CreatePlatform(platformModel);
            var result = _repo.SaveChanges();

            if (!result) return BadRequest("--> Error on creating platform");

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Tell command service that a new platform has been created
            try {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex) {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            // This gives you the route where you can find the entity in HEADERS
            return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto );
        }
    }
}