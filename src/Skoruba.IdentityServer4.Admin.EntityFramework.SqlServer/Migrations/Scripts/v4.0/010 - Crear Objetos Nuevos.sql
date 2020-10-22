USE OAuth2
GO

-- ==========================================================================================================
-- Author:		Mario Alberto Rojas Zúñiga
-- Create date: 06/06/2020
-- Description:	Se realizan cambios estructurales para compatibilidad con la version 4.0 de IdentityServer4
-- ==========================================================================================================

SET XACT_ABORT ON;
GO

BEGIN TRAN

DECLARE @sqlCommand varchar(MAX)
--===========================================================================================================
-- Table [Clients]
PRINT '------------------------------------- INICIANDO --------------------------------------------------------';
IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='Clients' 
                   and column_name='RequireRequestObject')
BEGIN
	ALTER TABLE [Clients]
		ADD RequireRequestObject Bit NOT NULL
		DEFAULT (0)
		WITH VALUES;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Specifies whether the client must use a request object on authorize requests (defaults to false.)',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'Clients',
    @level2type = N'Column',   @level2name = 'RequireRequestObject';

	PRINT 'Se creó la columna [RequireRequestObject], en la tabla [Clients]';
END
ELSE
	PRINT 'La columna [RequireRequestObject] ya existe, en la tabla [Clients]';

-- RequireRequestObject

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='Clients' 
                   and column_name='AllowedIdentityTokenSigningAlgorithms')
BEGIN
	ALTER TABLE [Clients]
		ADD AllowedIdentityTokenSigningAlgorithms nvarchar(100) NULL;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Signing algorithm for identity token. If empty, will use the server default signing algorithm.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'Clients',
    @level2type = N'Column',   @level2name = 'AllowedIdentityTokenSigningAlgorithms';

	PRINT 'Se creó la columna [AllowedIdentityTokenSigningAlgorithms], en la tabla [Clients]';
END
ELSE
	PRINT 'La columna [AllowedIdentityTokenSigningAlgorithms] ya existe, en la tabla [Clients]';

-- AllowedIdentityTokenSigningAlgorithms
PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [DeviceCodes]

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='DeviceCodes' 
                   and column_name='SessionId')
BEGIN
	ALTER TABLE [DeviceCodes]
		ADD SessionId nvarchar(100) NULL;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'The session identifier.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'DeviceCodes',
    @level2type = N'Column',   @level2name = 'SessionId';

	PRINT 'Se creó la columna [SessionId], en la tabla [DeviceCodes]';
END
ELSE
	PRINT 'La columna [SessionId] ya existe, en la tabla [DeviceCodes]';

-- SessionId
-------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='DeviceCodes' 
                   and column_name='Description')
BEGIN
	ALTER TABLE [DeviceCodes]
		ADD [Description] nvarchar(200) NULL;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'The description the user assigned to the device being authorized.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'DeviceCodes',
    @level2type = N'Column',   @level2name = 'Description';

	PRINT 'Se creó la columna [Description], en la tabla [DeviceCodes]';
END
ELSE
	PRINT 'La columna [Description] ya existe, en la tabla [DeviceCodes]';

-- Description
-------------------------------------------------------------------------------------------------------------
PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [PersistedGrants]

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='PersistedGrants' 
                   and column_name='SessionId')
BEGIN
	ALTER TABLE [PersistedGrants]
		ADD SessionId nvarchar(100) NULL;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'The session identifier.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'PersistedGrants',
    @level2type = N'Column',   @level2name = 'SessionId';

	PRINT 'Se creó la columna [SessionId], en la tabla [PersistedGrants]';
END
ELSE
	PRINT 'La columna [SessionId] ya existe, en la tabla [PersistedGrants]';

-- SessionId
-------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='PersistedGrants' 
                   and column_name='Description')
BEGIN
	ALTER TABLE [PersistedGrants]
		ADD [Description] nvarchar(200) NULL;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Descripción del token.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'PersistedGrants',
    @level2type = N'Column',   @level2name = 'Description';

	PRINT 'Se creó la columna [Description], en la tabla [PersistedGrants]';
END
ELSE
	PRINT 'La columna [Description] ya existe, en la tabla [PersistedGrants]';

-- Description
-------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='PersistedGrants' 
                   and column_name='ConsumedTime')
BEGIN
	ALTER TABLE [PersistedGrants]
		ADD ConsumedTime DateTime NULL

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'The consumed time.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'PersistedGrants',
    @level2type = N'Column',   @level2name = 'ConsumedTime';

	PRINT 'Se creó la columna [ConsumedTime], en la tabla [PersistedGrants]';
END
ELSE
	PRINT 'La columna [ConsumedTime] ya existe, en la tabla [PersistedGrants]';

-- ConsumedTime
PRINT '--------------------------------------------------------------------------------------------------------';


--===========================================================================================================
-- [Table IdentityResourceProperties]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityResourceProperties')
BEGIN
	SELECT * 
	INTO dbo.IdentityResourceProperties
	FROM dbo.IdentityProperties

	PRINT 'Se creó la tabla [IdentityResourceProperties] a partir de la tabla [IdentityProperties]';

	ALTER TABLE dbo.IdentityResourceProperties
	ADD CONSTRAINT [PK_IdentityResourceProperties] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Identificador de las propiedades por IdentityResource.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceProperties',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave de la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceProperties',
    @level2type = N'Column',   @level2name = 'Key';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Valor de la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceProperties',
    @level2type = N'Column',   @level2name = 'Value';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea que indica a cual IdentityResource pertenece la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceProperties',
    @level2type = N'Column',   @level2name = 'IdentityResourceId';
END
ELSE
	PRINT 'La tabla [IdentityResourceProperties] ya existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [IdentityResourceClaims]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='IdentityResourceClaims')
BEGIN

	SELECT * 
	INTO dbo.IdentityResourceClaims
	FROM dbo.IdentityClaims

	PRINT 'Se creó la tabla [IdentityResourceClaims] a partir de la tabla [IdentityClaims]';

	ALTER TABLE dbo.IdentityResourceClaims
	ADD CONSTRAINT [PK_IdentityResourceClaims] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Identificador de los claims por IdentityResource.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceClaims',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Nombre del Claim.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceClaims',
    @level2type = N'Column',   @level2name = 'Type';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea que indica a cual IdentityResource pertenece el claim.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'IdentityResourceClaims',
    @level2type = N'Column',   @level2name = 'IdentityResourceId';
END
ELSE
	PRINT 'La tabla [IdentityResourceClaims] ya existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResources]

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiResources' 
                   and column_name='AllowedAccessTokenSigningAlgorithms')
BEGIN
	ALTER TABLE ApiResources
	ADD AllowedAccessTokenSigningAlgorithms nvarchar(100) NULL

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Specifies whether the client must use a request object on authorize requests (defaults to false.)',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResources',
    @level2type = N'Column',   @level2name = 'AllowedAccessTokenSigningAlgorithms';

	PRINT 'Se creó la columna [AllowedAccessTokenSigningAlgorithms], en la tabla [ApiResources]';
END
ELSE
	PRINT 'La columna [AllowedAccessTokenSigningAlgorithms] ya existe, en la tabla [ApiResources]';

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiResources' 
                   and column_name='ShowInDiscoveryDocument')
BEGIN
	ALTER TABLE [ApiResources]
		ADD ShowInDiscoveryDocument  Bit NOT NULL
	DEFAULT (1)
	WITH VALUES;

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Specifies whether this scope is shown in the discovery document. Defaults to true.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResources',
    @level2type = N'Column',   @level2name = 'ShowInDiscoveryDocument';

	PRINT 'Se creó la columna [ShowInDiscoveryDocument], en la tabla [ApiResources]';
END
ELSE
	PRINT 'La columna [ShowInDiscoveryDocument] ya existe, en la tabla [ApiResources]';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceSecrets]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceSecrets')
BEGIN

	SELECT * 
	INTO dbo.ApiResourceSecrets
	FROM dbo.ApiSecrets
	
	PRINT 'Se creó la tabla [ApiResourceSecrets] a partir de la tabla [ApiSecrets]';	
	
	ALTER TABLE dbo.ApiResourceSecrets
	ADD CONSTRAINT [PK_ApiResourceSecrets] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Campo llave del Secret.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Descripción del Secret.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'Description';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Valor del Secret.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'Value';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Fecha de expiración del Secret.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'Expiration';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Tipo del Secret (SharedSecret | X509Thumbprint | X509Name | X509CertificateBase64).',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'Type';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Fecha de creación del Secret.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'Created';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea al recurso Api al que pertenece.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceSecrets',
    @level2type = N'Column',   @level2name = 'ApiResourceId';
END
ELSE
	PRINT 'La tabla [ApiResourceSecrets] ya existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceScopes]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceScopes')
BEGIN

	SELECT [Id], [Name] as Scope, ApiResourceId
	INTO dbo.ApiResourceScopes
	FROM ApiScopes;

	PRINT 'Se creó la tabla [ApiResourceScopes]';

	ALTER TABLE dbo.ApiResourceScopes
	ADD CONSTRAINT [PK_ApiResourceScopes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	ALTER TABLE dbo.ApiResourceScopes
	ADD CONSTRAINT [FK_ApiResourceScopes_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [ApiResources] ([Id]) ON DELETE CASCADE

	CREATE INDEX [IX_ApiResourceScopes_ApiResourceId] ON [ApiResourceScopes] ([ApiResourceId]);

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Campo llave del Scope.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceScopes',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Nombre del Scope.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceScopes',
    @level2type = N'Column',   @level2name = 'Scope';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea al recurso Api al que pertenece.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceScopes',
    @level2type = N'Column',   @level2name = 'ApiResourceId';

END
ELSE
	PRINT 'La tabla [ApiResourceScopes] ya existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceClaims]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceClaims')
BEGIN

	SELECT * 
	INTO dbo.ApiResourceClaims
	FROM dbo.ApiClaims

	PRINT 'Se creó la tabla [ApiResourceClaims] a partir de la tabla [ApiClaims]';

	ALTER TABLE dbo.ApiResourceClaims
	ADD CONSTRAINT [PK_ApiResourceClaims] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Identificador de los claims por ApiResource.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceClaims',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Nombre del Claim.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceClaims',
    @level2type = N'Column',   @level2name = 'Type';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea que indica a cual ApiResource pertenece el claim.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceClaims',
    @level2type = N'Column',   @level2name = 'ApiResourceId';
END
ELSE
	PRINT 'La tabla [ApiResourceClaims] ya existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiResourceProperties]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiResourceProperties')
BEGIN

	SELECT * 
	INTO dbo.ApiResourceProperties
	FROM dbo.ApiProperties

	PRINT 'Se creó la tabla [ApiResourceProperties] a partir de la tabla [ApiProperties]';

	ALTER TABLE dbo.ApiResourceProperties
	ADD CONSTRAINT [PK_ApiResourceProperties] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Identificador de las propiedades por ApiResource.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceProperties',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave de la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceProperties',
    @level2type = N'Column',   @level2name = 'Key';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Valor de la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceProperties',
    @level2type = N'Column',   @level2name = 'Value';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea que indica a cual ApiResource pertenece la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiResourceProperties',
    @level2type = N'Column',   @level2name = 'ApiResourceId';
END
ELSE
	PRINT 'La tabla [ApiResourceProperties] ya existe';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopes]

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopes' 
                   and column_name='Enabled')
BEGIN
	ALTER TABLE dbo.ApiScopes
		ADD [Enabled] Bit NOT NULL
	DEFAULT (1)
	WITH VALUES;

	EXEC sp_addextendedproperty 
	@name = N'MS_Description', @value = 'Indicates if this resource is enabled. Defaults to true.',
	@level0type = N'Schema',   @level0name = 'dbo',
	@level1type = N'Table',    @level1name = 'ApiScopes',
	@level2type = N'Column',   @level2name = 'Enabled';
	
	PRINT 'Se creó la columna [Enabled], en la tabla [ApiScopes]';
END
ELSE
	PRINT 'La columna [Enabled] ya existe, en la tabla [ApiScopes]';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopeClaims]

IF NOT EXISTS (SELECT column_name 
               FROM information_schema.columns 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopeClaims' 
                   and column_name='ScopeId')
BEGIN

	ALTER TABLE dbo.ApiScopeClaims
		ADD ScopeId INT NULL
	PRINT 'Se agregó la columna [ScopeId], en la tabla [ApiScopeClaims]';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea que indica a cual Scope pertenece el claim.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiScopeClaims',
    @level2type = N'Column',   @level2name = 'ScopeId';

	-- Migrar los ApiScopeId a la columna ScopeId
	SELECT @sqlCommand = '
		UPDATE	dbo.ApiScopeClaims
		SET		ScopeId = ApiScopeId
		FROM	dbo.ApiScopeClaims';
	EXEC (@sqlCommand)
	PRINT 'Se copiaron: ' +  CAST(@@ROWCOUNT as varchar(3))  + ' registros a la columna [ScopeId]';
	
	ALTER TABLE dbo.ApiScopeClaims
	ALTER COLUMN ScopeId INT NOT NULL

	CREATE INDEX [IX_ApiScopeClaims_ScopeId] ON [ApiScopeClaims] ([ScopeId]);
	PRINT 'Se creó el índice [IX_ApiScopeClaims_ScopeId], en la tabla [ApiScopeClaims]';

END
ELSE
	PRINT 'La columna [ScopeId] ya existe, en la tabla [ApiScopeClaims]';

PRINT '--------------------------------------------------------------------------------------------------------';
--===========================================================================================================
-- Table [ApiScopeProperties]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='ApiScopeProperties')
BEGIN

	CREATE TABLE [ApiScopeProperties] (
		[Id] int NOT NULL IDENTITY,
		[Key] nvarchar(250) NOT NULL,
		[Value] nvarchar(2000) NOT NULL,
		[ScopeId] int NOT NULL,
		CONSTRAINT [PK_ApiScopeProperties] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_ApiScopeProperties_ApiScopes_ScopeId] FOREIGN KEY ([ScopeId]) REFERENCES [ApiScopes] ([Id]) ON DELETE CASCADE
	);

	PRINT 'Se creó la tabla [ApiScopeProperties]';

	CREATE INDEX [IX_ApiScopeProperties_ScopeId] ON [ApiScopeProperties] ([ScopeId]);

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Identificador de las propiedades por Scope.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiScopeProperties',
    @level2type = N'Column',   @level2name = 'Id';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave de la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiScopeProperties',
    @level2type = N'Column',   @level2name = 'Key';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Valor de la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiScopeProperties',
    @level2type = N'Column',   @level2name = 'Value';

	EXEC sp_addextendedproperty 
    @name = N'MS_Description', @value = 'Llave foránea que indica a cual Scope pertenece la propiedad.',
    @level0type = N'Schema',   @level0name = 'dbo',
    @level1type = N'Table',    @level1name = 'ApiScopeProperties',
    @level2type = N'Column',   @level2name = 'ScopeId';

END
ELSE
	PRINT 'La tabla [ApiScopeProperties] ya existe';
--===========================================================================================================
--===========================================================================================================
-- Table [AuditLog]

IF NOT EXISTS (SELECT TABLE_NAME 
               FROM information_schema.TABLES 
               WHERE table_schema='dbo' 
                   and table_name='AuditLog')
BEGIN

	CREATE TABLE [dbo].[AuditLog](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[Event] [varchar](max) NOT NULL,
		[Source] [varchar](max) NOT NULL,
		[Category] [varchar](max) NULL,
		[SubjectIdentifier] [varchar](max) NULL,
		[SubjectName] [varchar](max) NULL,
		[SubjectType] [varchar](max) NOT NULL,
		[SubjectAdditionalData] [varchar](max) NOT NULL,
		[Action] [varchar](max) NOT NULL,
		[Data] [nvarchar](max) NOT NULL,
		[Created] [datetime] NOT NULL,
	 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([Id] ASC)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])
		ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

	PRINT 'Se creó la tabla [AuditLog]';
END
ELSE
	PRINT 'La tabla [AuditLog] ya existe';
--===========================================================================================================
PRINT '-------------------------------------------- FIN -------------------------------------------------------';
COMMIT TRAN