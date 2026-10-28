using DesignPatterns.Bridge.Good.EixoImplementacao.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Good.EixoImplementacao
{
    //The concret class of the implementation Axis
    public class LgTv : IDevice
    {
        public void SetPower(bool on) => Console.WriteLine("LG: Power " + on);
        public void SetVolume(int percent) => Console.WriteLine($"LG: Volume {percent}");
        public void SetChannel(int channel) => Console.WriteLine($"LG: Channel {channel}");
        public string GetBrand() => "LG WebOS";
    }
}
