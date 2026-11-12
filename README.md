# ⚖️ AI Decision Helper (Multi-Agent Arena)

An end-to-end Multi-Agent AI system built with **ASP.NET Core**, where multiple AI models (PRO, CON, CRITIC, and ADVISOR) debate on a given topic to help users clear their thoughts and make better decisions.

## 🚀 Tech Stack
- **Framework:** .NET 10 (ASP.NET Core Web API)
- **Database:** SQLite (via Entity Framework Core)
- **Architecture:** Layered Architecture (Controllers, Services, DTOs, Models), Strategy Pattern (Smart Router)
- **AI Integration:** Prompt Engineering, Groq API, OpenRouter API
- **Testing:** xUnit, Moq, FluentAssertions (Implemented)
- **Tools:** Swagger UI, Git, HTML/JS Frontend

## ✨ Features
- **Multi-Agent Architecture:** Orchestrates 4 distinct AI roles (PRO, CON, CRITIC, and ADVISOR).
- **Parallel Execution:** PRO and CON agents run concurrently using `Task.WhenAll` to reduce latency by 50%.
- **Smart Routing & Prompt Caching:** Optimizes API calls and structures prompts to leverage Groq's automatic prompt caching for faster responses.
- **Natural Language Decision Helper:** Instead of a rigid "Judge", the AI acts as a Decision Advisor, providing nuanced advice, a confidence score out of 10, and a disclaimer.
- **Structured Summarization:** Automatically parses AI responses to separate the detailed argument from a short 2-3 sentence summary for quick reading.
- **Data Persistence:** Saves the debate, turns (with summaries), and final advisor verdict securely in SQLite.
- **Frontend UI:** Includes a clean HTML/JS interface to interact with the API visually.

## 📡 API Endpoints

### 1. Create a Debate
`POST /api/Debate`
**Request Body:**
```json
{
  "question": "Is remote work better than office work?"
}
