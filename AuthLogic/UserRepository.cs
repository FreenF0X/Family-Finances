using Dapper;
using Npgsql;
using Domain.Models;

namespace AuthLogic
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<AppUser> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await connection.QuerySingleOrDefaultAsync<AppUser>(sql, new { Email = email });
        }

        public async Task<int> CreateAsync(AppUser user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Users (Email, PasswordHash) 
                    VALUES (@Email, @PasswordHash) 
                    RETURNING Id";
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }
    }
}
