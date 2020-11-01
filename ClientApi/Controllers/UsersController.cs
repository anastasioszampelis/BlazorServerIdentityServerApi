using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClientApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        public UsersController(IUsersRepository usersRepository)
        {
            this._usersRepository = usersRepository;
        }
        // GET: api/<UsersController>
        [HttpGet]
        [Authorize (Roles = "Admin")]
        public IActionResult Get()
        {
            var response = _usersRepository.GetUsers();
            if (response.Item1)
            {
                return Ok(response.Item2);
            }
            else
            {
                return BadRequest(response.Item3);
            }
        }

        //// GET api/<UsersController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<UsersController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<UsersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<UsersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
