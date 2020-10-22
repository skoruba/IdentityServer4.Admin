USE OAuth2
GO

-- ==========================================================================================================
-- Author:		Mario Alberto Rojas Zúñiga
-- Create date: 24/07/2020
-- Description:	Se realizan cambios estructurales para compatibilidad con la version 4.0 de IdentityServer4
-- ==========================================================================================================

SET XACT_ABORT ON;
GO

BEGIN TRAN

DECLARE @sqlCommand varchar(MAX)
--===========================================================================================================
PRINT '------------------------------------- INICIANDO --------------------------------------------------------';
-- Table [ApiScopes]
IF EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiScopes_ApiResources_ApiResourceId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiScopes')
)
BEGIN
  ALTER TABLE [ApiScopes]
  DROP CONSTRAINT [FK_ApiScopes_ApiResources_ApiResourceId];

  PRINT 'Se eliminó el constraint por llave foránea [FK_ApiScopes_ApiResources_ApiResourceId], de la tabla [ApiScopes]';
END

IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiScopes_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiScopes]'))
BEGIN
  DROP INDEX [IX_ApiScopes_ApiResourceId]
	ON [DBO].[ApiScopes];

  PRINT 'Se eliminó el índice [IX_ApiScopes_ApiResourceId], de la tabla [ApiScopes]';
END

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopes' 
                   and column_name='ApiResourceId')
BEGIN
  ALTER TABLE [ApiScopes]
	DROP COLUMN [ApiResourceId];

  PRINT 'Se eliminó la columna [ApiResourceId]';
END
ELSE
	PRINT 'La columna [ApiResourceId] ya no existe, en la tabla [ApiScopes]';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopeClaims]
IF EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiScopeClaims_ApiScopes_ApiScopeId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiScopeClaims')
)
BEGIN
  ALTER TABLE [ApiScopeClaims]
  DROP CONSTRAINT [FK_ApiScopeClaims_ApiScopes_ApiScopeId];

  PRINT 'Se eliminó el constraint por llave foránea [FK_ApiScopeClaims_ApiScopes_ApiScopeId], de la tabla [ApiScopeClaims]';
END

IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiScopeClaims_ApiScopeId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiScopeClaims]'))
BEGIN
  DROP INDEX [IX_ApiScopeClaims_ApiScopeId]
	ON [DBO].[ApiScopeClaims];

  PRINT 'Se eliminó el índice [IX_ApiScopeClaims_ApiScopeId], de la tabla [ApiScopeClaims]';
END

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopeClaims' 
                   and column_name='ApiScopeId')
BEGIN
  ALTER TABLE [ApiScopeClaims]
	DROP COLUMN [ApiScopeId];

  PRINT 'Se eliminó la columna [ApiScopeId] de la tabla [ApiScopeClaims]';
END
ELSE
	PRINT 'La columna [ApiScopeId] ya no existe, en la tabla [ApiScopeClaims]';
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiScopeClaims_ApiScopes_ScopeId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiScopeClaims'))
BEGIN
  ALTER TABLE [ApiScopeClaims]
	ADD CONSTRAINT [FK_ApiScopeClaims_ApiScopes_ScopeId] FOREIGN KEY ([ScopeId]) REFERENCES [ApiScopes] ([Id]) ON DELETE CASCADE;

  PRINT 'Se creó el constraint por llave foránea [FK_ApiScopeClaims_ApiScopes_ScopeId], de la tabla [ApiScopeClaims]';
END
PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
--dbo.ApiResourceSecrets
--dbo.ApiSecrets
IF EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiSecrets_ApiResources_ApiResourceId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiSecrets')
)
BEGIN
	ALTER TABLE [ApiSecrets]
		DROP CONSTRAINT [FK_ApiSecrets_ApiResources_ApiResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_ApiSecrets_ApiResources_ApiResourceId], de la tabla [ApiSecrets]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiSecrets')
BEGIN
	DROP TABLE dbo.ApiSecrets;

	PRINT 'Se eliminó la tabla [ApiSecrets]';
END
ELSE
	PRINT 'La tabla [ApiSecrets] ya no existe';
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiResourceSecrets_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiResourceSecrets]'))
BEGIN
	CREATE INDEX [IX_ApiResourceSecrets_ApiResourceId] ON [ApiResourceSecrets] ([ApiResourceId]);
	PRINT 'Se creó el índice [IX_ApiResourceSecrets_ApiResourceId], de la tabla [ApiResourceSecrets]';
END
ELSE
	PRINT 'El índice [IX_ApiResourceSecrets_ApiResourceId], ya existe en la tabla [ApiResourceSecrets]';

IF NOT EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiResourceSecrets_ApiResources_ApiResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiResourceSecrets'))
BEGIN
	ALTER TABLE [ApiResourceSecrets]
		ADD CONSTRAINT [FK_ApiResourceSecrets_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES ApiResources ([Id]) ON DELETE CASCADE;

	PRINT 'Se creó el constraint por llave foránea [FK_ApiResourceSecrets_ApiResources_ApiResourceId], de la tabla [ApiResourceSecrets]';
END

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
--dbo.ApiResourceClaims
--dbo.ApiClaims
IF EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiClaims_ApiResources_ApiResourceId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiClaims')
)
BEGIN
	ALTER TABLE [ApiClaims]
		DROP CONSTRAINT [FK_ApiClaims_ApiResources_ApiResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_ApiClaims_ApiResources_ApiResourceId], de la tabla [ApiClaims]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiClaims')
BEGIN
	DROP TABLE dbo.ApiClaims;

	PRINT 'Se eliminó la tabla [ApiClaims]';
END
ELSE
	PRINT 'La tabla [ApiClaims] ya no existe';
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiResourceClaims_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiResourceClaims]'))
BEGIN
	CREATE INDEX [IX_ApiResourceClaims_ApiResourceId] ON [ApiResourceClaims] ([ApiResourceId]);
	PRINT 'Se creó el índice [IX_ApiResourceClaims_ApiResourceId], de la tabla [ApiResourceClaims]';
END
ELSE
	PRINT 'El índice [IX_ApiResourceClaims_ApiResourceId], ya existe en la tabla [ApiResourceClaims]';

IF NOT EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiResourceClaims_ApiResources_ApiResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiResourceClaims'))
BEGIN
	ALTER TABLE [ApiResourceClaims]
		ADD CONSTRAINT [FK_ApiResourceClaims_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES ApiResources ([Id]) ON DELETE CASCADE;

	PRINT 'Se creó el constraint por llave foránea [FK_ApiResourceClaims_ApiResources_ApiResourceId], de la tabla [ApiResourceClaims]';
END

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
--dbo.ApiResourceProperties
--dbo.ApiProperties
IF EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiProperties_ApiResources_ApiResourceId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiProperties')
)
BEGIN
	ALTER TABLE [ApiProperties]
		DROP CONSTRAINT [FK_ApiProperties_ApiResources_ApiResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_ApiProperties_ApiResources_ApiResourceId], de la tabla [ApiProperties]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiProperties')
BEGIN
	DROP TABLE dbo.ApiProperties;

	PRINT 'Se eliminó la tabla [ApiProperties]';
END
ELSE
	PRINT 'La tabla [ApiProperties] ya no existe';
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiResourceProperties_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiResourceProperties]'))
BEGIN
	CREATE INDEX [IX_ApiResourceProperties_ApiResourceId] ON [ApiResourceProperties] ([ApiResourceId]);
	PRINT 'Se creó el índice [IX_ApiResourceProperties_ApiResourceId], de la tabla [ApiResourceProperties]';
END
ELSE
	PRINT 'El índice [IX_ApiResourceProperties_ApiResourceId], ya existe en la tabla [ApiResourceProperties]';

IF NOT EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiResourceProperties_ApiResources_ApiResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiResourceProperties'))
BEGIN
	ALTER TABLE [ApiResourceProperties]
		ADD CONSTRAINT [FK_ApiResourceProperties_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES ApiResources ([Id]) ON DELETE CASCADE;

	PRINT 'Se creó el constraint por llave foránea [FK_ApiResourceProperties_ApiResources_ApiResourceId], de la tabla [ApiResourceProperties]';
END

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
--dbo.IdentityResourceProperties
--dbo.IdentityProperties
IF EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_IdentityProperties_IdentityResources_IdentityResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.IdentityProperties'))
BEGIN
	ALTER TABLE [IdentityProperties]
		DROP CONSTRAINT [FK_IdentityProperties_IdentityResources_IdentityResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_IdentityProperties_IdentityResources_IdentityResourceId], de la tabla [IdentityProperties]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityProperties')
BEGIN
	DROP TABLE dbo.IdentityProperties;

	PRINT 'Se eliminó la tabla [IdentityProperties]';
END
ELSE
	PRINT 'La tabla [IdentityProperties] ya no existe';
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_IdentityResourceProperties_IdentityResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[IdentityResourceProperties]'))
BEGIN
	CREATE INDEX [IX_IdentityResourceProperties_IdentityResourceId] ON [IdentityResourceProperties] ([IdentityResourceId]);
	PRINT 'Se creó el índice [IX_IdentityResourceProperties_IdentityResourceId], de la tabla [IdentityResourceProperties]';
END
ELSE
	PRINT 'El índice [IX_IdentityResourceProperties_IdentityResourceId], ya existe en la tabla [IdentityResourceProperties]';

IF NOT EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_IdentityResourceProperties_IdentityResources_IdentityResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.IdentityResourceProperties'))
BEGIN
	ALTER TABLE [IdentityResourceProperties]
		ADD CONSTRAINT [FK_IdentityResourceProperties_IdentityResources_IdentityResourceId] FOREIGN KEY ([IdentityResourceId]) REFERENCES IdentityResources ([Id]) ON DELETE CASCADE;

	PRINT 'Se creó el constraint por llave foránea [FK_IdentityResourceProperties_IdentityResources_IdentityResourceId], de la tabla [IdentityResourceProperties]';
END

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
--dbo.IdentityResourceClaims
--dbo.IdentityClaims
IF EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_IdentityClaims_IdentityResources_IdentityResourceId')
   AND parent_object_id = OBJECT_ID(N'dbo.IdentityClaims'))
BEGIN
	ALTER TABLE [IdentityClaims]
		DROP CONSTRAINT [FK_IdentityClaims_IdentityResources_IdentityResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_IdentityClaims_IdentityResources_IdentityResourceId], de la tabla [IdentityClaims]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityClaims')
BEGIN
	DROP TABLE dbo.IdentityClaims;

	PRINT 'Se eliminó la tabla [IdentityClaims]';
END
ELSE
	PRINT 'La tabla [IdentityClaims] ya no existe';
---------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_IdentityResourceClaims_IdentityResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[IdentityResourceClaims]'))
BEGIN
	CREATE INDEX [IX_IdentityResourceClaims_IdentityResourceId] ON [IdentityResourceClaims] ([IdentityResourceId]);
	PRINT 'Se creó el índice [IX_IdentityResourceClaims_IdentityResourceId], de la tabla [IdentityResourceClaims]';
END
ELSE
	PRINT 'El índice [IX_IdentityResourceClaims_IdentityResourceId], ya existe en la tabla [IdentityResourceClaims]';

IF NOT EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_IdentityResourceClaims_IdentityResources_IdentityResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.IdentityResourceClaims'))
BEGIN
	ALTER TABLE [IdentityResourceClaims]
		ADD CONSTRAINT [FK_IdentityResourceClaims_IdentityResources_IdentityResourceId] FOREIGN KEY ([IdentityResourceId]) REFERENCES IdentityResources ([Id]) ON DELETE CASCADE;

	PRINT 'Se creó el constraint por llave foránea [FK_IdentityResourceClaims_IdentityResources_IdentityResourceId], de la tabla [IdentityResourceClaims]';
END

--===========================================================================================================
PRINT '-------------------------------------------- FIN -------------------------------------------------------';
COMMIT TRAN