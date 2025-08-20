using System.ComponentModel.DataAnnotations;

namespace CommandsService;

public class CommandCreateDto
{
  [Required]
  public string HowTo { get; set; } = string.Empty;
  [Required]
  public string CommandLine { get; set; } = string.Empty;
}
