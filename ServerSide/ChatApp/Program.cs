using ChatApp.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(option => option.AddDefaultPolicy(opt => opt
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(x=>true)
));
var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MessageHub>("/messagehub");
});

app.MapGet("/", () => "Hello World!");

app.Run();
