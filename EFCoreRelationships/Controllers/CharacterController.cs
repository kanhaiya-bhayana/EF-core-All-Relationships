using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRelationships.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly DataContext _context;
        public CharacterController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Character>>> Get(int userId)
        {
            var characters = await _context.Characters
                .Where(x => x.UserId == userId)
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .ToListAsync();

            return characters;
        }

        [HttpPost]
        public async Task<ActionResult<List<Character>>> Create(CreateCharacterDto requrest)
        {
            var user = await _context.Users.FindAsync(requrest.UserId);
            if (user == null)
                return NotFound();
            var newCharacter = new Character
            {
                Name = requrest.Name,
                RpgClass = requrest.RpgClass,
                User = user
            };
            _context.Characters.Add(newCharacter);
            await _context.SaveChangesAsync();

            return await Get(newCharacter.UserId);
        }

        [HttpPost("skill")]
        public async Task<ActionResult<Character>> AddCharacterSkill(AddCharacterSillDto requrest)
        {
            var character = await _context.Characters
                .Where(c => c.Id == requrest.CharacterId)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync();
            if (character == null)
                return NotFound();

            var skill = await _context.Skills.FindAsync(requrest.SkillId);
            if (skill == null)
                return NotFound();


            character.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return character;
        }
    }
}
