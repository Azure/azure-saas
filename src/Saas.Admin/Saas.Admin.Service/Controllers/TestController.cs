using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Saas.Admin.Service.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TestController : ControllerBase
{
    private readonly ITest testman;

    private readonly IAdminGraphServices _graphservices;

    public TestController(ITest test, IAdminGraphServices graphservices)
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
        CUser res = await _graphservices.GetUser(id);
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
