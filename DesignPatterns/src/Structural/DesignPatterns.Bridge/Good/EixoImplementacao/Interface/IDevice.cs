using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Good.EixoImplementacao.Interface
{
    // This is the interface of implementation side. 
    // It defines the basic operations every implementation device must have.
    // I Can create and manage N Implementation without comunicate with the Abstraction side
    public interface IDevice
    {
        void SetPower(bool on);
        void SetVolume(int percent);
        void SetChannel(int channel);
        string GetBrand();
    }
}
