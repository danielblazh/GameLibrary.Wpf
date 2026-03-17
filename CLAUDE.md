# CLAUDE.md

## Project Overview
GameLibrary.Wpf is a WPF desktop application for managing and tracking video games. Built with .NET 8, MVVM architecture, SQLite database, and Hebrew UI.

## Build & Run
```bash
dotnet build GameLibrary.Wpf.sln
dotnet run --project src/GameLibrary.Wpf/GameLibrary.Wpf.csproj
```

## Architecture
- **MVVM** — All logic in ViewModels, no code-behind logic (except PasswordBox binding)
- **Navigation** — `NavigationService` + DataTemplate mapping in `MainWindow.xaml` (ViewModel type → View)
- **Database** — SQLite via `Microsoft.Data.Sqlite` (ADO.NET, no EF). All queries in `DatabaseService.cs`
- **Theme** — Dark/Light mode via `DynamicResource` + swapping `ResourceDictionary` at runtime

## Project Structure (under `src/GameLibrary.Wpf/`)
- `Models/` — POCOs: User, Game, Achievement
- `ViewModels/` — BaseViewModel, RelayCommand, and one ViewModel per screen
- `Views/` — XAML UserControls (one per screen)
- `Services/` — DatabaseService, AuthService, NavigationService
- `Converters/` — IValueConverter implementations (6 total)
- `Assets/Styles/` — Colors.xaml, AppStyles.xaml, DarkTheme.xaml, LightTheme.xaml

## Key Conventions
- Hebrew UI text — use Unicode escapes (`\u05XX`) in C# strings, HTML entities (`&#x05XX;`) in XAML
- Margins/Padding must be multiples of 8 (4/8/16/24/32)
- All `ResourceDictionary` colors use `DynamicResource` (not `StaticResource`) for theme switching
- Use `ObservableCollection<T>` for UI-bound lists, never `List<T>`
- Commands via `RelayCommand` with `CanExecute` for button disable logic
- SQL always uses parameterized queries (`$param`) — no string concatenation

## Permissions / Roles
- **Guest** — Main page only
- **User** — Dashboard, game library, statistics, leaderboard, achievements
- **Admin** — All of the above + user management (AdminDashboard)

## Default Credentials
- Admin: `admin` / `admin123`

## Database
SQLite file created at runtime in the app's base directory (`gamelibrary.db`). Schema auto-migrates (CREATE IF NOT EXISTS + ALTER TABLE in try/catch).

### Tables
- `Users` — Id, Username, Email, PasswordHash, Role, CreatedAt
- `Games` — Id, Title, Genre, Platform, Rating, Status, HoursPlayed, ReleaseDate, AddedDate, Notes, Level, UserId
- `Achievements` — Id, GameId, UserId, Title, Description, IsUnlocked, UnlockedDate, CreatedDate
