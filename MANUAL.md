### Разворачивание Identity Server 4 и АРМ его администрирования
1. Создать файлы миграции БД:
### В NUGET консоле
Add-Migration AspNetIdentityDbInit -context AdminIdentityDbContext -output Data/Migrations/Identity
Add-Migration LoggingDbInit -context AdminLogDbContext -output Data/Migrations/Logging
Add-Migration IdentityServerConfigurationDbInit -context IdentityServerConfigurationDbContext -output Data/Migrations/IdentityServerConfiguration
Add-Migration IdentityServerPersistedGrantsDbInit -context IdentityServerPersistedGrantDbContext -output Data/Migrations/IdentityServerGrants
### Or via `dotnet CLI`
dotnet ef migrations add AspNetIdentityDbInit -c AdminIdentityDbContext -o Data/Migrations/Identity
dotnet ef migrations add LoggingDbInit -c AdminLogDbContext -o Data/Migrations/Logging
dotnet ef migrations add IdentityServerConfigurationDbInit -c IdentityServerConfigurationDbContext -o Data/Migrations/IdentityServerConfiguration
dotnet ef migrations add IdentityServerPersistedGrantsDbInit -c IdentityServerPersistedGrantDbContext -o Data/Migrations/IdentityServerGrants

2. Развернуть в Docker.
3. Для отладки на локальном ПК дополнительно нужно:
  - открыть в брэндмауре порт 5000
  - в хост добавить 10.0.75.1 local.docker.ru
  - в файле докера .env указать домен MLK_PROD_EXTERNAL_DNS_NAME_OR_IP=local.docker.ru
3. В АРМ администратора загрузить через меню "Клиенты/Ресурсы->Загрузка конфигурации" настройку в папке "configs" "config.json" прежде заменив домен "localhost" на необходимый
4. Из БД свормировать файл для загрузки пользователей запросом:
  SELECT json_agg(u)
  FROM (
      SELECT * FROM dbo.user_profiles u
      INNER JOIN dbo.user_profiles_info uf ON u.id = uf.id
  ) u;
5. В АРМ администратора загрузить через меню "Пользователи/Роли->Загрузка пользователей" сформированный файл с пользователями.
  Примечание: Добавляются только новые пользователи
