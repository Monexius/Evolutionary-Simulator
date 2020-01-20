using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static public float currentTime;
        static public int rounded;
        public static Texture2D graphics;
        public static int i = 0;
        public static float TIMER = 0.5f;
        public static int time = 0;
        Map map;
        Sprite sprite;

        public static void Initialize(Texture2D texture, Rectangle baseInitialFrame, int baseFrameCount)
        {
            int frameWidth = baseInitialFrame.Width;
            int frameHeight = baseInitialFrame.Height;
            xPosition = rnd.Next(0, 99) * 16;
            yPosition = rnd.Next(0, 99) * 16;
            graphics = texture;
            BaseSprite = new Sprite(new Vector2(xPosition, yPosition), texture, baseInitialFrame);
            BaseSprite.BoundingXPadding = 4;
            BaseSprite.BoundingYPadding = 4;
            BaseSprite.AnimateWhenStopped = false;
            getcurrentTile();
        }

        public static int getcurrentTile()
        {
            return currentTile = Map.GetTileAtPixel(xPosition, yPosition, Map.mapSquares);
        }

        public static int getCloseTile(int x, int y, int layer)
        {
            int tile = 0;

            if (layer == 1)
            {
                tile = Map.GetTileAtPixel(xPosition + (x * 16), yPosition + (y * 16), Map.mapSquares);
            }
            else if(layer == 2)
            {
                tile = Map.GetTileAtPixel(xPosition + (x * 16), yPosition + (y * 16), Map.layer2);
            }
            else if(layer == 3)
            {
                tile = Map.GetTileAtPixel(xPosition + (x * 16), yPosition + (y * 16), Map.layer3);
            }

            return tile;
        }

        public static int[,] GetSurroundingTiles()
        {
            int[,] matrix = new int[5,5];
            matrix[0,0] = getCloseTile(-2, -2, 1);
            matrix[0, 1] = getCloseTile(-1, -2, 1);
            matrix[0, 2] = getCloseTile(0, -2, 1);
            matrix[0, 3] = getCloseTile(1, -2, 1);
            matrix[0, 4] = getCloseTile(2, -2, 1);

            matrix[1, 0] = getCloseTile(-2, -1, 1);
            matrix[1, 1] = getCloseTile(-1, -1, 1);
            matrix[1, 2] = getCloseTile(0, -1, 1);
            matrix[1, 3] = getCloseTile(1, -1, 1);
            matrix[1, 4] = getCloseTile(2, -1, 1);

            matrix[2, 0] = getCloseTile(-2, 0, 1);
            matrix[2, 1] = getCloseTile(-1, 0, 1);
            matrix[2, 2] = 100;
            matrix[2, 3] = getCloseTile(1, 0, 1);
            matrix[2, 4] = getCloseTile(2, 0, 1);

            matrix[3, 0] = getCloseTile(-2, 1, 1);
            matrix[3, 1] = getCloseTile(-1, 1, 1);
            matrix[3, 2] = getCloseTile(0, 1, 1);
            matrix[3, 3] = getCloseTile(1, 1, 1);
            matrix[3, 4] = getCloseTile(2, 1, 1);

            matrix[4, 0] = getCloseTile(-2, 2, 1);
            matrix[4, 1] = getCloseTile(-1, 2, 1);
            matrix[4, 2] = getCloseTile(0, 2, 1);
            matrix[4, 3] = getCloseTile(1, 2, 1);
            matrix[4, 4] = getCloseTile(2, 2, 1);

            return matrix;
        }

        public static int[,] GetSurroundingObjects()
        {
            int[,] matrix = new int[5, 5];
            matrix[0, 0] = getCloseTile(-2, -2, 2);
            matrix[0, 1] = getCloseTile(-1, -2, 2);
            matrix[0, 2] = getCloseTile(0, -2, 2);
            matrix[0, 3] = getCloseTile(1, -2, 2);
            matrix[0, 4] = getCloseTile(2, -2, 2);

            matrix[1, 0] = getCloseTile(-2, -1, 2);
            matrix[1, 1] = getCloseTile(-1, -1, 2);
            matrix[1, 2] = getCloseTile(0, -1, 2);
            matrix[1, 3] = getCloseTile(1, -1, 2);
            matrix[1, 4] = getCloseTile(2, -1, 2);

            matrix[2, 0] = getCloseTile(-2, 0, 2);
            matrix[2, 1] = getCloseTile(-1, 0, 2);
            matrix[2, 2] = getCloseTile(-1, 0, 2);
            matrix[2, 3] = getCloseTile(1, 0, 2);
            matrix[2, 4] = getCloseTile(2, 0, 2);

            matrix[3, 0] = getCloseTile(-2, 1, 2);
            matrix[3, 1] = getCloseTile(-1, 1, 2);
            matrix[3, 2] = getCloseTile(0, 1, 2);
            matrix[3, 3] = getCloseTile(1, 1, 2);
            matrix[3, 4] = getCloseTile(2, 1, 2);

            matrix[4, 0] = getCloseTile(-2, 2, 2);
            matrix[4, 1] = getCloseTile(-1, 2, 2);
            matrix[4, 2] = getCloseTile(0, 2, 2);
            matrix[4, 3] = getCloseTile(1, 2, 2);
            matrix[4, 4] = getCloseTile(2, 2, 2);

            return matrix;
        }

        public static int[,] GetSurroundingItems()
        {
            int[,] matrix = new int[5, 5];
            matrix[0, 0] = getCloseTile(-2, -2, 3);
            matrix[0, 1] = getCloseTile(-1, -2, 3);
            matrix[0, 2] = getCloseTile(0, -2, 3);
            matrix[0, 3] = getCloseTile(1, -2, 3);
            matrix[0, 4] = getCloseTile(2, -2, 3);

            matrix[1, 0] = getCloseTile(-2, -1, 3);
            matrix[1, 1] = getCloseTile(-1, -1, 3);
            matrix[1, 2] = getCloseTile(0, -1, 3);
            matrix[1, 3] = getCloseTile(1, -1, 3);
            matrix[1, 4] = getCloseTile(2, -1, 3);

            matrix[2, 0] = getCloseTile(-2, 0, 3);
            matrix[2, 1] = getCloseTile(-1, 0, 3);
            matrix[2, 2] = getCloseTile(-1, 0, 3);
            matrix[2, 3] = getCloseTile(1, 0, 3);
            matrix[2, 4] = getCloseTile(2, 0, 3);

            matrix[3, 0] = getCloseTile(-2, 1, 3);
            matrix[3, 1] = getCloseTile(-1, 1, 3);
            matrix[3, 2] = getCloseTile(0, 1, 3);
            matrix[3, 3] = getCloseTile(1, 1, 3);
            matrix[3, 4] = getCloseTile(2, 1, 3);

            matrix[4, 0] = getCloseTile(-2, 2, 3);
            matrix[4, 1] = getCloseTile(-1, 2, 3);
            matrix[4, 2] = getCloseTile(0, 2, 3);
            matrix[4, 3] = getCloseTile(1, 2, 3);
            matrix[4, 4] = getCloseTile(2, 2, 3);

            return matrix;
        }

        #region Movement
        public void moveUp()
        {
            yPosition = yPosition - 16;
            BaseSprite.WorldLocation = new Vector2(xPosition, yPosition);
        }

        public void moveDown()
        {
            yPosition = yPosition + 16;
            BaseSprite.WorldLocation = new Vector2(xPosition, yPosition);
        }

        public void moveLeft()
        {
            xPosition = xPosition - 16;
            BaseSprite.WorldLocation = new Vector2(xPosition, yPosition);
        }

        public void moveRight()
        {
            xPosition = xPosition + 16;
            BaseSprite.WorldLocation = new Vector2(xPosition, yPosition);
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            rounded = (int)Math.Round(currentTime, 0);
            if (currentTime >= TIMER)
            {
                Debug.WriteLine(i++);

                switch (rnd.Next(4))
                {
                    case 0:
                        moveUp();
                        break;

                    case 1:
                        moveDown();
                        break;

                    case 2:
                        moveLeft();
                        break;
                    case 3:
                        moveRight();
                        break;
                }
                currentTime = 0;
            }
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
