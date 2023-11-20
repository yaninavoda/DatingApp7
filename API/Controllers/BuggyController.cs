using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly DataContext _context;

    public BuggyController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var user = _context.Users.Find(-1);
        return user == null ?
        NotFound() :
        user;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var nullUser = _context.Users.Find(-1);
        var willCauseNullReferenceException = nullUser.ToString();
        return willCauseNullReferenceException;
        
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This is a bad request");
    }
}
