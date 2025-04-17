using Fetch.ReceiptProcessor.Controllers;
using Fetch.ReceiptProcessor.Helper;
using Fetch.ReceiptProcessor.Services;
using Fetch.ReceiptProcessor.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddSingleton<IPointsServiceAsync, PointsServiceAsync>();
builder.Services.AddSingleton<IReceiptStorage, ReceiptStorage>();
builder.Services.AddSingleton<IReceiptService, ReceiptService>();
builder.Services.AddSingleton<ILogger, Logger<ReceiptStorage>>();
builder.Services.AddSingleton<ILogger, Logger<PointsServiceAsync>>();
builder.Services.AddSingleton<ILogger, Logger<ReceiptsController>>();
builder.Services.AddSingleton<ILogger, Logger<ReceiptProcessorWorker>>();
builder.Services.AddSingleton<IReceiptQueue, ReceiptQueue>();

builder.Services.AddHostedService<ReceiptProcessorWorker>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();