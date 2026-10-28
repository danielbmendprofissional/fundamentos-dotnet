using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Bad
{
    public class LgRemoteWithMute : LgRemote
    {
        public void Mute() => Console.WriteLine("LG Muted");
    }
}
