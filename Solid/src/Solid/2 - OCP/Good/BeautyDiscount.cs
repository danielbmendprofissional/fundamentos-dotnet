using Solid.OCP.Good.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.OCP.Good
{
    public class BeautyDiscount : Discount
    {
        public override decimal Calculate(decimal valor) => valor * 0.4M;
    }
}
