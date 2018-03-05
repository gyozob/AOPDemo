using AOPDemo.Models;

namespace AOPDemo.Repositories
{
    public class ValuesRepository : IValuesRepository
    {
        public string Get(GetValueRequest request, int dummy)
        {
            return $"value - {request.Id}";
        }
    }
}