using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerPrivate.Api.Models.File
{
    public class UploadMultipleFileRequest
    {
        public List<IFormFile>? Files { get; set; }

    }
}
