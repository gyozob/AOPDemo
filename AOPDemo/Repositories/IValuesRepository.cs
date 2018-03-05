using AOPDemo.Attributes;
using AOPDemo.Models;

namespace AOPDemo.Repositories
{
    public interface IValuesRepository
    {
        [Cache(cacheKeyPattern:"{Id}-{dummy}", durationMinutes: 1)]
        string Get(GetValueRequest request, int dummy);
    }
}