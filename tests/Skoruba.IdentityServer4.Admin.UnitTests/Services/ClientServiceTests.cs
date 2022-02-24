﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
    public class ClientServiceTests
    {
        public ClientServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<IdentityServerConfigurationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        private readonly DbContextOptions<IdentityServerConfigurationDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        private IClientRepository GetClientRepository(IdentityServerConfigurationDbContext context)
        {
            IClientRepository clientRepository = new ClientRepository<IdentityServerConfigurationDbContext>(context);

            return clientRepository;
        }

        private IClientService GetClientService(IClientRepository repository, IClientServiceResources resources, IAuditEventLogger auditEventLogger)
        {
            IClientService clientService = new ClientService(repository, resources, auditEventLogger);

            return clientService;
        }

        private IClientService GetClientService(IdentityServerConfigurationDbContext context)
        {
            var clientRepository = GetClientRepository(context);

            var localizerMock = new Mock<IClientServiceResources>();
            var localizer = localizerMock.Object;

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            var auditLogger = auditLoggerMock.Object;

            var clientService = GetClientService(clientRepository, localizer, auditLogger);

            return clientService;
        }

        [Fact]
        public async Task AddClientAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task CloneClientAsync()
        {
            int clonedClientId;

            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                //Generate random new client
                var clientDto = ClientDtoMock.GenerateRandomClient(0);

                var clientService = GetClientService(context);

                //Add new client
                await clientService.AddClientAsync(clientDto);

                var clientId = await context.Clients.Where(x => x.ClientId == clientDto.ClientId).Select(x => x.Id)
                    .SingleOrDefaultAsync();

                var clientDtoToClone = await clientService.GetClientAsync(clientId);

                var clientCloneDto = ClientDtoMock.GenerateClientCloneDto(clientDtoToClone.Id);

                //Try clone it
                clonedClientId = await clientService.CloneClientAsync(clientCloneDto);

                var cloneClientEntity = await context.Clients
                    .Include(x => x.AllowedGrantTypes)
                    .Include(x => x.RedirectUris)
                    .Include(x => x.PostLogoutRedirectUris)
                    .Include(x => x.AllowedScopes)
                    .Include(x => x.ClientSecrets)
                    .Include(x => x.Claims)
                    .Include(x => x.IdentityProviderRestrictions)
                    .Include(x => x.AllowedCorsOrigins)
                    .Include(x => x.Properties)
                    .Where(x => x.Id == clonedClientId).SingleOrDefaultAsync();

                var clientToCompare = await context.Clients
                    .Include(x => x.AllowedGrantTypes)
                    .Include(x => x.RedirectUris)
                    .Include(x => x.PostLogoutRedirectUris)
                    .Include(x => x.AllowedScopes)
                    .Include(x => x.ClientSecrets)
                    .Include(x => x.Claims)
                    .Include(x => x.IdentityProviderRestrictions)
                    .Include(x => x.AllowedCorsOrigins)
                    .Include(x => x.Properties)
                    .Where(x => x.Id == clientDtoToClone.Id).SingleOrDefaultAsync();

                //Assert cloned client
                cloneClientEntity.Should().BeEquivalentTo(clientToCompare,
                    options => options.Excluding(o => o.Id)
                        .Excluding(o => o.ClientSecrets)
                        .Excluding(o => o.ClientId)
                        .Excluding(o => o.ClientName)

                        //Skip the collections because is not possible ignore property in list :-(
                        //Note: I've found the solution above - try ignore property of the list using SelectedMemberPath                        
                        .Excluding(o => o.AllowedGrantTypes)
                        .Excluding(o => o.RedirectUris)
                        .Excluding(o => o.PostLogoutRedirectUris)
                        .Excluding(o => o.AllowedScopes)
                        .Excluding(o => o.ClientSecrets)
                        .Excluding(o => o.Claims)
                        .Excluding(o => o.IdentityProviderRestrictions)
                        .Excluding(o => o.AllowedCorsOrigins)
                        .Excluding(o => o.Properties)
                );


                //New client relations have new id's and client relations therefore is required ignore them
                cloneClientEntity.AllowedGrantTypes.Should().BeEquivalentTo(clientToCompare.AllowedGrantTypes,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.AllowedCorsOrigins.Should().BeEquivalentTo(clientToCompare.AllowedCorsOrigins,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.RedirectUris.Should().BeEquivalentTo(clientToCompare.RedirectUris,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.PostLogoutRedirectUris.Should().BeEquivalentTo(clientToCompare.PostLogoutRedirectUris,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.AllowedScopes.Should().BeEquivalentTo(clientToCompare.AllowedScopes,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.ClientSecrets.Should().BeEquivalentTo(clientToCompare.ClientSecrets,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.Claims.Should().BeEquivalentTo(clientToCompare.Claims,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.IdentityProviderRestrictions.Should().BeEquivalentTo(
                    clientToCompare.IdentityProviderRestrictions,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));

                cloneClientEntity.Properties.Should().BeEquivalentTo(clientToCompare.Properties,
                    option => option.Excluding(x => x.Path.EndsWith("Id"))
                        .Excluding(x => x.Path.EndsWith("Client")));
            }
        }

        [Fact]
        public async Task UpdateClientAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client without id
                var client = ClientDtoMock.GenerateRandomClient(0);

                //Add new client
                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(clientEntity).State = EntityState.Detached;

                //Generete new client with added item id
                var updatedClient = ClientDtoMock.GenerateRandomClient(clientDto.Id);

                //Update client
                await clientService.UpdateClientAsync(updatedClient);

                //Get updated client
                var updatedClientEntity = await context.Clients.Where(x => x.Id == updatedClient.Id).SingleAsync();

                var updatedClientDto = await clientService.GetClientAsync(updatedClientEntity.Id);

                //Assert updated client
                updatedClient.Should().BeEquivalentTo(updatedClientDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task RemoveClientAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client without id
                var client = ClientDtoMock.GenerateRandomClient(0);

                //Add new client
                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(clientEntity).State = EntityState.Detached;

                //Remove client
                await clientService.RemoveClientAsync(clientDto);

                //Try Get Removed client
                var removeClientEntity = await context.Clients.Where(x => x.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Assert removed client - it might be null
                removeClientEntity.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetClientAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task AddClientClaimAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client Claim
                var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientEntity.Id);

                //Add new client claim
                await clientService.AddClientClaimAsync(clientClaim);

                //Get inserted client claims
                var claim = await context.ClientClaims.Where(x => x.Value == clientClaim.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var claimsDto = claim.ToModel();

                //Get new client claim    
                var clientClaimsDto = await clientService.GetClientClaimAsync(claim.Id);

                //Assert
                clientClaimsDto.Should().BeEquivalentTo(claimsDto, options =>
                    options.Excluding(o => o.ClientClaimId)
                           .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task DeleteClientClaimAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client Claim
                var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientEntity.Id);

                //Add new client claim
                await clientService.AddClientClaimAsync(clientClaim);

                //Get inserted client claims
                var claim = await context.ClientClaims.Where(x => x.Value == clientClaim.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var claimsDto = claim.ToModel();

                //Get new client claim    
                var clientClaimsDto = await clientService.GetClientClaimAsync(claim.Id);

                //Assert
                clientClaimsDto.Should().BeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId)
                                .Excluding(o => o.ClientName));

                //Delete client claim
                await clientService.DeleteClientClaimAsync(clientClaimsDto);

                //Get removed client claim
                var deletedClientClaim = await context.ClientClaims.Where(x => x.Id == claim.Id).SingleOrDefaultAsync();

                //Assert after delete it
                deletedClientClaim.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetClientClaimAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client Claim
                var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientEntity.Id);

                //Add new client claim
                await clientService.AddClientClaimAsync(clientClaim);

                //Get inserted client claims
                var claim = await context.ClientClaims.Where(x => x.Value == clientClaim.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var claimsDto = claim.ToModel();

                //Get new client claim    
                var clientClaimsDto = await clientService.GetClientClaimAsync(claim.Id);

                //Assert
                clientClaimsDto.Should().BeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId)
                    .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task AddClientPropertyAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client property
                var clicentProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientEntity.Id);

                //Add new client property
                await clientService.AddClientPropertyAsync(clicentProperty);

                //Get inserted client property
                var property = await context.ClientProperties.Where(x => x.Value == clicentProperty.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var propertyDto = property.ToModel();

                //Get new client property    
                var clientPropertiesDto = await clientService.GetClientPropertyAsync(property.Id);

                //Assert
                clientPropertiesDto.Should().BeEquivalentTo(propertyDto, options => 
                    options.Excluding(o => o.ClientPropertyId)
                           .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task GetClientPropertyAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client property
                var clicentProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientEntity.Id);

                //Add new client property
                await clientService.AddClientPropertyAsync(clicentProperty);

                //Get inserted client property
                var property = await context.ClientProperties.Where(x => x.Value == clicentProperty.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var propertyDto = property.ToModel();

                //Get new client property    
                var clientPropertiesDto = await clientService.GetClientPropertyAsync(property.Id);

                //Assert
                clientPropertiesDto.Should().BeEquivalentTo(propertyDto, options => options.Excluding(o => o.ClientPropertyId)
                    .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task DeleteClientPropertyAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client Property
                var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientEntity.Id);

                //Add new client Property
                await clientService.AddClientPropertyAsync(clientProperty);

                //Get inserted client Property
                var property = await context.ClientProperties.Where(x => x.Value == clientProperty.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var propertiesDto = property.ToModel();

                //Get new client Property    
                var clientPropertiesDto = await clientService.GetClientPropertyAsync(property.Id);

                //Assert
                clientPropertiesDto.Should().BeEquivalentTo(propertiesDto, options => options.Excluding(o => o.ClientPropertyId)
                    .Excluding(o => o.ClientName));

                //Delete client Property
                await clientService.DeleteClientPropertyAsync(clientPropertiesDto);

                //Get removed client Property
                var deletedClientProperty = await context.ClientProperties.Where(x => x.Id == property.Id).SingleOrDefaultAsync();

                //Assert after delete it
                deletedClientProperty.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddClientSecretAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client secret
                var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientEntity.Id);

                //Add new client secret
                await clientService.AddClientSecretAsync(clientSecret);

                //Get inserted client secret
                var secret = await context.ClientSecrets.Where(x => x.Value == clientSecret.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var clientSecretsDto = secret.ToModel();

                //Get new client secret    
                var secretsDto = await clientService.GetClientSecretAsync(secret.Id);

                clientSecretsDto.Value.Should().Be(clientSecret.Value);

                //Assert
                secretsDto.Should().BeEquivalentTo(clientSecretsDto, options =>
                    options.Excluding(o => o.ClientSecretId)
                        .Excluding(o => o.ClientName)
                        .Excluding(o => o.Value));
            }
        }

        [Fact]
        public async Task GetClientSecretAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client secret
                var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientEntity.Id);

                //Add new client secret
                await clientService.AddClientSecretAsync(clientSecret);

                //Get inserted client secret
                var secret = await context.ClientSecrets.Where(x => x.Value == clientSecret.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var clientSecretsDto = secret.ToModel();

                //Get new client secret    
                var secretsDto = await clientService.GetClientSecretAsync(secret.Id);

                clientSecretsDto.Value.Should().Be(clientSecret.Value);

                //Assert
                secretsDto.Should().BeEquivalentTo(clientSecretsDto, options => options.Excluding(o => o.ClientSecretId)
                    .Excluding(o => o.ClientName)
                    .Excluding(o => o.Value));
            }
        }

        [Fact]
        public async Task DeleteClientSecretAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var clientService = GetClientService(context);
                
                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.Should().BeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

                //Generate random new Client secret
                var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientEntity.Id);

                //Add new client secret
                await clientService.AddClientSecretAsync(clientSecret);

                //Get inserted client secret
                var secret = await context.ClientSecrets.Where(x => x.Value == clientSecret.Value && x.Client.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Map entity to model
                var secretsDto = secret.ToModel();

                //Get new client secret    
                var clientSecretsDto = await clientService.GetClientSecretAsync(secret.Id);

                //Assert
                clientSecretsDto.Should().BeEquivalentTo(secretsDto, options => options.Excluding(o => o.ClientSecretId)
                    .Excluding(o => o.ClientName)
                    .Excluding(o => o.Value));

                clientSecret.Value.Should().Be(secret.Value);

                //Delete client secret
                await clientService.DeleteClientSecretAsync(clientSecretsDto);

                //Get removed client secret
                var deleteClientSecret = await context.ClientSecrets.Where(x => x.Id == secret.Id).SingleOrDefaultAsync();

                //Assert after delete it
                deleteClientSecret.Should().BeNull();
            }
        }
    }
}