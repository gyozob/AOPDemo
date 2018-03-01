namespace AOPDemo.Repositories
{
    public class ValuesRepository : IValuesRepository
    {
        public string Get(GetValueRequest request, int good)
        {
            return $"value - {request.Id}";
        }
    }
}