using Love2Cook.API.Data;
using Love2Cook.API.Models;
using Love2Cook.API.DTOs;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Love2Cook.API.Services {
    public interface IAuthService {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
    }

    public class AuthService : IAuthService {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // Перевірка чи email вже існує
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email)) {
                return null; // Email already exists
            }

            // Перевірка чи username вже існує
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username)) {
                return null; // Username already exists
            }

            // Хешування пароля
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Створення нового користувача
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                RoleId = 1, // Standard user
                IsVip = false,
                DefaultServings = dto.DefaultServings,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Повернення даних користувача (без реального JWT для MVP)
            return new AuthResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Token = GenerateSimpleToken(user.UserId), // Простий токен для MVP
                IsVip = user.IsVip,
                DefaultServings = user.DefaultServings,
                RoleId = user.RoleId
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Пошук користувача за email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user == null) {
                return null; // User not found
            }

            // Перевірка пароля
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) {
                return null; // Invalid password
            }

            // Оновлення last_login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Повернення даних користувача
            return new AuthResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Token = GenerateSimpleToken(user.UserId),
                IsVip = user.IsVip,
                DefaultServings = user.DefaultServings,
                RoleId = user.RoleId
            };
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

            if (user == null) {
                return null;
            }

            return new UserProfileDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsVip = user.IsVip,
                DefaultServings = user.DefaultServings,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt
            };
        }

        // Простий токен для MVP (не справжній JWT)
        private string GenerateSimpleToken(int userId)
        {
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"user_{userId}_{DateTime.UtcNow.Ticks}")
            );
        }
    }
}