using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZRiverSys.Model
{
    public class FileManageModel
    {
        public string ID { get; set; }

        public string FileName { get; set; }

        public string FileDescribe { get; set; }

        public string FileType { get; set; }

        public string FilePath { get; set; }

        public string Creater { get; set; }

        public string CreateTime { get; set; }

        public string RoleID { get; set; }

        public File2005HTML Operate { get; set; }
    }
}