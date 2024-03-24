using Microsoft.Extensions.Diagnostics.Enrichment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshopping.ServiceDefaults
{
    public class CorrelationIdEnricher : ILogEnricher
    {
        public void Enrich(IEnrichmentTagCollector collector)
        {
            collector.Add("CorrelationId", Guid.NewGuid());
        }
    }
}
