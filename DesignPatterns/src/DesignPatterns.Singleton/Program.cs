using DesignPatterns.Singleton;

var singleton = UploadService.Instance;
var singleton2 = UploadService.Instance;

Console.WriteLine(ReferenceEquals(singleton, singleton2)); // True