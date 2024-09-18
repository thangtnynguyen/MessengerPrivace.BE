using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerPrivate.Api.Models.File
{
    public class UploadFileRequest
    {
        public IFormFile? DataFile { get; set; }

        public string? FolderName { get; set; }
    }
}
