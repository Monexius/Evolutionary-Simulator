using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class ScreenTransition : Camera
    {

        public static List<FadeItem> items = new List<FadeItem>();
        static private Texture2D texture;

        public static void Initialise(Texture2D circleTexture2D)
        {
            texture = circleTexture2D;
            
        }

        public static void RunAnimation()
        {
            for (int xPixel = 32; xPixel - 32 < ViewPortWidth; xPixel += 100)
            {
                for (int yPixel = 32; yPixel - 32 < ViewPortHeight; yPixel += 100)
                {
                    items.Add(new FadeItem()
                    {
                        Xpos = xPixel,
                        Ypos = yPixel,
                        Delay = xPixel + yPixel
                    });
                }
            }
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var item in items)
            {
                item.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            }
        }

        static public void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            foreach (var item in items)
            {
                spritebatch.Draw(texture, new Vector2(item.Xpos, item.Ypos), null, Color.White, 0f, new Vector2(32, 32), item.Scale, SpriteEffects.None, 0);
            }
        }
    }
}
