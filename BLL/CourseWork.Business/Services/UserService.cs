using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseWork.Business.DTOs;
using CourseWork.Business.Exceptions;
using CourseWork.Business.Mapping;
using CourseWork.Data.Entities;
using CourseWork.Data.Enums;
using CourseWork.Data.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CourseWork.Business.Services;

public class UserService : IUserService
{
    private const int MinimumPasswordLength = 6;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        ValidateRegistration(dto);

        var existing = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (existing is not null)
        {
            throw new ValidationException("Користувач з такою електронною поштою вже існує.");
        }

        var user = new User
        {
            Email = dto.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Role = UserRole.Registered
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateAuthResult(user);
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant(), cancellationToken)
            ?? throw new UnauthorizedBusinessException("Невірна електронна пошта або пароль.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedBusinessException("Невірна електронна пошта або пароль.");
        }

        return CreateAuthResult(user);
    }

    public async Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Користувач", id);

        return EntityMapper.ToDto(user);
    }

    private static void ValidateRegistration(RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ValidationException("Електронна пошта обов'язкова.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < MinimumPasswordLength)
        {
            throw new ValidationException($"Пароль повинен містити щонайменше {MinimumPasswordLength} символів.");
        }
    }

    private AuthResultDto CreateAuthResult(User user)
    {
        var token = GenerateJwtToken(user);
        return new AuthResultDto(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            user.Role,
            token);
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? "CourseWorkSecretKeyForJwtTokenGeneration2024!";
        var issuer = _configuration["Jwt:Issuer"] ?? "CourseWorkAgency";
        var audience = _configuration["Jwt:Audience"] ?? "CourseWorkAgencyUsers";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
