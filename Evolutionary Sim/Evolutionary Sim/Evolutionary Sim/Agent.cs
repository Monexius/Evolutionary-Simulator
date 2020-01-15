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
    class Agent
    {
        public static Sprite BaseSprite;
        private int xPosition;
        private int yPosition;

        public static void Initialize(Texture2D texture, Rectangle baseInitialFrame, int baseFrameCount, Vector2 worldLoaction)
        {
            int frameWidth = baseInitialFrame.Width;
            int frameHeight = baseInitialFrame.Height;
            BaseSprite = new Sprite(worldLoaction, texture, baseInitialFrame);

            BaseSprite.BoundingXPadding = 4;
            BaseSprite.BoundingYPadding = 4;
            BaseSprite.AnimateWhenStopped = false;
        }
        #region Movement
        public static void moveUp()
        {

        }

        public static void moveDown()
        {

        }

        public static void moveLeft()
        {

        }

        public static void moveRight()
        {

        }
        #endregion
        public static void Update(GameTime gameTime)
        {
            BaseSprite.Update(gameTime);
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            BaseSprite.Draw(spriteBatch);
        }
    }
}
