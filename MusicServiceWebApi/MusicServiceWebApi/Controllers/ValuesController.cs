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
            IEnumerable<Artist> artists = _repo.SearchArtists(name);
            return artists;
            //string result = "";
            //foreach(Artist artist in artists)
            //{
            //    result += artist.ToString();
            //}
            //return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
    }
}
