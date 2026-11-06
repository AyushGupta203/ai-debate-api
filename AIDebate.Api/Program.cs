using AIDebate.Api.Data;
using AIDebate.Api.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();



builder.Services.AddDbContext<AppDbContext>(option=>option.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ILlmService, FakeLlmService>();
builder.Services.AddScoped<DebateOrchestrator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
