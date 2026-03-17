using System.IO;
using GameLibrary.Wpf.Models;
using Microsoft.Data.Sqlite;

namespace GameLibrary.Wpf.Services
{
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string Username { get; set; } = string.Empty;
        public double TotalHours { get; set; }
        public int CompletedGames { get; set; }
        public int TotalGames { get; set; }
        public double AverageRating { get; set; }
        public int UnlockedAchievements { get; set; }
    }

    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gamelibrary.db");
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username     TEXT    NOT NULL UNIQUE,
                    Email        TEXT    NOT NULL,
                    PasswordHash TEXT    NOT NULL,
                    Role         TEXT    NOT NULL DEFAULT 'User',
                    CreatedAt    DATETIME DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS Games (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title        TEXT    NOT NULL,
                    Genre        TEXT    NOT NULL,
                    Platform     TEXT    NOT NULL,
                    Rating       INTEGER DEFAULT 0,
                    Status       TEXT    NOT NULL DEFAULT 'Queued',
                    HoursPlayed  REAL    DEFAULT 0,
                    ReleaseDate  DATETIME,
                    AddedDate    DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Notes        TEXT    DEFAULT '',
                    Level        INTEGER DEFAULT 1,
                    UserId       INTEGER NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );

                CREATE TABLE IF NOT EXISTS Achievements (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    GameId       INTEGER NOT NULL,
                    UserId       INTEGER NOT NULL,
                    Title        TEXT    NOT NULL,
                    Description  TEXT    DEFAULT '',
                    IsUnlocked   INTEGER DEFAULT 0,
                    UnlockedDate DATETIME,
                    CreatedDate  DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (GameId) REFERENCES Games(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
            ";
            cmd.ExecuteNonQuery();

            // Migration: add Level column to existing databases
            try
            {
                cmd.CommandText = "ALTER TABLE Games ADD COLUMN Level INTEGER DEFAULT 1";
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException) { /* Column already exists */ }

            // Seed admin user if none exists
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin'";
            var count = Convert.ToInt64(cmd.ExecuteScalar());
            if (count == 0)
            {
                cmd.CommandText = @"INSERT INTO Users (Username, Email, PasswordHash, Role)
                                    VALUES ($username, $email, $hash, $role)";
                cmd.Parameters.AddWithValue("$username", "admin");
                cmd.Parameters.AddWithValue("$email", "admin@gamelibrary.com");
                cmd.Parameters.AddWithValue("$hash", AuthService.HashPassword("admin123"));
                cmd.Parameters.AddWithValue("$role", "Admin");
                cmd.ExecuteNonQuery();
            }
        }

        // ===== USERS =====

        public User? GetUser(string username, string passwordHash)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, Email, PasswordHash, Role, CreatedAt FROM Users WHERE Username = $username AND PasswordHash = $hash";
            cmd.Parameters.AddWithValue("$username", username);
            cmd.Parameters.AddWithValue("$hash", passwordHash);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    Role = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                };
            }
            return null;
        }

        public bool UsernameExists(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = $username";
            cmd.Parameters.AddWithValue("$username", username);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public void AddUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Users (Username, Email, PasswordHash, Role)
                                VALUES ($username, $email, $hash, $role)";
            cmd.Parameters.AddWithValue("$username", user.Username);
            cmd.Parameters.AddWithValue("$email", user.Email);
            cmd.Parameters.AddWithValue("$hash", user.PasswordHash);
            cmd.Parameters.AddWithValue("$role", user.Role);
            cmd.ExecuteNonQuery();
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, Email, Role, CreatedAt FROM Users";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Role = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }
            return users;
        }

        public void UpdateUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Users SET Username = $username, Email = $email, Role = $role WHERE Id = $id";
            cmd.Parameters.AddWithValue("$username", user.Username);
            cmd.Parameters.AddWithValue("$email", user.Email);
            cmd.Parameters.AddWithValue("$role", user.Role);
            cmd.Parameters.AddWithValue("$id", user.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Achievements WHERE UserId = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Games WHERE UserId = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Users WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        // ===== GAMES =====

        private const string GameSelectColumns = "Id, Title, Genre, Platform, Rating, Status, HoursPlayed, ReleaseDate, AddedDate, Notes, Level, UserId";

        public List<Game> GetGamesByUser(int userId)
        {
            var games = new List<Game>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT {GameSelectColumns} FROM Games WHERE UserId = $userId";
            cmd.Parameters.AddWithValue("$userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                games.Add(ReadGame(reader));
            }
            return games;
        }

        public List<Game> GetAllGames()
        {
            var games = new List<Game>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT {GameSelectColumns} FROM Games";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                games.Add(ReadGame(reader));
            }
            return games;
        }

        private Game ReadGame(SqliteDataReader reader)
        {
            return new Game
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Genre = reader.GetString(2),
                Platform = reader.GetString(3),
                Rating = reader.GetInt32(4),
                Status = reader.GetString(5),
                HoursPlayed = reader.GetDouble(6),
                ReleaseDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                AddedDate = reader.GetDateTime(8),
                Notes = reader.IsDBNull(9) ? "" : reader.GetString(9),
                Level = reader.GetInt32(10),
                UserId = reader.GetInt32(11)
            };
        }

        public void AddGame(Game game)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Games (Title, Genre, Platform, Rating, Status, HoursPlayed, ReleaseDate, Notes, Level, UserId)
                                VALUES ($title, $genre, $platform, $rating, $status, $hours, $release, $notes, $level, $userId)";
            cmd.Parameters.AddWithValue("$title", game.Title);
            cmd.Parameters.AddWithValue("$genre", game.Genre);
            cmd.Parameters.AddWithValue("$platform", game.Platform);
            cmd.Parameters.AddWithValue("$rating", game.Rating);
            cmd.Parameters.AddWithValue("$status", game.Status);
            cmd.Parameters.AddWithValue("$hours", game.HoursPlayed);
            cmd.Parameters.AddWithValue("$release", (object?)game.ReleaseDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$notes", game.Notes);
            cmd.Parameters.AddWithValue("$level", game.Level);
            cmd.Parameters.AddWithValue("$userId", game.UserId);
            cmd.ExecuteNonQuery();
        }

        public void UpdateGame(Game game)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Games SET Title=$title, Genre=$genre, Platform=$platform,
                                Rating=$rating, Status=$status, HoursPlayed=$hours,
                                ReleaseDate=$release, Notes=$notes, Level=$level WHERE Id=$id";
            cmd.Parameters.AddWithValue("$title", game.Title);
            cmd.Parameters.AddWithValue("$genre", game.Genre);
            cmd.Parameters.AddWithValue("$platform", game.Platform);
            cmd.Parameters.AddWithValue("$rating", game.Rating);
            cmd.Parameters.AddWithValue("$status", game.Status);
            cmd.Parameters.AddWithValue("$hours", game.HoursPlayed);
            cmd.Parameters.AddWithValue("$release", (object?)game.ReleaseDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$notes", game.Notes);
            cmd.Parameters.AddWithValue("$level", game.Level);
            cmd.Parameters.AddWithValue("$id", game.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteGame(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Achievements WHERE GameId = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Games WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public bool GameExistsForUser(string title, int userId, int? excludeId = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            if (excludeId.HasValue)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Games WHERE Title = $title AND UserId = $userId AND Id != $excludeId";
                cmd.Parameters.AddWithValue("$excludeId", excludeId.Value);
            }
            else
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Games WHERE Title = $title AND UserId = $userId";
            }
            cmd.Parameters.AddWithValue("$title", title);
            cmd.Parameters.AddWithValue("$userId", userId);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        // ===== ACHIEVEMENTS =====

        public List<Achievement> GetAchievementsByGame(int gameId)
        {
            var list = new List<Achievement>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, GameId, UserId, Title, Description, IsUnlocked, UnlockedDate, CreatedDate FROM Achievements WHERE GameId = $gameId";
            cmd.Parameters.AddWithValue("$gameId", gameId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(ReadAchievement(reader));
            return list;
        }

        public List<Achievement> GetAchievementsByUser(int userId)
        {
            var list = new List<Achievement>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, GameId, UserId, Title, Description, IsUnlocked, UnlockedDate, CreatedDate FROM Achievements WHERE UserId = $userId";
            cmd.Parameters.AddWithValue("$userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(ReadAchievement(reader));
            return list;
        }

        private Achievement ReadAchievement(SqliteDataReader reader)
        {
            return new Achievement
            {
                Id = reader.GetInt32(0),
                GameId = reader.GetInt32(1),
                UserId = reader.GetInt32(2),
                Title = reader.GetString(3),
                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                IsUnlocked = reader.GetInt32(5) == 1,
                UnlockedDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                CreatedDate = reader.GetDateTime(7)
            };
        }

        public void AddAchievement(Achievement a)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Achievements (GameId, UserId, Title, Description, IsUnlocked, UnlockedDate)
                                VALUES ($gameId, $userId, $title, $desc, $unlocked, $unlockedDate)";
            cmd.Parameters.AddWithValue("$gameId", a.GameId);
            cmd.Parameters.AddWithValue("$userId", a.UserId);
            cmd.Parameters.AddWithValue("$title", a.Title);
            cmd.Parameters.AddWithValue("$desc", a.Description);
            cmd.Parameters.AddWithValue("$unlocked", a.IsUnlocked ? 1 : 0);
            cmd.Parameters.AddWithValue("$unlockedDate", (object?)a.UnlockedDate ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAchievement(Achievement a)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Achievements SET IsUnlocked=$unlocked, UnlockedDate=$unlockedDate WHERE Id=$id";
            cmd.Parameters.AddWithValue("$unlocked", a.IsUnlocked ? 1 : 0);
            cmd.Parameters.AddWithValue("$unlockedDate", (object?)a.UnlockedDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$id", a.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAchievement(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Achievements WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public int GetTotalAchievementsCount(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Achievements WHERE UserId = $userId";
            cmd.Parameters.AddWithValue("$userId", userId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetUnlockedAchievementsCount(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Achievements WHERE UserId = $userId AND IsUnlocked = 1";
            cmd.Parameters.AddWithValue("$userId", userId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ===== STATISTICS =====

        public int GetTotalGamesCount(int userId) => GetGamesByUser(userId).Count;

        public double GetTotalHoursPlayed(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(SUM(HoursPlayed), 0) FROM Games WHERE UserId = $userId";
            cmd.Parameters.AddWithValue("$userId", userId);
            return Convert.ToDouble(cmd.ExecuteScalar());
        }

        public Dictionary<string, int> GetGamesByStatus(int userId)
        {
            var result = new Dictionary<string, int>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Status, COUNT(*) FROM Games WHERE UserId = $userId GROUP BY Status";
            cmd.Parameters.AddWithValue("$userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }
            return result;
        }

        public Dictionary<string, int> GetGamesByGenre(int userId)
        {
            var result = new Dictionary<string, int>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Genre, COUNT(*) FROM Games WHERE UserId = $userId GROUP BY Genre";
            cmd.Parameters.AddWithValue("$userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }
            return result;
        }

        public double GetAverageRating(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(AVG(CAST(Rating AS REAL)), 0) FROM Games WHERE UserId = $userId AND Rating > 0";
            cmd.Parameters.AddWithValue("$userId", userId);
            return Convert.ToDouble(cmd.ExecuteScalar());
        }

        // ===== LEADERBOARD =====

        public List<LeaderboardEntry> GetLeaderboard()
        {
            var entries = new List<LeaderboardEntry>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT u.Username,
                       COALESCE(SUM(g.HoursPlayed), 0) AS TotalHours,
                       COUNT(CASE WHEN g.Status = 'Completed' THEN 1 END) AS CompletedGames,
                       COUNT(g.Id) AS TotalGames,
                       COALESCE(AVG(CASE WHEN g.Rating > 0 THEN CAST(g.Rating AS REAL) END), 0) AS AvgRating,
                       (SELECT COUNT(*) FROM Achievements a WHERE a.UserId = u.Id AND a.IsUnlocked = 1) AS UnlockedAch
                FROM Users u
                LEFT JOIN Games g ON u.Id = g.UserId
                WHERE u.Role != 'Guest'
                GROUP BY u.Id, u.Username
                ORDER BY TotalHours DESC";
            using var reader = cmd.ExecuteReader();
            int rank = 1;
            while (reader.Read())
            {
                entries.Add(new LeaderboardEntry
                {
                    Rank = rank++,
                    Username = reader.GetString(0),
                    TotalHours = reader.GetDouble(1),
                    CompletedGames = reader.GetInt32(2),
                    TotalGames = reader.GetInt32(3),
                    AverageRating = reader.GetDouble(4),
                    UnlockedAchievements = reader.GetInt32(5)
                });
            }
            return entries;
        }
    }
}
