namespace AIDebate.Api.Models;

    public class Debate
    {
      public Guid Id { get; set;} = Guid.NewGuid();

      public Guid? UserId {get; set;}

      public string Question {get; set;} = string.Empty;
      public string Status {get; set;} = "running";
      public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

      public User? User {get; set;} = null;
      public List<Turn> Turns {get; set;} = new();
      public Verdict? Verdict {get; set;}

      
    }
