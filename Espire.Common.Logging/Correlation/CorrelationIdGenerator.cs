using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Espire.Common.Logging.Correlation
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        private string _correlationId = Guid.NewGuid().ToString("D");
        public string Get() => _correlationId;

        public void Set(string correlationId)=> _correlationId = correlationId;
    }
}
