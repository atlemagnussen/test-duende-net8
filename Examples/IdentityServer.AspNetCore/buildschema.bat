rmdir /S /Q "Data/Migrations"

dotnet ef migrations add Users -c ApplicationDbContext -o Data/Migrations

dotnet ef migrations script -c ApplicationDbContext -o Data/Migrations/ApplicationDb.sql