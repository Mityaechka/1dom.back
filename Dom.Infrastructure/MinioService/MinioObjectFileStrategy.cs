namespace Taxiverk.Infrastructure.MinioService;

public delegate string MinioObjectFileStrategy(string fullFileName);

public static class MinioObjectFileStrategies
{
    public static MinioObjectFileStrategy TimestampStrategy => fullFileName => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + Path.GetExtension(fullFileName);
    public static MinioObjectFileStrategy GuidStrategy => fullFileName => Guid.NewGuid() + Path.GetExtension(fullFileName);
}