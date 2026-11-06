namespace AIDebate.Api.Dtos;

public class JudgeResponseDto
{
    public string Outcome { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public double Confidence { get; set; }
}