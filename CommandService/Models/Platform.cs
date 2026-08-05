using System.ComponentModel.DataAnnotations;

namespace CommandService.Models
{
    public record PlatformCreateDto(int Id, string Name);
    public record PlatformReadDto(int Id, string Name);
    public record PlatformPublishedDto(int Id, string Name, string Event);
    public record GenericEventDto(string Event);
    public class Platform
    {
        [Key]
        public required int Id { get; set; }
        public required int ExternalId { get; set; }
        public required string Name { get; set; }
        public ICollection<Command> Commands { get; set; } = [];
    }
}
