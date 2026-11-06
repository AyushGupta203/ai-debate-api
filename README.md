# ⚖️ AI Debate API

An end-to-end AI orchestration system built with **ASP.NET Core**, where two AI agents (PRO and CON) debate on a given topic, and an impartial AI Judge evaluates the arguments to deliver a structured verdict.

## 🚀 Tech Stack
- **Framework:** .NET 10 (ASP.NET Core Web API)
- **Database:** SQLite (via Entity Framework Core)
- **Architecture:** Layered Architecture (Controllers, Services, DTOs, Models)
- **AI Integration:** Prompt Engineering, JSON structured output parsing
- **Tools:** Swagger UI, Git, xUnit (planned)

## ✨ Features
- **Debate Orchestration:** Automatically triggers PRO and CON arguments based on the user's question.
- **AI Judge:** Evaluates the debate and returns a structured JSON verdict (Outcome, Summary, Confidence Score).
- **Graceful Parsing:** Robust JSON parsing for LLM outputs with case-insensitive matching.
- **Data Persistence:** Saves the debate, turns, and verdict securely in SQLite.
- **Clean API Design:** Uses DTOs (Data Transfer Objects) to prevent over-posting and expose only relevant data.

## 📡 API Endpoints

### 1. Create a Debate
`POST /api/Debate`
**Request Body:**
```json
{
  "question": "Should AI be regulated?"
}
