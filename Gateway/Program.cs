var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Đăng ký service ở đây
services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Sau đó mới build app
var app = builder.Build();

app.UseRouting();
app.MapReverseProxy();
app.Run();