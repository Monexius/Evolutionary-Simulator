using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class FadeItem
    {
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Delay { get; set; }
        public float Radians { get; set; }

        public float Scale
        {
            get
            {
                if (Delay > 0)
                    return 2.0f;
                else if (Radians > MathHelper.Pi)
                    return 0f;

                return (float)Math.Cos(Radians) + 1;
            }
        }

        public void Update(float deltaTimeInMilliseconds)
        {
            Delay -= deltaTimeInMilliseconds;
            if (Delay < 0)
            {
                Radians += deltaTimeInMilliseconds / 100.0f;
            }
        }
    }
}
