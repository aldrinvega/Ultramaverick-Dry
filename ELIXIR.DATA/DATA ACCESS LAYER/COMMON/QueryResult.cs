using System.Collections.Generic;
using System.Security.Principal;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.COMMON
{
    public class QueryResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}