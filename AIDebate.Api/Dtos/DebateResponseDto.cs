namespace AIDebate.Api.Dtos;
public class DebateResponseDto
{
    public Guid Id {get; set;}
    public string? Question {get; set;}
    public String? Status {get; set;}
    public List<TurnDto>? Turns {get; set;}
    public VerdictDto? Verdict {get; set;}

}

public class TurnDto
{
  public string Side {get; set;} = string.Empty;
  public string Phase {get; set;} = string.Empty;
  public string Content {get; set;} = string.Empty;
  
  public string Summary { get; set; } = string.Empty;
}

public class VerdictDto
{
  public string Outcome {get; set;} = string.Empty;
  public string Summary {get; set;} = string.Empty;
  public double Confidence {get; set;}
}