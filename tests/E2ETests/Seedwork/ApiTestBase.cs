namespace E2ETests.Seedwork;

[Collection(nameof(CollectionServerFixture))]
public class ApiTestBase(AppHostFixture given) : IAsyncLifetime
{
    protected AppHostFixture Given { get; set; } = given;

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}