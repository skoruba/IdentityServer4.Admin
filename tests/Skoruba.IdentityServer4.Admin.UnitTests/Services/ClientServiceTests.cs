using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
    public class ClientServiceTests
    {
        public ClientServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        private readonly DbContextOptions<AdminDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        private IClientRepository<AdminDbContext> GetClientRepository(AdminDbContext context)
        {
            IClientRepository<AdminDbContext> clientRepository = new ClientRepository<AdminDbContext>(context);

            return clientRepository;
        }

        private IClientService<AdminDbContext> GetClientService(IClientRepository<AdminDbContext> repository, IClientServiceResources resources)
        {
            IClientService<AdminDbContext> clientService = new ClientService<AdminDbContext>(repository, resources);

            return clientService;
        }

        [Fact]
        public async Task AddClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task CloneClientAsync()
        {
            int clonedClientId;

            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                //Generate random new client
                var clientDto = ClientDtoMock.GenerateRandomClient(0);

                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

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
                cloneClientEntity.ShouldBeEquivalentTo(clientToCompare,
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
                cloneClientEntity.AllowedGrantTypes.ShouldBeEquivalentTo(clientToCompare.AllowedGrantTypes,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.AllowedCorsOrigins.ShouldBeEquivalentTo(clientToCompare.AllowedCorsOrigins,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.RedirectUris.ShouldBeEquivalentTo(clientToCompare.RedirectUris,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.PostLogoutRedirectUris.ShouldBeEquivalentTo(clientToCompare.PostLogoutRedirectUris,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.AllowedScopes.ShouldBeEquivalentTo(clientToCompare.AllowedScopes,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.ClientSecrets.ShouldBeEquivalentTo(clientToCompare.ClientSecrets,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.Claims.ShouldBeEquivalentTo(clientToCompare.Claims,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.IdentityProviderRestrictions.ShouldBeEquivalentTo(
                    clientToCompare.IdentityProviderRestrictions,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.Properties.ShouldBeEquivalentTo(clientToCompare.Properties,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
        }

        [Fact]
        public async Task UpdateClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client without id
                var client = ClientDtoMock.GenerateRandomClient(0);

                //Add new client
                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                updatedClient.ShouldBeEquivalentTo(updatedClientDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task RemoveClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client without id
                var client = ClientDtoMock.GenerateRandomClient(0);

                //Add new client
                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task AddClientClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientClaimsDto.ShouldBeEquivalentTo(claimsDto, options =>
                    options.Excluding(o => o.ClientClaimId)
                           .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task DeleteClientClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientClaimsDto.ShouldBeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId)
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
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientClaimsDto.ShouldBeEquivalentTo(claimsDto, options => options.Excluding(o => o.ClientClaimId)
                    .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task AddClientPropertyAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientPropertiesDto.ShouldBeEquivalentTo(propertyDto, options => 
                    options.Excluding(o => o.ClientPropertyId)
                           .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task GetClientPropertyAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientPropertiesDto.ShouldBeEquivalentTo(propertyDto, options => options.Excluding(o => o.ClientPropertyId)
                    .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task DeleteClientPropertyAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientPropertiesDto.ShouldBeEquivalentTo(propertiesDto, options => options.Excluding(o => o.ClientPropertyId)
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
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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

                //Assert
                secretsDto.ShouldBeEquivalentTo(clientSecretsDto, options => 
                    options.Excluding(o => o.ClientSecretId)
                           .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task GetClientSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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

                //Assert
                secretsDto.ShouldBeEquivalentTo(clientSecretsDto, options => options.Excluding(o => o.ClientSecretId)
                    .Excluding(o => o.ClientName));
            }
        }

        [Fact]
        public async Task DeleteClientSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var clientRepository = GetClientRepository(context);

                var localizerMock = new Mock<IClientServiceResources>();
                var localizer = localizerMock.Object;

                var clientService = GetClientService(clientRepository, localizer);

                //Generate random new client
                var client = ClientDtoMock.GenerateRandomClient(0);

                await clientService.AddClientAsync(client);

                //Get new client
                var clientEntity =
                    await context.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();

                var clientDto = await clientService.GetClientAsync(clientEntity.Id);

                //Assert new client
                client.ShouldBeEquivalentTo(clientDto, options => options.Excluding(o => o.Id));

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
                clientSecretsDto.ShouldBeEquivalentTo(secretsDto, options => options.Excluding(o => o.ClientSecretId)
                    .Excluding(o => o.ClientName));

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