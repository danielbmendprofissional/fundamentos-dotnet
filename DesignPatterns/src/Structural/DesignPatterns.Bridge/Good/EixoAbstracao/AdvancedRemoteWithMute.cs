using DesignPatterns.Bridge.Good.EixoAbstracao.Base;
using DesignPatterns.Bridge.Good.EixoImplementacao.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Good.EixoAbstracao
{
    public class AdvancedRemote : RemoteControl
    {
        public AdvancedRemote(IDevice device) : base(device) { }

        public void Mute() 
            => _device.SetVolume(0);
    }
}
