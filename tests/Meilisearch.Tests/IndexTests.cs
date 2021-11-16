namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HttpClientFactoryLite;
    using Xunit;

    [Collection("Sequential")]
    public class IndexTests : IAsyncLifetime
    {
        private readonly MeilisearchClient defaultClient;
        private readonly string defaultPrimaryKey;
        private readonly IndexFixture fixture;

        public IndexTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.defaultClient = fixture.DefaultClient;
            this.defaultPrimaryKey = "movieId";
        }

        public async Task InitializeAsync() => await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicIndexCreation()
        {
            var indexUid = "BasicIndexCreationTest";
            var index = await this.defaultClient.CreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task IndexCreationWithPrimaryKey()
        {
            var indexUid = "IndexCreationWithPrimaryKeyTest";
            var index = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task BasicUsageOfIndexMethod()
        {
            var indexUid = "BasicUsageOfIndexMethodTest";
            var index = this.defaultClient.Index(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndex(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task IndexMethodUsageOnExistingIndex()
        {
            Meilisearch.Index index;
            var indexUid = "IndexMethodUsageOnExistingIndexTest";
            index = await this.defaultClient.CreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            index = this.defaultClient.Index(indexUid);
            index.Uid.Should().Be(indexUid);

            var document = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });

            document.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task IndexFetchExistingIndexPrimaryKey()
        {
            var indexUid = "IndexFetchExistingIndexPrimaryKeyTest";
            var createIndex = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var indexObject = this.defaultClient.Index(indexUid);
            Assert.Equal(createIndex.Uid, indexObject.Uid);
            createIndex.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            indexObject.PrimaryKey.Should().BeNull();
            var primaryKey = await indexObject.FetchPrimaryKey();
            Assert.Equal(createIndex.PrimaryKey, primaryKey);
        }

        [Fact]
        public async Task IndexAlreadyExistsError()
        {
            var indexUid = "IndexAlreadyExistsErrorTest";
            var index = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey));
            Assert.Equal("index_already_exists", ex.Code);
        }

        [Fact]
        public async Task UpdateIndex()
        {
            var updatedPrimaryKey = "UpdateIndexTest";
            await this.defaultClient.GetOrCreateIndex(updatedPrimaryKey);
            var primarykey = "MovieId" + new Random().Next();
            var modifiedIndex = await this.defaultClient.UpdateIndex(updatedPrimaryKey, primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
        }

        [Fact]
        public async Task IndexNameWrongFormattedError()
        {
            var indexUid = "Wrong UID";
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.CreateIndex(indexUid));
            Assert.Equal("invalid_index_uid", ex.Code);
        }

        [Fact]
        public async Task GetAllRawIndexes()
        {
            var indexUid = "GetAllRawIndexesTest";
            await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var indexes = await this.defaultClient.GetAllRawIndexes();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
            var index = indexes.First();
            Assert.Equal(index.GetProperty("uid").GetString(), indexUid);
            Assert.Equal(index.GetProperty("name").GetString(), indexUid);
            Assert.Equal(index.GetProperty("primaryKey").GetString(), this.defaultPrimaryKey);
        }

        [Fact]
        public async Task GetAllExistingIndexes()
        {
            var indexUid = "GetAllExistingIndexesTest";
            await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var indexes = await this.defaultClient.GetAllIndexes();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneExistingIndex()
        {
            var indexUid = "GetOneExistingIndexTest";
            await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var index = await this.defaultClient.GetIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetAnNonExistingIndex()
        {
            var indexUid = "GetAnNonExistingIndexTest";
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndex(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task GetOrCreateIndexIfIndexDoesNotExist()
        {
            var indexUid = "GetOrCreateIndexIfIndexDoesNotExistTest";
            var index = await this.defaultClient.GetOrCreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task GetOrCreateIndexIfIndexAlreadyExists()
        {
            var indexUid = "GetOrCreateIndexIfIndexAlreadyExistsTest";
            await this.defaultClient.GetOrCreateIndex(indexUid);
            var index = await this.defaultClient.GetOrCreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task GetOrCreateIndexWithPrimaryKey()
        {
            var indexUid = "GetOrCreateIndexWithPrimaryKeyTest";
            await this.defaultClient.GetOrCreateIndex(indexUid, this.defaultPrimaryKey);
            var index = await this.defaultClient.GetOrCreateIndex(indexUid, this.defaultPrimaryKey);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task FetchPrimaryKey()
        {
            var indexUid = "FetchPrimaryKeyTest";
            var index = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            await index.FetchPrimaryKey();
            Assert.Equal(this.defaultPrimaryKey, index.PrimaryKey);
        }

        [Fact]
        public async Task UpdatePrimaryKey()
        {
            var index = await this.defaultClient.GetOrCreateIndex("UpdatePrimaryKeyTest");
            var primarykey = "MovieId" + new Random().Next();
            var modifiedIndex = await index.Update(primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
            modifiedIndex.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            modifiedIndex.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetStats()
        {
            var index = await this.defaultClient.GetOrCreateIndex("GetStatsTests");
            var stats = await index.GetStats();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task WhenIndexExists_DeleteIfExists_ShouldReturnTrue()
        {
            var indexUid = "DeleteIndexTestUid";
            var index = await this.defaultClient.GetOrCreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            var deleted = await index.DeleteIfExists();
            deleted.Should().BeTrue();
        }

        [Fact]
        public async Task WhenIndexNoLongerExists_DeleteIfExists_ShouldReturnFalse()
        {
            var indexUid = "DeleteIndexTestUid";
            var index = await this.defaultClient.GetOrCreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            var deleted = await index.DeleteIfExists();
            deleted.Should().BeTrue();
            var deletedAgain = await index.DeleteIfExists();
            deletedAgain.Should().BeFalse();
        }

        [Fact]
        public async Task GetRawIndex()
        {
            await this.fixture.SetUpBasicIndex("BasicIndex");
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);

            var rawIndex = await ms.GetRawIndex("BasicIndex");

            rawIndex.GetProperty("uid").GetString().Should().Be("BasicIndex");
        }
    }
}
