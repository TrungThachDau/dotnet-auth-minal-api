


namespace dotnet_auth.Endpoints
{
  public static class AuthEndPoint
  {
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
      app.MapGet("/helloworld", () => "Hello World!")
         .WithName("HelloWorld");
    }
  }
}

