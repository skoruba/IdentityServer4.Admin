USE OAuth2
GO
--rollback
-- ==========================================================================================================
-- Author:		Mario Alberto Rojas Zúñiga
-- Create date: 20/06/2020
-- Description:	Se realizan cambios estructurales para compatibilidad con la version 4.0 de IdentityServer4
-- ==========================================================================================================

SET XACT_ABORT ON;
GO

BEGIN TRAN
DECLARE @database varchar(max) = 'OAuth2';
DECLARE @table varchar(MAX)
DECLARE @colName varchar(max)
DECLARE @sqlCommand varchar(MAX)

--===========================================================================================================
-- Table [Clients]
PRINT '------------------------------------- INICIANDO --------------------------------------------------------';
IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='Clients' 
                   and column_name='RequireRequestObject')
BEGIN
	SELECT	@table = 'Clients';
	SELECT  @colName = 'RequireRequestObject';
	select  @sqlCommand = 'ALTER TABLE ' + @table + ' DROP CONSTRAINT [' + (
		select d.name
		from 
			 sys.tables t
			 join sys.default_constraints d on d.parent_object_id = t.object_id
			 join sys.columns c on c.object_id = t.object_id
								   and c.column_id = d.parent_column_id
		where 
			 t.name = @table
			 and c.name = @colName
		) + ']'

    exec    (@SQLCOMMAND)

	ALTER TABLE [Clients]
		DROP COLUMN RequireRequestObject

	PRINT 'Se eliminó la columna [RequireRequestObject], en la tabla [Clients]';
END
ELSE
	PRINT 'La columna [RequireRequestObject] no existe, en la tabla [Clients]';

-- RequireRequestObject

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='Clients' 
                   and column_name='AllowedIdentityTokenSigningAlgorithms')
BEGIN
	ALTER TABLE [Clients] 
		DROP COLUMN AllowedIdentityTokenSigningAlgorithms

	PRINT 'Se eliminó la columna [AllowedIdentityTokenSigningAlgorithms], en la tabla [Clients]';
END
ELSE
	PRINT 'La columna [AllowedIdentityTokenSigningAlgorithms] no existe, en la tabla [Clients]';

-- AllowedIdentityTokenSigningAlgorithms
PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [DeviceCodes]

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='DeviceCodes' 
                   and column_name='SessionId')
BEGIN
	ALTER TABLE [DeviceCodes]
		DROP COLUMN SessionId

	PRINT 'Se eliminó la columna [SessionId], en la tabla [DeviceCodes]';
END
ELSE
	PRINT 'La columna [SessionId] no existe, en la tabla [DeviceCodes]';

-- SessionId
-------------------------------------------------------------------------------------------------------------

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='DeviceCodes' 
                   and column_name='Description')
BEGIN
	ALTER TABLE [DeviceCodes]
		DROP COLUMN [Description] 

	PRINT 'Se eliminó la columna [Description], en la tabla [DeviceCodes]';
END
ELSE
	PRINT 'La columna [Description] no existe, en la tabla [DeviceCodes]';

-- Description
-------------------------------------------------------------------------------------------------------------
PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [PersistedGrants]

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='PersistedGrants' 
                   and column_name='SessionId')
BEGIN
	ALTER TABLE [PersistedGrants]
		DROP COLUMN SessionId

	PRINT 'Se eliminó la columna [SessionId], en la tabla [PersistedGrants]';
END
ELSE
	PRINT 'La columna [SessionId] no existe, en la tabla [PersistedGrants]';

-- SessionId
-------------------------------------------------------------------------------------------------------------

IF  EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='PersistedGrants' 
                   and column_name='Description')
BEGIN
	ALTER TABLE [PersistedGrants]
		DROP COLUMN [Description]

	PRINT 'Se eliminó la columna [Description], en la tabla [PersistedGrants]';
END
ELSE
	PRINT 'La columna [Description] no existe, en la tabla [PersistedGrants]';

-- Description
-------------------------------------------------------------------------------------------------------------

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='PersistedGrants' 
                   and column_name='ConsumedTime')
BEGIN
	ALTER TABLE [PersistedGrants]
		DROP COLUMN ConsumedTime

	PRINT 'Se eliminó la columna [ConsumedTime], en la tabla [PersistedGrants]';
END
ELSE
	PRINT 'La columna [ConsumedTime] no existe, en la tabla [PersistedGrants]';

-- ConsumedTime
PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [IdentityResourceClaims]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityClaims')
BEGIN
	SELECT *
	INTO dbo.IdentityClaims
	FROM dbo.IdentityResourceClaims
	
	PRINT 'Se creó la tabla [IdentityClaims]';
END

-----------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_IdentityResourceClaims_IdentityResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[IdentityResourceClaims]'))
BEGIN
	DROP INDEX [IX_IdentityResourceClaims_IdentityResourceId] ON [IdentityResourceClaims];
	PRINT 'Se eliminó el índice [IX_IdentityResourceClaims_IdentityResourceId], de la tabla [IdentityResourceClaims]';
END
	

IF EXISTS (SELECT Name 
	FROM sys.foreign_keys 
	WHERE object_id = OBJECT_ID(N'dbo.FK_IdentityResourceClaims_IdentityResources_IdentityResourceId')
	AND parent_object_id = OBJECT_ID(N'dbo.IdentityResourceClaims')
)
BEGIN
	ALTER TABLE [IdentityResourceClaims]
	DROP CONSTRAINT [FK_IdentityResourceClaims_IdentityResources_IdentityResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_IdentityResourceClaims_IdentityResources_IdentityResourceId], de la tabla [IdentityResourceClaims]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityResourceClaims')
BEGIN
	DROP TABLE dbo.IdentityResourceClaims;

	PRINT 'Se eliminó la tabla [IdentityResourceClaims]';
END
ELSE
	PRINT 'La tabla [IdentityResourceClaims] ya no existe';

PRINT '--------------------------------------------------------------------------------------------------------';

--===========================================================================================================
-- [Table IdentityResourceProperties]
-------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityProperties')
BEGIN
	SELECT *
	INTO dbo.IdentityProperties
	FROM dbo.IdentityResourceProperties
	
	PRINT 'Se creó la tabla [IdentityProperties]';

END

IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_IdentityResourceProperties_IdentityResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[IdentityResourceProperties]'))
BEGIN
	DROP INDEX [IX_IdentityResourceProperties_IdentityResourceId] ON [IdentityResourceProperties];
	PRINT 'Se eliminó el índice [IX_IdentityResourceProperties_IdentityResourceId], de la tabla [IdentityResourceProperties]';
END

IF EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_IdentityResourceProperties_IdentityResources_IdentityResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.IdentityResourceProperties'))
BEGIN
	ALTER TABLE [IdentityResourceProperties]
		DROP CONSTRAINT [FK_IdentityResourceProperties_IdentityResources_IdentityResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_IdentityResourceProperties_IdentityResources_IdentityResourceId], de la tabla [IdentityResourceProperties]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityResourceProperties')
BEGIN
	DROP TABLE dbo.IdentityResourceProperties;

	PRINT 'Se eliminó la tabla [IdentityResourceProperties]';
END
ELSE
	PRINT 'La tabla [IdentityResourceProperties] ya no existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResources]

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiResources' 
                   and column_name='AllowedAccessTokenSigningAlgorithms')
BEGIN
	ALTER TABLE ApiResources
		DROP COLUMN AllowedAccessTokenSigningAlgorithms

	PRINT 'Se eliminó la columna [AllowedAccessTokenSigningAlgorithms], en la tabla [ApiResources]';
END
ELSE
	PRINT 'La columna [AllowedAccessTokenSigningAlgorithms] ya no existe, en la tabla [ApiResources]';

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiResources' 
                   and column_name='ShowInDiscoveryDocument')
BEGIN
	SELECT	@table = 'ApiResources';
	SELECT @colName = 'ShowInDiscoveryDocument';
	select  @sqlCommand = 'ALTER TABLE ' + @table + ' DROP CONSTRAINT [' + (
		select d.name
		from 
			 sys.tables t
			 join sys.default_constraints d on d.parent_object_id = t.object_id
			 join sys.columns c on c.object_id = t.object_id
								   and c.column_id = d.parent_column_id
		where 
			 t.name = @table
			 and c.name = @colName
		) + ']'

    exec    (@SQLCOMMAND)

	ALTER TABLE [ApiResources]
	DROP COLUMN ShowInDiscoveryDocument

	PRINT 'Se eliminó la columna [ShowInDiscoveryDocument], en la tabla [ApiResources]';
END
ELSE
	PRINT 'La columna [ShowInDiscoveryDocument] ya no existe, en la tabla [ApiResources]';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceSecrets]
IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiSecrets')
BEGIN
	SELECT * 
	INTO dbo.ApiSecrets
	FROM dbo.ApiResourceSecrets
	
	PRINT 'Se creó la tabla [ApiSecrets] a partir de la tabla [ApiResourceSecrets]';	
END

IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiResourceSecrets_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiResourceSecrets]'))
BEGIN
	DROP INDEX [IX_ApiResourceSecrets_ApiResourceId] ON [ApiResourceSecrets];
	PRINT 'Se elimió el índice [IX_ApiResourceSecrets_ApiResourceId], de la tabla [ApiResourceSecrets]';
END

IF EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiResourceSecrets_ApiResources_ApiResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiResourceSecrets'))
BEGIN
	ALTER TABLE [ApiResourceSecrets]
		DROP CONSTRAINT [FK_ApiResourceSecrets_ApiResources_ApiResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_ApiResourceSecrets_ApiResources_ApiResourceId], de la tabla [ApiResourceSecrets]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceSecrets')
BEGIN
	DROP TABLE dbo.ApiResourceSecrets;
	PRINT 'Se eliminó la tabla [ApiResourceSecrets]';
END
ELSE
	PRINT 'La tabla [ApiResourceSecrets] ya no existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceClaims]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiClaims')
BEGIN
	SELECT * 
	INTO dbo.ApiClaims
	FROM dbo.ApiResourceClaims

	PRINT 'Se creó la tabla [ApiClaims] a partir de la tabla [ApiResourceClaims]';
END

IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiResourceClaims_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiResourceClaims]'))
BEGIN
	DROP INDEX [IX_ApiResourceClaims_ApiResourceId] ON [ApiResourceClaims];
	PRINT 'Se eliminó el índice [IX_ApiResourceClaims_ApiResourceId], de la tabla [ApiResourceClaims]';
END

IF EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiResourceClaims_ApiResources_ApiResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiResourceClaims'))
BEGIN
	ALTER TABLE [ApiResourceClaims]
		DROP CONSTRAINT [FK_ApiResourceClaims_ApiResources_ApiResourceId];

	PRINT 'Se eliminó el constraint por llave foránea [FK_ApiResourceClaims_ApiResources_ApiResourceId], de la tabla [ApiResourceClaims]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceClaims')
BEGIN
	DROP TABLE dbo.ApiResourceClaims;

	PRINT 'Se eliminó la tabla [ApiResourceClaims]';
END
ELSE
	PRINT 'La tabla [ApiResourceClaims] ya no existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceProperties]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiProperties')
BEGIN

	SELECT * 
	INTO dbo.ApiProperties
	FROM dbo.ApiResourceProperties

	PRINT 'Se creó la tabla [ApiProperties] a partir de la tabla [ApiResourceProperties]';
END

IF NOT EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiProperties_ApiResources_ApiResourceId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiProperties')
)
BEGIN
	ALTER TABLE [ApiProperties]
		ADD CONSTRAINT [FK_ApiProperties_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [ApiResources] ([Id]) ON DELETE CASCADE;

	PRINT 'Se creó el constraint por llave foránea [FK_ApiProperties_ApiResources_ApiResourceId], de la tabla [ApiProperties]';
END

---------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiResourceProperties_ApiResourceId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiResourceProperties]'))
BEGIN
	DROP INDEX [IX_ApiResourceProperties_ApiResourceId] on [ApiResourceProperties];
	PRINT 'Se eliminó el índice [IX_ApiResourceProperties_ApiResourceId], de la tabla [ApiResourceProperties]';
END

IF EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiResourceProperties_ApiResources_ApiResourceId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiResourceProperties'))
BEGIN
	ALTER TABLE [ApiResourceProperties]
		DROP CONSTRAINT [FK_ApiResourceProperties_ApiResources_ApiResourceId];
	PRINT 'Se eliminó el constraint por llave foránea [FK_ApiResourceProperties_ApiResources_ApiResourceId], de la tabla [ApiResourceProperties]';
END

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceProperties')
BEGIN
	DROP TABLE dbo.ApiResourceProperties;

	PRINT 'Se eliminó la tabla [ApiResourceProperties]';
END
ELSE
	PRINT 'La tabla [ApiResourceProperties] ya no existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopes]
-- Se elimina la columa [Enabled]
-- Se restaura la relación de llave foránea hacia la tabla [ApiResources]
-- Se remueve la llave foránea de la tabla [ApiResourcesScopes]
-- Se elimina la tabla [ApiResourcesScopes]

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopes' 
                   and column_name='Enabled')
BEGIN

	SELECT	@table = 'ApiScopes';
	SELECT @colName = 'Enabled';
	select  @sqlCommand = 'ALTER TABLE ' + @table + ' DROP CONSTRAINT [' + (
		select d.name
		from 
			 sys.tables t
			 join sys.default_constraints d on d.parent_object_id = t.object_id
			 join sys.columns c on c.object_id = t.object_id
								   and c.column_id = d.parent_column_id
		where 
			 t.name = @table
			 and c.name = @colName
		) + ']'

    exec    (@SQLCOMMAND)

	ALTER TABLE ApiScopes
	DROP COLUMN [Enabled];
	
	PRINT 'Se eliminó la columna [Enabled], en la tabla [ApiScopes]';
END
ELSE
	PRINT 'La columna [Enabled] ya no existe, en la tabla [ApiScopes]';
-------------------------------------------------------------------------------
IF NOT EXISTS (SELECT column_name 
            FROM information_schema.columns 
            WHERE table_schema='dbo' 
                and table_name='ApiScopes' 
                and column_name='ApiResourceId')
BEGIN

	ALTER TABLE [ApiScopes]
		ADD [ApiResourceId] INT NULL;

	IF EXISTS (SELECT TABLE_NAME 
				FROM information_schema.TABLES 
				WHERE table_schema='dbo' 
					and table_name='ApiResourceScopes')
	BEGIN
		-- Migrar los ApiScopes a la tabla ApiResourceScopes
		SELECT @sqlCommand = '
		UPDATE Table_A
		SET
			Table_A.ApiResourceId = Table_B.ApiResourceId
		FROM
			[dbo].ApiResourceScopes AS Table_B
			INNER JOIN [dbo].ApiScopes AS Table_A
				ON Table_A.Name = Table_B.Scope
		WHERE Table_A.Name = Table_B.Scope;';

		exec (@sqlCommand);
		PRINT 'Se copiaron: ' +  CAST(@@ROWCOUNT as varchar(3))  + ' registros a la tabla [ApiScopes]';

		ALTER TABLE [ApiScopes]
			ALTER COLUMN ApiResourceId INT NOT NULL;

		ALTER TABLE ApiResourceScopes
			DROP CONSTRAINT FK_ApiResourceScopes_ApiResources_ApiResourceId;

		DROP TABLE ApiResourceScopes;

		PRINT 'Se eliminó la tabla [ApiResourceScopes]';
	END

	IF NOT EXISTS (SELECT NAME
		FROM sys.indexes
		WHERE name='IX_ApiScopes_ApiResourceId' 
				AND object_id = OBJECT_ID('[DBO].[ApiScopes]'))
	BEGIN
  		CREATE INDEX [IX_ApiScopes_ApiResourceId] ON [DBO].[ApiScopes] ([ApiResourceId]);

		PRINT 'Se creó el índice [IX_ApiScopes_ApiResourceId], en la columna [ApiResourceId], de la tabla [ApiScopes]';
	END
	IF NOT EXISTS (SELECT Name 
	  FROM sys.foreign_keys 
	   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiScopes_ApiResources_ApiResourceId')
	   AND parent_object_id = OBJECT_ID(N'dbo.ApiScopes')
	)
	BEGIN
		ALTER TABLE [dbo].[ApiScopes]  WITH CHECK 
		ADD  CONSTRAINT [FK_ApiScopes_ApiResources_ApiResourceId] FOREIGN KEY([ApiResourceId])
		REFERENCES [dbo].[ApiResources] ([Id])
		ON DELETE CASCADE;

		ALTER TABLE [dbo].[ApiScopes] CHECK CONSTRAINT [FK_ApiScopes_ApiResources_ApiResourceId];

		PRINT 'Se creó el foreing key [FK_ApiScopes_ApiResources_ApiResourceId], en la columna [ApiResourceId], de la tabla [ApiScopes]';
	END
END

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopeClaims]

---------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT Name 
		   FROM sys.foreign_keys 
		   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiScopeClaims_ApiScopes_ScopeId')
		   AND parent_object_id = OBJECT_ID(N'dbo.ApiScopeClaims'))
BEGIN
  ALTER TABLE [ApiScopeClaims]
	DROP CONSTRAINT [FK_ApiScopeClaims_ApiScopes_ScopeId];

  PRINT 'Se eliminó el constraint por llave foránea [FK_ApiScopeClaims_ApiScopes_ScopeId], de la tabla [ApiScopeClaims]';
END

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopeClaims' 
                   and column_name='ApiScopeId')
BEGIN
  ALTER TABLE [ApiScopeClaims]
	ADD [ApiScopeId]  INT NULL
	PRINT 'Se agregó la columna [ApiScopeId], en la tabla [ApiScopeClaims]';

		-- Migrar los ApiScopeId a la columna ScopeId
	SELECT @sqlCommand = '
		UPDATE	dbo.ApiScopeClaims
		SET		ApiScopeId = ScopeId
		FROM	dbo.ApiScopeClaims';
	EXEC (@sqlCommand)

	PRINT 'Se copiaron: ' +  CAST(@@ROWCOUNT as varchar(3))  + ' registros a la columna [ScopeId]';

	ALTER TABLE [ApiScopeClaims]
		ALTER COLUMN ApiScopeId INT NOT NULL;
END

IF EXISTS (SELECT NAME
               FROM sys.indexes
               WHERE name='IX_ApiScopeClaims_ScopeId' 
					 AND object_id = OBJECT_ID('[DBO].[ApiScopeClaims]'))
BEGIN
  DROP INDEX [IX_ApiScopeClaims_ScopeId]
	ON [DBO].[ApiScopeClaims];

  PRINT 'Se eliminó el índice [IX_ApiScopeClaims_ScopeId], de la tabla [ApiScopeClaims]';
END

IF EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopeClaims' 
                   and column_name='ScopeId')
BEGIN
	
	ALTER TABLE dbo.[ApiScopeClaims]
	DROP COLUMN ScopeId

	PRINT 'Se eliminó la columna [ScopeId], en la tabla [ApiScopeClaims]';
END
ELSE
	PRINT 'La columna [ScopeId] ya no existe, en la tabla [ApiScopeClaims]';


IF NOT EXISTS (SELECT Name 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_ApiScopeClaims_ApiScopes_ApiScopeId')
   AND parent_object_id = OBJECT_ID(N'dbo.ApiScopeClaims')
)
BEGIN
  ALTER TABLE [ApiScopeClaims]
	ADD CONSTRAINT [FK_ApiScopeClaims_ApiScopes_ApiScopeId] FOREIGN KEY([ApiScopeId])
		REFERENCES [dbo].[ApiScopes] ([Id])
		ON DELETE CASCADE;

  PRINT 'Se creó el constraint por llave foránea [FK_ApiScopeClaims_ApiScopes_ApiScopeId], de la tabla [ApiScopeClaims]';
END

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopeProperties]

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopeProperties')
BEGIN
	-- TODO: Eliminar constrains
	ALTER TABLE [ApiScopeProperties] DROP CONSTRAINT [FK_ApiScopeProperties_ApiScopes_ScopeId]
	DROP INDEX [IX_ApiScopeProperties_ScopeId] ON [ApiScopeProperties] 
	DROP TABLE [ApiScopeProperties] 

	PRINT 'Se eliminó la tabla [ApiScopeProperties]';
END
ELSE
	PRINT 'La tabla [ApiScopeProperties] ya no existe';
--===========================================================================================================
-- Table [AuditLog]

IF EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='AuditLog')
BEGIN

	DROP TABLE [dbo].[AuditLog]

	PRINT 'Se eliminó la tabla [AuditLog]';
END
ELSE
	PRINT 'La tabla [AuditLog] ya no existe';
--===========================================================================================================
PRINT '-------------------------------------------- FIN -------------------------------------------------------';
COMMIT TRAN
GO
