using AIDebate.Api.Data;
using AIDebate.Api.Dtos;
using AIDebate.Api.Models;
using System.Text.Json;

namespace AIDebate.Api.Services;

public class DebateOrchestrator
{
    private readonly ILlmService _llm; 
    private readonly AppDbContext _db;

    public DebateOrchestrator(ILlmService llm, AppDbContext db)
    {
        _llm = llm;
        _db = db;
    }

    // Helper Method: AI ke jawab ko Content aur Summary mein todne ke liye
    private (string content, string summary) ParseResponse(string rawResponse)
    {
        var delimiter = "### SUMMARY ###";
        var parts = rawResponse.Split(delimiter);

        string content = parts[0].Trim();
        string summary = parts.Length > 1 ? parts[1].Trim() : "No summary generated.";

        return (content, summary);
    }

    public async Task<DebateResponseDto> RunAsync(string question)
    {
        var debate = new Debate
        {
            Question = question,
            Status = "running",
            UserId = null
        };
        _db.Debates.Add(debate);
        await _db.SaveChangesAsync();

        try
        {
            // ==========================================
            // 1 & 2. PRO aur CON (Parallel Execution)
            // ==========================================
            var proSystemPrompt = @"You are an expert PRO debater. 
                Write a strong, detailed argument FOR the motion using real-world facts. 
                Do NOT hallucinate. 
                At the very end of your response, write exactly '### SUMMARY ###' and then provide a 2-3 sentence summary.";

            var conSystemPrompt = @"You are an expert CON debater. 
                Write a strong, detailed argument AGAINST the motion using real-world facts. 
                Do NOT hallucinate. 
                At the very end of your response, write exactly '### SUMMARY ###' and then provide a 2-3 sentence summary.";

            var proTask = _llm.CallAsync("groq/openai/gpt-oss-120b", proSystemPrompt, question);
            var conTask = _llm.CallAsync("groq/openai/gpt-oss-20b", conSystemPrompt, question);

            await Task.WhenAll(proTask, conTask);

            var proRaw = await proTask;
            var conRaw = await conTask;

            // Parse PRO and CON
            var (proContent, proSummary) = ParseResponse(proRaw);
            var (conContent, conSummary) = ParseResponse(conRaw);

            _db.Turns.Add(new Turn { DebateId = debate.Id, Side = "pro", Phase = "opening", Content = proContent, Summary = proSummary });
            _db.Turns.Add(new Turn { DebateId = debate.Id, Side = "con", Phase = "opening", Content = conContent, Summary = conSummary });
            await _db.SaveChangesAsync();

            // ==========================================
            // 3. CRITIC
            // ==========================================
            var criticSystemPrompt = @"You are an expert Debate Critic. 
                Analyze both sides and point out logical fallacies or weak evidence. 
                At the very end of your response, write exactly '### SUMMARY ###' and then provide a 2-3 sentence summary of your critique.";

            var criticUserContent = $@"
                The motion is: {question}.
                PRO argued: {(proContent.Length > 800 ? proContent.Substring(0, 800) : proContent)}
                CON argued: {(conContent.Length > 800 ? conContent.Substring(0, 800) : conContent)}";

            var criticRaw = await _llm.CallAsync("groq/openai/gpt-oss-20b", criticSystemPrompt, criticUserContent);

            // Parse CRITIC
            var (criticContent, criticSummary) = ParseResponse(criticRaw);

            _db.Turns.Add(new Turn { DebateId = debate.Id, Side = "critic", Phase = "critique", Content = criticContent, Summary = criticSummary });
            await _db.SaveChangesAsync();

            // ==========================================
            // 4. DECISION HELPER (Expert Advisor)
            // ==========================================
            var helperSystemPrompt = @"You are an Expert AI Decision Advisor. 
                Your goal is to help the user make a clear, informed decision about their topic. 
                
                You will be provided with arguments from a PRO side, a CON side, and a Critic. 
                Treat the PRO and CON as brainstorming inputs, and the Critic as a filter for weak logic. 
                
                Do NOT just pick a side. Instead, synthesize the information, use your own knowledge to fill in gaps or correct bad arguments, and provide the BEST POSSIBLE ADVICE for the user.

                Return ONLY a valid JSON object with these exact keys:
                - 'outcome': (string) Your direct recommendation. E.g., 'Go for it', 'Avoid it', 'Proceed with caution', 'It depends on your goals', etc.
                - 'confidence': (double) A number from 0 to 10 (e.g., 7.5)
                - 'summary': (string) Write a natural, conversational paragraph (max 5 sentences) following these rules:
                    1. Give your direct recommendation based on the synthesis of everything.
                    2. Give your confidence score out of 10.
                    3. Explain exactly WHY you deducted points (mention if PRO or CON missed a critical point or if the topic is highly subjective).
                    4. End with a disclaimer that this is an AI helper to clear their thoughts, and they should apply their own judgment before making a final decision.";

            var helperUserContent = $@"
                The user's topic/doubt is: {question}.
                
                Here is the raw debate context:
                PRO argued: {(proContent.Length > 800 ? proContent.Substring(0, 800) : proContent)}
                CON argued: {(conContent.Length > 800 ? conContent.Substring(0, 800) : conContent)}
                Critic Analysis: {criticSummary}";

            var helperContent = await _llm.CallAsync(
                "groq/openai/gpt-oss-120b", 
                helperSystemPrompt, 
                helperUserContent);

            var helperDto = JsonSerializer.Deserialize<JudgeResponseDto>(
                helperContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (helperDto == null)
                throw new Exception("Failed to deserialize helper response");

            var verdict = new Verdict
            {
                DebateId = debate.Id,
                Outcome = helperDto.Outcome,
                Summary = helperDto.Summary,
                Confidence = helperDto.Confidence
            };

            _db.Verdicts.Add(verdict);
            debate.Status = "completed";
            await _db.SaveChangesAsync();

            return await BuildResponseAsync(debate.Id);
        }
        catch
        {
            debate.Status = "failed";
            await _db.SaveChangesAsync();
            throw;
        }
    }

    private async Task<DebateResponseDto> BuildResponseAsync(Guid debateId)
    {
        var debate = await _db.Debates.FindAsync(debateId);
        var turns = _db.Turns.Where(t => t.DebateId == debateId).OrderBy(t => t.CreatedAt).ToList();
        var verdict = _db.Verdicts.FirstOrDefault(v => v.DebateId == debateId);

        return new DebateResponseDto
        {
            Id = debate!.Id,
            Question = debate.Question,
            Status = debate.Status,
            Turns = turns.Select(t => new TurnDto
            {
                Side = t.Side,
                Phase = t.Phase,
                Content = t.Content,
                Summary = t.Summary
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
        if (debate == null) return null;

        return await BuildResponseAsync(debateId);
    }
}