namespace AIDebate.Api.Models;

public class Verdict
{
  public Guid Id{get; set;} = Guid.NewGuid();
  public Guid DebateId {get; set;}

  public string Outcome {get; set;} = string.Empty;
  public string Summary {get; set;} = string.Empty;
  public Double Confidence {get; set;} = 0.0;
  public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

  public Debate Debate {get; set;} = null!;

}