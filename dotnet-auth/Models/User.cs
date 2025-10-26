using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_auth.Models;


[Table("User")]
public class User
{
    public int id { get; set; }
    public string username { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}