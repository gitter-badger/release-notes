using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseNotes.Models.Api
{
    public class RepositoryModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
