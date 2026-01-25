using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Adapter._1.Interface
{
    //Just an example interface representing an external API that provides data in JSON format
    public interface ILumiaJSONAdaptee
    {
        string GetLumiaMobilesJSONSpecifications();
    }
}
