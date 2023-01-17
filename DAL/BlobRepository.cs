using System.Net;

using Minio;

namespace SafeShare.DAL;

public class BlobRepository : IBlobRepository
{
    private readonly MinioClient _minioClient;

    public BlobRepository(
        IConfiguration configuration)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(configuration["MINIO_ENDPOINT_URL"])
            .WithCredentials(configuration["MINIO_ACCESS_KEY_ID"], configuration["MINIO_SECRET_ACCESS_KEY"])
            // Proxy and SSL setting should be removed for prod environment
            .WithProxy(new WebProxy(configuration["MINIO_CONTAINER_NAME"]!,
                Int32.Parse(configuration["MINIO_API_PORT"]!)))
            .WithSSL(false)
            .Build();
    }

    /// <summary>
    ///     Generate a presigned URL that can be used to access the file named
    ///     in the objectKey parameter for the amount of time specified in the
    ///     duration parameter.
    /// </summary>
    /// <param name="bucketName">
    ///     The name of the S3 bucket containing the
    ///     object for which to create the presigned URL.
    /// </param>
    /// <param name="name">
    ///     Human-readable name of the file.
    /// </param>
    /// <param name="uid">
    ///     The name of the object to access with the
    ///     presigned URL.
    /// </param>
    /// <param name="durationSeconds">
    ///     The length of time for which the presigned
    ///     URL will be valid.
    /// </param>
    /// <returns>A string representing the generated presigned URL.</returns>
    public Task<string> GeneratePresignedPutRequest(string bucketName, string name, string uid, int durationSeconds)
    {
        Dictionary<string, string> presignedUrlHeaders =
            new() { { "Content-Disposition", "attachment; filename=" + name } };
        PresignedPutObjectArgs args = new PresignedPutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(uid)
            .WithExpiry(durationSeconds)
            .WithHeaders(presignedUrlHeaders);

        return _minioClient.PresignedPutObjectAsync(args);
    }

    public Task<string> GeneratePresignedGetRequest(string bucketName, string uid, int durationSeconds)
    {
        PresignedGetObjectArgs args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(uid)
            .WithExpiry(durationSeconds);

        return _minioClient.PresignedGetObjectAsync(args);
    }

    public async Task RemoveObject(string bucketName, string uid)
    {
        RemoveObjectArgs args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(uid);
        await _minioClient.RemoveObjectAsync(args);
    }

    public async Task CreateBucketIfNotExists(string bucketName)
    {
        if (!await _minioClient.BucketExistsAsync(bucketName))
        {
            await _minioClient.MakeBucketAsync(bucketName);
        }
    }
}


