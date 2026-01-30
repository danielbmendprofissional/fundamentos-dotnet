using DesignPatterns.Bridge.Bad.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Bad
{
    public class LgRemote : RemoteControl
    {
        public override void TogglePower() => Console.WriteLine("LG TV Power Toggled");
    }
}
