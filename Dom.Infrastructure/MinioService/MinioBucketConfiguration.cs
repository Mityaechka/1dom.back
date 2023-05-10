namespace Taxiverk.Infrastructure.MinioService;

public interface MinioBucketConfiguration
{
    public string Name { get; }
}

public class TestMinioBucket : MinioBucketConfiguration
{
    public string Name => "test-bucket";
}