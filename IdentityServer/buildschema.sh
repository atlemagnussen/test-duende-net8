#!/bin/bash

rm -rf Migrations

dotnet ef migrations add Users -c ApplicationDbContext -o Data/Migrations
dotnet ef migrations add Grants -c PersistedGrantDbContext -o Data/Migrations/PersistedGrantDb
dotnet ef migrations add Configuration -c ConfigurationDbContext -o Data/Migrations/ConfigurationDb
dotnet ef migrations add DataProtection -c DataProtectionDbContext -o Data/Migrations/DataProtectionDb

dotnet ef migrations script -c ApplicationDbContext -o Data/Migrations/ApplicationDb.sql
dotnet ef migrations script -c PersistedGrantDbContext -o Data/Migrations/PersistedGrantDb.sql
dotnet ef migrations script -c ConfigurationDbContext -o Data/Migrations/ConfigurationDb.sql
dotnet ef migrations script -c DataProtectionDbContext -o Data/Migrations/DataProtectionDb.sql
