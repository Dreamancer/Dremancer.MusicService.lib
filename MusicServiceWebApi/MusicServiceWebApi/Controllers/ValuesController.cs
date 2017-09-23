using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MusicServiceWebApi.ApiWrappers;
using Newtonsoft.Json.Linq;
using MusicServiceWebApi.MusicRepo;
using MusicServiceWebApi.Models;

namespace MusicServiceWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IMusicRepo _repo;

        public ValuesController(IConfiguration config)
        {
            _config = config;
            _repo = new MusicRepo.MusicRepo(config);
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("search/{name}")]
        public IEnumerable<Artist> Search(string name)
        {
            return _repo.SearchArtists(name);
        }

        // GET api/values/5
        [HttpGet("artist/s/{id}")]
        public Artist GetBySpotifyId(string id)
        {
            return _repo.GetArtist(id);
            
        }

        [HttpGet("artist/m/{id}")]
        public Artist GetByMbId(string id)
        {
            return _repo.GetArtist(Guid.Parse(id));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpDelete("db/clear/{table}")]
        public void ClearDb(string tableName)
        {
            _repo.ClearTempContext(tableName);
        }
    }
}
