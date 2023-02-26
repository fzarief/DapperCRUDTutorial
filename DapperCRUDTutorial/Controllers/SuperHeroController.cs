using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace DapperCRUDTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }



        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("Select * from superheroes where Id = @Id",
                new { Id = heroId });
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));


            await connection.ExecuteAsync("insert into superheroes(sname, firstname, lastname) values (@SName, @FirstName, @LastName)", hero);
            return Ok(await SelectAllHeroes(connection));

        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update superheroes set sname=@SName, firstname=@FirstName, lastname=@LastName where id=@Id", hero);
            return Ok(await SelectAllHeroes(connection));

        }
        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from superheroes where id=@Id", new { Id = heroId });
            return Ok(await SelectAllHeroes(connection));

        }


        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("Select * from superheroes");
        }
    }
    
       


}
