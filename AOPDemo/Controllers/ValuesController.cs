using AOPDemo.Models;
using AOPDemo.Repositories;
using System.Web.Http;

namespace AOPDemo.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IValuesRepository _repository;
        public ValuesController(IValuesRepository repository)
        {
            _repository = repository;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return _repository.Get(new GetValueRequest() { Id = id }, id * 3);
        }
    }
}
