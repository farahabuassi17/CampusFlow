using System.ComponentModel.DataAnnotations;

namespace CampusFlow.Mvc.ViewModels;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
