using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class DirectoryBrowseModel
    {
        public MyDirectory ParentDirectory { get; set; }
        public MyDirectory CurrentDirectory { get; set; }
        public List<MyDirectory> Directories { get; set; }
        public List<MyFile> Files { get; set; }
        public List<MyFileSize> AllFilesSizes { get; set; }
    }
}