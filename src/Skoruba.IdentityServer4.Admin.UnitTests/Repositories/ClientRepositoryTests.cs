using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Repositories
{
    public class ClientRepositoryTests
    {
        public ClientRepositoryTests()
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

        [Fact]
        public async Task AddClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await context.Clients.Where(x => x.Id == client.Id).SingleAsync();

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task AddClientClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Claim
                var clientClaim = ClientMock.GenerateRandomClientClaim(0);

                //Add new client claim
                await clientRepository.AddClientClaimAsync(clientEntity.Id, clientClaim);

                //Get new client claim
                var newClientClaim =
                    await context.ClientClaims.Where(x => x.Id == clientClaim.Id).SingleOrDefaultAsync();

                newClientClaim.ShouldBeEquivalentTo(clientClaim,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));
            }
        }

        [Fact]
        public async Task AddClientPropertyAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client property
                var clientProperty = ClientMock.GenerateRandomClientProperty(0);

                //Add new client property
                await clientRepository.AddClientPropertyAsync(clientEntity.Id, clientProperty);

                //Get new client property
                var newClientProperty = await context.ClientProperties.Where(x => x.Id == clientProperty.Id)
                    .SingleOrDefaultAsync();

                newClientProperty.ShouldBeEquivalentTo(clientProperty,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));
            }
        }

        [Fact]
        public async Task AddClientSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Secret
                var clientSecret = ClientMock.GenerateRandomClientSecret(0);

                //Add new client secret
                await clientRepository.AddClientSecretAsync(clientEntity.Id, clientSecret);

                //Get new client secret
                var newSecret = await context.ClientSecrets.Where(x => x.Id == clientSecret.Id).SingleOrDefaultAsync();

                newSecret.ShouldBeEquivalentTo(clientSecret,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));
            }
        }

        [Fact]
        public async Task CloneClientAsync()
        {
            int clonedClientId;

            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                //Generate random new client
                var client = ClientMock.GenerateRandomClient(0);

                IClientRepository clientRepository = new ClientRepository(context);

                //Add new client
                await clientRepository.AddClientAsync(client);

                var clientToClone = await context.Clients.Where(x => x.Id == client.Id).SingleOrDefaultAsync();

                //Try clone it
                clonedClientId = await clientRepository.CloneClientAsync(clientToClone);

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
                    .Where(x => x.Id == clientToClone.Id).SingleOrDefaultAsync();

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

                cloneClientEntity.IdentityProviderRestrictions.ShouldBeEquivalentTo(clientToCompare.IdentityProviderRestrictions,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));

                cloneClientEntity.Properties.ShouldBeEquivalentTo(clientToCompare.Properties,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Client")));
            }
        }

        [Fact]
        public async Task DeleteClientClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Claim
                var clientClaim = ClientMock.GenerateRandomClientClaim(0);

                //Add new client claim
                await clientRepository.AddClientClaimAsync(clientEntity.Id, clientClaim);

                //Get new client claim
                var newClientClaim =
                    await context.ClientClaims.Where(x => x.Id == clientClaim.Id).SingleOrDefaultAsync();

                //Asert
                newClientClaim.ShouldBeEquivalentTo(clientClaim,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));

                //Try delete it
                await clientRepository.DeleteClientClaimAsync(newClientClaim);

                //Get new client claim
                var deletedClientClaim =
                    await context.ClientClaims.Where(x => x.Id == clientClaim.Id).SingleOrDefaultAsync();

                //Assert
                deletedClientClaim.Should().BeNull();
            }
        }

        [Fact]
        public async Task DeleteClientPropertyAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Property
                var clientProperty = ClientMock.GenerateRandomClientProperty(0);

                //Add new client property
                await clientRepository.AddClientPropertyAsync(clientEntity.Id, clientProperty);

                //Get new client property
                var newClientProperties = await context.ClientProperties.Where(x => x.Id == clientProperty.Id)
                    .SingleOrDefaultAsync();

                //Asert
                newClientProperties.ShouldBeEquivalentTo(clientProperty,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));

                //Try delete it
                await clientRepository.DeleteClientPropertyAsync(newClientProperties);

                //Get new client property
                var deletedClientProperty = await context.ClientProperties.Where(x => x.Id == clientProperty.Id)
                    .SingleOrDefaultAsync();

                //Assert
                deletedClientProperty.Should().BeNull();
            }
        }

        [Fact]
        public async Task DeleteClientSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Secret
                var clientSecret = ClientMock.GenerateRandomClientSecret(0);

                //Add new client secret
                await clientRepository.AddClientSecretAsync(clientEntity.Id, clientSecret);

                //Get new client secret
                var newSecret = await context.ClientSecrets.Where(x => x.Id == clientSecret.Id).SingleOrDefaultAsync();

                //Asert
                newSecret.ShouldBeEquivalentTo(clientSecret,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));

                //Try delete it
                await clientRepository.DeleteClientSecretAsync(newSecret);

                //Get new client secret
                var deletedSecret =
                    await context.ClientSecrets.Where(x => x.Id == clientSecret.Id).SingleOrDefaultAsync();

                //Assert
                deletedSecret.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task GetClientClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client claim
                var clientClaim = ClientMock.GenerateRandomClientClaim(0);

                //Add new client claim
                await clientRepository.AddClientClaimAsync(clientEntity.Id, clientClaim);

                //Get new client claim
                var newClientClaim = await clientRepository.GetClientClaimAsync(clientClaim.Id);

                newClientClaim.ShouldBeEquivalentTo(clientClaim,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));
            }
        }

        [Fact]
        public async Task GetClientPropertyAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Property
                var clientProperty = ClientMock.GenerateRandomClientProperty(0);

                //Add new client Property
                await clientRepository.AddClientPropertyAsync(clientEntity.Id, clientProperty);

                //Get new client Property
                var newClientProperty = await clientRepository.GetClientPropertyAsync(clientProperty.Id);

                newClientProperty.ShouldBeEquivalentTo(clientProperty,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));
            }
        }

        [Fact]
        public async Task GetClientsAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                var rnd = new Random();
                var generateRows = rnd.Next(1, 10);

                //Generate random new clients
                var randomClients = ClientMock.GenerateRandomClients(0, generateRows);

                foreach (var client in randomClients)
                    //Add new client
                    await clientRepository.AddClientAsync(client);

                var clients = await clientRepository.GetClientsAsync();

                //Assert that it is count correct
                clients.Data.Count.Should().Be(randomClients.Count);

                //Assert that clients are same
                clients.Data.ShouldBeEquivalentTo(randomClients);
            }
        }

        [Fact]
        public async Task GetClientSecretAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await clientRepository.GetClientAsync(client.Id);

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Generate random new Client Secret
                var clientSecret = ClientMock.GenerateRandomClientSecret(0);

                //Add new client secret
                await clientRepository.AddClientSecretAsync(clientEntity.Id, clientSecret);

                //Get new client secret
                var newSecret = await clientRepository.GetClientSecretAsync(clientSecret.Id);

                newSecret.ShouldBeEquivalentTo(clientSecret,
                    options => options.Excluding(o => o.Id).Excluding(x => x.Client));
            }
        }

        [Fact]
        public async Task RemoveClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await context.Clients.Where(x => x.Id == client.Id).SingleAsync();

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(clientEntity).State = EntityState.Detached;

                //Remove client
                await clientRepository.RemoveClientAsync(clientEntity);

                //Try Get Removed client
                var removeClientEntity = await context.Clients.Where(x => x.Id == clientEntity.Id)
                    .SingleOrDefaultAsync();

                //Assert removed client - it might be null
                removeClientEntity.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateClientAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IClientRepository clientRepository = new ClientRepository(context);

                //Generate random new client without id
                var client = ClientMock.GenerateRandomClient(0);

                //Add new client
                await clientRepository.AddClientAsync(client);

                //Get new client
                var clientEntity = await context.Clients.Where(x => x.Id == client.Id).SingleAsync();

                //Assert new client
                clientEntity.ShouldBeEquivalentTo(client, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(clientEntity).State = EntityState.Detached;

                //Generete new client with added item id
                var updatedClient = ClientMock.GenerateRandomClient(clientEntity.Id);

                //Update client
                await clientRepository.UpdateClientAsync(updatedClient);

                //Get updated client
                var updatedClientEntity =
                    await context.Clients.Where(x => x.Id == updatedClient.Id).SingleAsync();

                //Assert updated client
                updatedClientEntity.ShouldBeEquivalentTo(updatedClient);
            }
        }
    }
}