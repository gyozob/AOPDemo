using AOPDemo.Attributes;

namespace AOPDemo.Repositories
{
    public interface IValuesRepository
    {
        [Cache("{Id}-{good}")]
        string Get(GetValueRequest request, int good);
    }

    public class GetValueRequest
    {
        public int Id { get; set; }
    }
}