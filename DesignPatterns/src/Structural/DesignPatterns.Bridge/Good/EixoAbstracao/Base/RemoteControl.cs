using DesignPatterns.Bridge.Good.EixoImplementacao.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Bridge.Good.EixoAbstracao.Base
{
    //This is the abstract class of the Abstraction side
    //Defines todas as necessidades que um tipo da abstração tem.
    //No exemplo Todo controle remoto tem um botão de ligar, mas
    //não sabe como liga... Isso fica com o outro eixo...
    public abstract class RemoteControl
    {
        protected IDevice _device; // The Bridge between two axies

        protected RemoteControl(IDevice device) 
            => _device = device;

        public void TogglePower() 
            => _device.SetPower(true);
    }
}
