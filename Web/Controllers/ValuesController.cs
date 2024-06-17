using Microsoft.AspNetCore.Mvc;

namespace TestDuende.Web.Controllers
{
    public record KeyVal
    {
        public string? Key { get; init; }
        public string? Value { get; init; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public List<KeyVal> List()
        {
            var values = new List<KeyVal>
            {
                new KeyVal { Key = "1", Value = "value"}
            };

            return values;
        }
    }
}
