using DesignPatterns.Bridge.Good.EixoAbstracao.Base;
using DesignPatterns.Bridge.Good.EixoImplementacao.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Good.EixoAbstracao
{
    public class SmartRemote : RemoteControl
    {
        public SmartRemote(IDevice device) : base(device) { }

        public void OpenNetflix()
        {
            _device.SetChannel(999);
            Console.WriteLine($"Opening Netflix on {_device.GetBrand()}...");
        }
    }
}
