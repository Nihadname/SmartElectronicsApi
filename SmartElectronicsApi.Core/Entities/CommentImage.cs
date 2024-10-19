using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class CommentImage:BaseEntity
    {
        public string Name { get; set; }
        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }
    }
}
