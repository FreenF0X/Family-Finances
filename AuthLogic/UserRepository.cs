using Dapper;
using Npgsql;
using Domain.Models;
using System.Data.Common;
using AuthLogic.Models;

namespace AuthLogic
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IdentityUser?> GetByIdAsync(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM identity_users WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<IdentityUser>(sql, new { Id = id });
        }

        public async Task<IdentityUser?> GetByLoginAsync(string login)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM identity_users WHERE login = @Login";
            return await connection.QueryFirstOrDefaultAsync<IdentityUser>(sql, new { Login = login });
        }

        public async Task<IdentityUser?> GetByRefreshTokenAsync(string refreshToken)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM identity_users WHERE refresh_token = @RefreshToken";
            return await connection.QueryFirstOrDefaultAsync<IdentityUser>(sql, new { RefreshToken = refreshToken });
        }

        public async Task CreateAsync(IdentityUser user)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
            INSERT INTO identity_users 
                (id, login, password_hash, email, created_at, refresh_token, refresh_token_expiry_time)
            VALUES 
                (@Id, @Login, @PasswordHash, @Email, @CreatedAt, @RefreshToken, @RefreshTokenExpiryTime)";
            await connection.ExecuteAsync(sql, user);
        }

        public async Task UpdateRefreshTokenAsync(Guid id, string? refreshToken, DateTime? expiry)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
            UPDATE identity_users 
            SET refresh_token = @RefreshToken, 
                refresh_token_expiry_time = @Expiry
            WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, RefreshToken = refreshToken, Expiry = expiry });
        }

        public async Task UpdatePasswordHashAsync(Guid id, string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE identity_users SET password_hash = @PasswordHash WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, PasswordHash = passwordHash });
        }
    }
}
