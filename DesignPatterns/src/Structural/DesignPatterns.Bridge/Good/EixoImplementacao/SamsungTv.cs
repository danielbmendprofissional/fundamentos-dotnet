using DesignPatterns.Bridge.Good.EixoImplementacao.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Good.EixoImplementacao
{
    //The concret class of the implementation Axis

    public class SamsungTv : IDevice
    {
        public void SetPower(bool on) => Console.WriteLine("Samsung: Power " + on);
        public void SetVolume(int percent) => Console.WriteLine($"Samsung: Volume {percent}");
        public void SetChannel(int channel) => Console.WriteLine($"Samsung: Channel {channel}");
        public string GetBrand() => "Samsung Tizen";
    }
}
