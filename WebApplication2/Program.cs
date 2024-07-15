using NLog.Web;
using WebApplication2.Service;

var builder = WebApplication.CreateBuilder(args).Inject();

builder.Host.UseNLog();

builder.Services.AddControllers().AddInject();
var path = Environment.CurrentDirectory+System.IO.Path.DirectorySeparatorChar+"Git";
builder.Services.AddSingleton(new GitService(path));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseInject("swagger");

app.MapControllers();

app.Run();