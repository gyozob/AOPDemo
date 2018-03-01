using AOPDemo.Repositories;
using System.Collections.Generic;
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
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return _repository.Get(new GetValueRequest() { Id = id }, id * 3);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
