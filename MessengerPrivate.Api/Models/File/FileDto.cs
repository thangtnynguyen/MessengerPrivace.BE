using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerPrivate.Api.Models.File
{
    public class FileDto
    {

        public IFormFile? DataFile { get; set; }

        public string? FolderName { get; set; }

        public string? AbsoluteFilePath { get; set; }

        public string? RelativeFilePath { get; set; }
    }
}
