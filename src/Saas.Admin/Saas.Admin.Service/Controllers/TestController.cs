using Microsoft.AspNetCore.Mvc;
using Saas.Admin.Service.Interfaces;
using Saas.Admin.Service.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Saas.Admin.Service.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class TestController : ControllerBase
{
    private readonly ITest testman;

    private readonly IUserGraphService _graphservices;

    public TestController(ITest test, IUserGraphService graphservices)
    {
        testman = test;
        _graphservices = graphservices;
    }
    // GET: api/<TestController>
    [HttpGet]
    public string Get()
    {
        
        return testman.GetTestString();
    }

    // GET api/<TestController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        AppUser res = await _graphservices.GetUserInfoByEmail(id);
        return new JsonResult(res);
    }

    // POST api/<TestController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<TestController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<TestController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
