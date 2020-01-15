using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class InnovationGenerator
    {
        private static int innovation = 0;
        public static int GetInnovation()
        {
            innovation++;
            return innovation;
        }
        public static void SetInnovation(int inno)
        {
            if (inno > innovation)
            {
                innovation = inno;
            }
        }
    }
}
