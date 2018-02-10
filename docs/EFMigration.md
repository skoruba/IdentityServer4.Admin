# Create the migration of database:

```
Add-Migration Initial -context AdminDbContext -output Data/Migrations
Update-Database -context AdminDbContext
```

