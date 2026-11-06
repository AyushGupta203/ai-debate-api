using AIDebate.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace AIDebate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DebateController: ControllerBase
{
  private readonly DebateOrchestrator _debateOrchestrator;

  public DebateController(DebateOrchestrator debateOrchestrator)
  {
    _debateOrchestrator = debateOrchestrator;
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateDebateRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Question))
    {
      return BadRequest(new { error = "Question cannot be empty." });
    }
    var result = await _debateOrchestrator.RunAsync(request.Question);
    return Ok(result);
  }
[HttpGet("{id}")]

public async Task<IActionResult> GetDebate(Guid id)
  {
    var result = await _debateOrchestrator.GetDebateAsync(id);

    if (result == null)
    {
      return NotFound(new { error = "Debate not found." });
    }
    return Ok(result);
  } 

 
}
 public class CreateDebateRequest
  {
    public string  Question { get; set; } = string.Empty;
  }