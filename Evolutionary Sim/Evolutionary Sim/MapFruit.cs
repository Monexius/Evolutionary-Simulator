using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class MapFruit
    {
        public static Sprite baseSprite;
        public static Texture2D Texture;
        public static int FrameCount;
        public static Vector2 WorldLocation;
        public static Rectangle InitialFrame;

        public MapFruit(Texture2D texture, Rectangle initialFrame, int frameCount, Vector2 worldLocation)
        {
            int frameWidth = initialFrame.Width;
            int frameHeight = initialFrame.Height;
            Texture = texture;
            FrameCount = frameCount;
            WorldLocation = worldLocation;
            InitialFrame = initialFrame;
            Debug.WriteLine("Passed");
            baseSprite = new Sprite(WorldLocation, Texture, InitialFrame);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            baseSprite.Draw(spriteBatch);
        }
      
    }
}
