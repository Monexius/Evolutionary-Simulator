using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class ScreenTransition
    {
       

        static private Texture2D texture;
        public static void Initialise(Texture2D circleTexture2D)
        {
            texture = circleTexture2D;
        }
        static public void Draw(SpriteBatch spritebatch, GameTime gameTime, float x, float y, float scale)
        {
            spritebatch.Draw(texture, new Vector2(x, y), null, Color.White, 0f, new Vector2(32, 32), scale, SpriteEffects.None, 0);
        }
    }
}
