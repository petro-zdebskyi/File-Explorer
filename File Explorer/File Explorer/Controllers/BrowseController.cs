using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WebApi.Models;

namespace File_Explorer.Controllers
{
    public class BrowseController : ApiController
    {
        // GET: api/Browse
        // GET request must be in such format: ~/api/browse or ~/api/browse?path=C:\\, or ~/api/browse?path=D:\\Programs, etc.
        public DirectoryBrowseModel Get()
        {
            DirectoryBrowseModel model = new DirectoryBrowseModel();
            DirectoryInfo currentDirectory = null;
            string absoluteCurrentPath = "C:\\";

            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            Uri myUri = new Uri(url);

            if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("path"))
            {
                absoluteCurrentPath = HttpUtility.ParseQueryString(myUri.Query).Get("path");
            }
            else
            {
                absoluteCurrentPath = HostingEnvironment.MapPath("~/");
            }

            // Set current directory (model.CurrentDirectory) - Name, Path

            currentDirectory = new DirectoryInfo(absoluteCurrentPath);
            model.CurrentDirectory = new MyDirectory(currentDirectory.Name, currentDirectory.FullName);

            // Set parent directory (model.ParentDirectory) - Name, Path
            if (currentDirectory.Parent != null)
            {
                DirectoryInfo parentDirectory = currentDirectory.Parent;
                model.ParentDirectory = new MyDirectory(parentDirectory.Name, parentDirectory.FullName);
            }

            // Set all directories (model.Directories) - List of Name, Path
            model.Directories = new List<MyDirectory>();
            DirectoryInfo[] directories = currentDirectory.GetDirectories();
            foreach (var d in directories)
            {
                model.Directories.Add(new MyDirectory(d.Name, d.FullName));
            }

            // Set all files (model.Files) - List of Name, Path
            model.Files = new List<MyFile>();
            FileInfo[] files = currentDirectory.GetFiles();
            foreach (var f in files)
            {
                model.Files.Add(new MyFile(f.Name, f.FullName));
            }

            // Set all files sizes(model.AllFilesSizes) in current directory and all subdirectories - List of Size
            //FileInfo[] allFiles = currentDirectory.GetFiles("*", SearchOption.AllDirectories);
            model.AllFilesSizes = new List<MyFileSize>();
            foreach (var f in GetFiles(currentDirectory, "*"))
            {
                model.AllFilesSizes.Add(new MyFileSize((f.Length / 1024f) / 1024f));
            }

            return model;
        }

        public static IEnumerable<FileInfo> GetFiles(DirectoryInfo dirInfo, string searchPattern)
        {
            Stack<DirectoryInfo> pending = new Stack<DirectoryInfo>();
            pending.Push(dirInfo);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                FileInfo[] next = null;
                DirectoryInfo[] nextDir = null;

                try
                {
                    DirectoryInfo di = new DirectoryInfo(path.FullName);
                    next = di.GetFiles();
                }
                catch
                { }

                if (next != null && next.Length != 0)
                {
                    foreach (var file in next)
                    {
                        yield return file;
                    }
                }

                try
                {
                    DirectoryInfo di = new DirectoryInfo(path.FullName);
                    nextDir = di.GetDirectories();
                    foreach (var subdir in nextDir)
                    {
                        pending.Push(subdir);
                    }
                }
                catch
                { }
            }
        }
    }
}
