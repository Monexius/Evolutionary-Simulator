using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        static private int xPosition;
        static private int yPosition;
        static private Random rnd = new Random();
        static private int centerPixel = (16 * 16) / 2;
        static private int currentTile;
        static private SpriteFont basicFont;
        Map map;

        public static void Initialize(Texture2D texture, Rectangle baseInitialFrame, int baseFrameCount)
        {
            int frameWidth = baseInitialFrame.Width;
            int frameHeight = baseInitialFrame.Height;
            xPosition = rnd.Next(0, 99) * 16;
            yPosition = rnd.Next(0, 99) * 16;
            BaseSprite = new Sprite(new Vector2(xPosition, yPosition), texture, baseInitialFrame);
            currentTile = Map.GetTileAtPixel(xPosition, yPosition, Map.mapSquares);
            BaseSprite.BoundingXPadding = 4;
            BaseSprite.BoundingYPadding = 4;
            BaseSprite.AnimateWhenStopped = false;
        }

        public static int getCloseTile(int x, int y)
        {
            int tile;
            tile = Map.GetTileAtPixel(xPosition + (x * 16) , yPosition + (y * 16), Map.mapSquares);
            return tile;
        }

        public static int[,] GetSurroundingTiles()
        {
            int[,] matrix = new int[3,3];
            matrix[0,0] = getCloseTile(-1, -1);
            matrix[0, 1] = getCloseTile(0, -1);
            matrix[0, 2] = getCloseTile(1, -1);

            matrix[1, 0] = getCloseTile(-1, 0);
            matrix[1, 1] = getCloseTile(0, 0);
            matrix[1, 2] = getCloseTile(1, 0);

            matrix[2, 0] = getCloseTile(-1, 1);
            matrix[2, 1] = getCloseTile(0, 1);
            matrix[2, 2] = getCloseTile(1, 1);

            return matrix;
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

        #region Getters and Setters

        public int CurrentTile
        {
            get { return currentTile; }
            set { currentTile = value; }
        }
        #endregion
    }
}
