using AOPDemo.Attributes;

namespace AOPDemo.Repositories
{
    public interface IValuesRepository
    {
        [Cache(cacheKeyPatter:"{Id}-{dummy}", durationMinutes: 1)]
        string Get(GetValueRequest request, int dummy);
    }

    public class GetValueRequest
    {
        public int Id { get; set; }
    }
}