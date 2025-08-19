namespace HizmetPinari.Api.Dtos;
public class AdminRegisterDto {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AdminKey { get; set; } = string.Empty;
}