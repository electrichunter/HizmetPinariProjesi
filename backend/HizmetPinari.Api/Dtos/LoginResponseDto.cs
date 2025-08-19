namespace HizmetPinari.Api.Dtos;
public class LoginResponseDto {
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}