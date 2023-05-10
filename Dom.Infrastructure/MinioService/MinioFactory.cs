using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;

namespace Taxiverk.Infrastructure.MinioService;

public class MinioFactory
{
    private readonly MinioObjectFileStrategy _minioObjectFileStrategy;
    private readonly MinioClient _minioClient;
    private string _bucket;

    public MinioClient Client => _minioClient;

    public class Options
    {
        public const string Path = "MinIO";
        public string Url { get; set; }
        public string AccessKey { get; set; }//eJALv9LGCW8J0suu
        public string SecretKey { get; set; }//TTTMs3VVc6Tj5AIIN1PVXYnHr3lm5AR5
    }

    public MinioFactory(IOptions<Options> options, MinioObjectFileStrategy minioObjectFileStrategy)
    {
        _minioObjectFileStrategy = minioObjectFileStrategy;
        _minioClient = new MinioClient()
            .WithEndpoint(options.Value.Url)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
            .WithSSL(false)
            .Build();
    }

    public async Task Test(IFormFile formFile)
    {
        _bucket = "test2";
        var beArgs = new BucketExistsArgs().WithBucket(_bucket);
        
        bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(_bucket);
            await _minioClient.MakeBucketAsync(mbArgs);
        }

        await using var stream = formFile.OpenReadStream();
        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() +
                        Path.GetExtension(formFile.FileName))
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(formFile.ContentType);
        await _minioClient.PutObjectAsync(putObjectArgs);
        var getListBucketsTask = await _minioClient.ListBucketsAsync();
        
        var listArgs = new ListObjectsArgs()
            .WithBucket(_bucket)
            .WithRecursive(false);
        var observable = _minioClient.ListObjectsAsync(listArgs);
        var subscription = observable.Subscribe(
            item => Console.WriteLine($"Object: {item.Key}"),
            ex => Console.WriteLine($"OnError: {ex}"),
            () => Console.WriteLine($"Listed all objects in bucket {_bucket}\n"));
    }
}