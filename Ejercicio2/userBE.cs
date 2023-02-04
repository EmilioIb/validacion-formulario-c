using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio2
{
    class userBE
    {
        public userBE()
        {

        }
        public string user { get; set; }
        public byte[] pass { get; set; }
        public byte[] salt { get; set; }
        
    }
}
