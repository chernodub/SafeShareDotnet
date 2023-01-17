namespace SafeShare.DAL;

public interface IBlobRepository
{
    public Task<string> GeneratePresignedPutRequest(string bucketName, string name, string uid, int durationSeconds);
    public Task<string> GeneratePresignedGetRequest(string bucketName, string uid, int durationSeconds);
    public Task CreateBucketIfNotExists(string bucketName);
}