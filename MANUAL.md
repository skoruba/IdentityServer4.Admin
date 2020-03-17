### Разворачивание Identity Server 4 и АРМ его администрирования
1. Создать файлы миграции БД:
Для linux:
  установить:
    dotnet add package Microsoft.EntityFrameworkCore.Tools --version 3.1.2
  переименовать в файле add-migrations.ps1 слэшь \ на /
  $targetContexts = @{ 
    AdminIdentityDbContext                = "Migrations/Identity"
    AdminLogDbContext                     = "Migrations/Logging";
    IdentityServerConfigurationDbContext  = "Migrations/IdentityServerConfiguration";
    IdentityServerPersistedGrantDbContext = "Migrations/IdentityServerGrants";
    AdminAuditLogDbContext                = "Migrations/AuditLogging";
  }
Запустить с папки build
.\add-migrations.ps1 -migration dbIdtInit -migrationProviderName PostgreSQL

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
