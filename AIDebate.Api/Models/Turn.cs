namespace AIDebate.Api.Models
{
  
  public class Turn
  {
    public Guid Id {get; set;} = Guid.NewGuid();
    public Guid DebateId {get; set;}

    public Debate? Debate {get; set;} = null;
    public string Side {get; set;} = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public string Content {get; set;} = string.Empty;

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;




  }
}