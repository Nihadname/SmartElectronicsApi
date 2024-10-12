using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class Contact:BaseEntity
    {
        public string Name { get; set; }
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
