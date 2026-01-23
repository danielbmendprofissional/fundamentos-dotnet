using DesignPatterns.Singleton.Pain;
using DesignPatterns.Singleton.Solution;

//Pain
// No Processador de Fotos
var service1 = new BadUploadService();
service1.Upload("foto1.jpg"); // Print: Total 1

// No Processador de Vídeos
var service2 = new BadUploadService();
service2.Upload("video1.mp4"); // Print: Total 1 (Should be 2!)


// Solution
var singleton = UploadService.Instance;
var singleton2 = UploadService.Instance;

Console.WriteLine(ReferenceEquals(singleton, singleton2)); // True