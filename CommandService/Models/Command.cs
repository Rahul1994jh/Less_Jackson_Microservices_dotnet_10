using System.ComponentModel.DataAnnotations;

namespace CommandService.Models
{
    public record CommandCreateDto(string HowTo, string CommandLine);
    public record CommandReadDto(int Id, string HowTo, string CommandLine, int PlatformId);
    public class Command
    {
        [Key]
        public required int Id { get; set; }
        public required string HowTo { get; set; }
        public required string CommandLine { get; set; }
        public required int PlatformId { get; set; }
        public Platform? Platform { get; set; }
    }
}
