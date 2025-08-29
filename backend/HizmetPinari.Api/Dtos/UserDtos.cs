// Dtos/UserDtos.cs

public class UserRegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } // Bu zaten null olabildiği için dokunmaya gerek yok.
}

// Dtos/UserProfileDto.cs
public class UserProfileDto
{
    public long UserId { get; set; }
    public string Email { get; set; }= string.Empty;
    public string FirstName { get; set; }= string.Empty;
    public string LastName { get; set; }= string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

// Dtos/UpdateUserProfileDto.cs
public class UpdateUserProfileDto
{
    public string FirstName { get; set; }= string.Empty;
    public string LastName { get; set; }= string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
}