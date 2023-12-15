using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Voltage.Business.Services.Abstract;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Voltage.Controllers;

[Authorize(Roles = "User, Admin")]
[Route("[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IFriendListService friendListService;

    public ValuesController(IFriendListService friendListService)
    {
        this.friendListService = friendListService;
    }

    [HttpPost("User")]
    public async Task<string> GetUserr()
    {
        var d = await friendListService.GetUserDtoByNameAsync("Royal_45");
        return JsonSerializer.Serialize(d);
    }

    // GET: api/<ValuesController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<ValuesController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<ValuesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
