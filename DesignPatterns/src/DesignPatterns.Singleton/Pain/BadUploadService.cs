using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Singleton.Pain
{
    // Here, when someone calls, the ideal was to every upload the count will get bigger,
    // but if we create multiple instances of this class, each instance will have its own count,
    // leading to inconsistent behavior.
    internal class BadUploadService
    {
        private int _uploadCount = 0;

        public void Upload(string fileName)
        {
            _uploadCount++;
            Console.WriteLine($"Subindo {fileName}. Total de uploads neste serviço: {_uploadCount}");
        }
    }
}
