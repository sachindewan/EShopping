using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Entities
{
    public class Address : BaseEntity
    {
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
    }
}
