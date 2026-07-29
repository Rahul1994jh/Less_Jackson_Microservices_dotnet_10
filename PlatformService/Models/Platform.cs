using System.ComponentModel.DataAnnotations;
using Mapster;

namespace PlatformService.Models
{
    public record PlatformReadDto(int Id, string Name, string Publisher, string Cost);
    public record PlatformCreateDto(string Name, string Publisher, string Cost);
    public class Platform
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Publisher { get; set; }
        public required string Cost { get; set; }
    }
}