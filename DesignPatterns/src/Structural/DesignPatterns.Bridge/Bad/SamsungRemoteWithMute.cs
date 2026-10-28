using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Bad
{
    public class SamsungRemoteWithMute : SamsungRemote
    {
        public void Mute() => Console.WriteLine("Samsung Muted");
    }
}
