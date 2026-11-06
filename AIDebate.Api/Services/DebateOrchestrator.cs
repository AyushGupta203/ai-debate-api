using AIDebate.Api.Data;
using AIDebate.Api.Dtos;
using AIDebate.Api.Models;
using System.Text.Json;

namespace AIDebate.Api.Services;
public class DebateOrchestrator
{
  private readonly ILlmService _Llm;
  private readonly AppDbContext _db;

  public DebateOrchestrator(ILlmService Llm, AppDbContext db)
  {
    _Llm = Llm;
    _db = db;
  }


  public async Task<DebateResponseDto> RunAsync(string question)
  {
    var debate = new Debate
    {
      Question  =question,
      Status = "running",
      UserId = null
    };
    _db.Debates.Add(debate);
    await _db.SaveChangesAsync();

    try{
    var proContent = await _Llm.CallAsync("You are PRO debater. Argue for the motion", question);

    _db.Turns.Add(new Turn
    {
      DebateId = debate.Id,
      Side = "pro",
      Phase = "opening",
      Content = proContent
    });


    var conContent = await _Llm.CallAsync("You are CON debater. Argue against the motion", question);

    _db.Turns.Add(new Turn
    {
      DebateId = debate.Id,
      Side = "con",
      Phase = "opening",
      Content = conContent
    });

    await _db.SaveChangesAsync();

    var judgePrompt = "You are an impartial debate judge. Return JSON with outcome, summary, confidence.";
    var judgeContent = await _Llm.CallAsync(judgePrompt, question);

    var judgeDto = JsonSerializer.Deserialize<JudgeResponseDto>(judgeContent,
    new JsonSerializerOptions{PropertyNameCaseInsensitive  = true})
    ;
    if(judgeDto == null)
    
      throw new Exception("Failed to deserialize judge response");

      var verdict = new Verdict
      {
        DebateId = debate.Id,
        Outcome = judgeDto.Outcome,
        Summary = judgeDto.Summary,
        Confidence = judgeDto.Confidence
      };

      _db.Verdicts.Add(verdict);
      debate.Status = "completed";
      await _db.SaveChangesAsync();
      return await BuildResponseAsync(debate.Id);
    }

      catch{
        debate.Status = "failed";
        await _db.SaveChangesAsync();
        throw;
      
    }
  }

  private async Task<DebateResponseDto> BuildResponseAsync(Guid debateId)
  {
    var debate = await _db.Debates.FindAsync(debateId);
    var turns = _db.Turns.Where(t=>t.DebateId== debateId).ToList();
    var verdict = _db.Verdicts.FirstOrDefault(v=>v.DebateId== debateId);
     return new DebateResponseDto
     {
       Id = debate!.Id,
        Question = debate.Question,
        Status = debate.Status,
        Turns = turns.Select(t=> new TurnDto
        {
          Side = t.Side,
          Phase = t.Phase,
          Content = t.Content
        }).ToList(),
        Verdict = verdict == null ? null : new VerdictDto
        {
          Outcome = verdict.Outcome,
          Summary = verdict.Summary,
          Confidence = verdict.Confidence
        }
     };
  }


  public async Task<DebateResponseDto?> GetDebateAsync(Guid debateId)
  {
    var debate = await _db.Debates.FindAsync(debateId);
    if(debate == null) return null;

      return await BuildResponseAsync(debateId);
    
  }


}