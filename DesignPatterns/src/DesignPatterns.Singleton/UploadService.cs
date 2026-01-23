using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Singleton
{
    //Needs to be thread safe
    public class UploadService
    {
        // Private static instance variable to hold the single instance of the class
        // Cannot be accessed from outside the class
        // Using Lazy<T> to ensure thread-safe lazy initialization

        // Lazy replace this:
        /*
         if(_instance == null)
         {
             lock(_lock)
             {
                 if(_instance == null)
                 {
                     _instance = new UploadService();
                 }
             }
         }
        */

        // static because you cannot have instance members in static context
        private static readonly Lazy<UploadService> _instance =
            new Lazy<UploadService>(() => new UploadService());

        // Public static property to provide global access to the instance
        // Can be a method as well, but property is more idiomatic for singleton pattern
        public static UploadService Instance => _instance.Value;

        // Cannot have any parameters and need to be private, because the instance is created internally
        // without any arguments, from outside.
        private UploadService() {}
    }
}
