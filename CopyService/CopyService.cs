using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CopyService
{
    public partial class CopyService : ServiceBase
    {
        private string[] args;

        static string startFolder = @"C:/start/";
        static string endFolder = @"C:/end/";

        public CopyService(string[] _args)
        {
            InitializeComponent();
            args = _args;
        }

        protected override void OnStart(string[] args)
        {
            
            if (args.Length > 0) 
            {
                startFolder = args[0];
            }
            if (args.Length > 1) 
            {
                endFolder = args[1];
            }

            if (!Directory.Exists(startFolder)) 
            {
                Directory.CreateDirectory(startFolder);
            }

            if (!Directory.Exists(endFolder))
            {
                Directory.CreateDirectory(endFolder);
            }

            FileSystemWatcher watcher = new FileSystemWatcher(startFolder);

            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter = NotifyFilters.FileName
                                 | NotifyFilters.LastWrite;


            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;

            
            watcher.EnableRaisingEvents = true;

            string[] files = Directory.GetFiles(startFolder);

            foreach (string filePath in files)
            {
                WriteFile(filePath);
            }

        }

        private static void WriteFile(string path) 
        {
            string filename = Path.GetFileName(path);
            string destFile = Path.Combine(endFolder, filename);

            FileStream fileStream = new FileStream(destFile, FileMode.Create);

            fileStream.Write(File.ReadAllBytes(path), 0, File.ReadAllBytes(path).Length);

            fileStream.Flush();

            fileStream.Close();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e) 
        {
            WriteFile(e.FullPath);
        }

        protected override void OnStop()
        {
            Directory.Delete(endFolder, true);
        }
    }
}
