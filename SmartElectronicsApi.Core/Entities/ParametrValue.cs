using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class ParametrValue:BaseEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int ParametrGroupId  { get; set; }
        public ParametrGroup ParametrGroup { get; set; }
    }
}
