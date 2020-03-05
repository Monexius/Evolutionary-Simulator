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

using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace Evolutionary_Sim
{
    class Agent
    {
        private static Sprite baseSprite;
        static private int xPosition;
        static private int yPosition;
        static private Random rnd = new Random();
        static private int centerPixel = (16 * 16) / 2;
        static private int currentTile;
        static public float currentTime;
        static public int rounded;
        public static Texture2D graphics;
        public static int i = 0;
        public static float TIMER = 0.1f;
        public static int time = 0;
        private static IBlackBox brain;
        private static int fruitTotal;
        public static int skipFrameCounter;

        camera camera;
        Map map;
        Sprite sprite;
        HealthBar health;
        Game1 game1;
        Agent agent;

        // NEAT Algorithm
        public static IBlackBox Brain { get; set; }

        public void Initialize(Texture2D texture, Texture2D healthTexture, Rectangle baseInitialFrame, int baseFrameCount)
        {
            int frameWidth = baseInitialFrame.Width;
            int frameHeight = baseInitialFrame.Height;
            spawn();
            graphics = texture;
            BaseSprite = new Sprite(new Vector2(xPosition, yPosition), texture, baseInitialFrame);
            BaseSprite.BoundingXPadding = 4;
            BaseSprite.BoundingYPadding = 4;
            BaseSprite.AnimateWhenStopped = false;
            getcurrentTile();
            health = new HealthBar();
            game1 = new Game1();

        }

        public void Iterate()
        {
            spawn();
        }

        public void InitializeBrain(IBlackBox brain)
        {
            Brain = brain;
        } 

        public void spawn()
        {
            xPosition = rnd.Next(0, Map.MapWidth) * 16;
            yPosition = rnd.Next(0, Map.MapHeight) * 16;

            if (Map.GetTileAtPixel(xPosition, yPosition, Map.mapSquares  ) != 1){
                spawn();
            }
        }

        public int getcurrentTile()
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
            else if (layer == 4)
            {
                tile = Map.GetTileAtPixel(xPosition + (x * 16), yPosition + (y * 16), Map.MergeLayers(Map.mapSquares, Map.layer3));
            }

            return tile;
        }
        
        #region Movement
        public static void CheckDirt()
        {
            if(getCloseTile(0,0, 1) == 22)
            {
                skipFrameCounter++;
            }
        }
        public static void CheckFruit()
        {
            int[,] matrix = GetThreeByThree(3);
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (matrix[y,x] == 40 || matrix[y,x] == 41)
                    {
                        EatFruit(y, x);
                        fruitTotal++;
                    }
                }
            }
        }

        public static void EatFruit(int y, int x)
        {
            int[,] xMatrix = GetXCoordinates();
            int[,] yMatrix = GetYCoordinates();
            Map.SetTileAtSquare(xMatrix[y,x] / 16, yMatrix[y,x] / 16, 0, Map.layer3);
            
            Debug.WriteLine("\n");
        }

         public static Vector2 CheckNextMove(int x, int y, int newDirX, int newDirY, int layer)// 500, 484, 0, -16
        {
            if(getCloseTile(x, y, 1) != 2)  //move if there is no next water tile, if so then stay
            {
                xPosition += newDirX;
                yPosition += newDirY;
            }
            return new Vector2(xPosition, yPosition);
        }
        public void moveUp()
        {
            BaseSprite.WorldLocation = CheckNextMove(0, -1, 0, -16, 1); // 500, 484, 0, -16
        }

        public void moveDown()
        {
            BaseSprite.WorldLocation = CheckNextMove(0, 1, 0, 16, 1);
        }

        public void moveLeft()
        {
            BaseSprite.WorldLocation = CheckNextMove(-1,0,-16,0,1);
        }

        public void moveRight()
        {
            BaseSprite.WorldLocation = CheckNextMove(1,0,16,0,1);
        }
        #endregion

        public void MoveAgent(int move)
        {
            switch (move)
            {
                case 1:
                    moveUp();
                    break;

                case 2:
                    moveDown();
                    break;

                case 3:
                    moveLeft();
                    break;

                case 4:
                    moveRight();
                    break;
            }
        }
        public void Update(GameTime gameTime)
        {
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            rounded = (int)Math.Round(currentTime, 0);

            if (currentTime >= TIMER)
            {
                if (HealthBar.GetHealth() > 0) // remove 5 health every update
                {
                    health.RemoveHealth(5);
                }
                else
                {
                    Map.ClearArray();
                    map = new Map();
                    game1.getMap(graphics); // initialise map
                    health.AddHealth(game1.hp);
                    Iterate();
                    fruitTotal = 0;
                }
               
              

                if (skipFrameCounter > 0)
                {
                    skipFrameCounter--;
                }else
                {
                    MoveAgent(GetMove(GetSurroundingMerge()));
                    CheckDirt();
                    CheckFruit();
                    //camera.Move(new Vector2(xPosition, yPosition));
                }
                currentTime = 0;
            }
            BaseSprite.Update(gameTime);
        }
       
        public int GetTimeSurvived()
        {
            return rounded;
        }

        public int GetFruitTotal()
        {
            return fruitTotal;
        }

        public int GetMove(int[,] board)
        { 
            // Clear the network
            Brain.ResetState();

            // Convert the game board into an input array for the network
            setInputSignalArray(Brain.InputSignalArray, board);

            // Activate the network
            Brain.Activate();

            int move = 0;
            double max = double.MinValue;

            for (int i = 0; i < 5; i++)
            {
                //if the square is water, ignore it #1

                // Set the score for this move
                double score = Brain.OutputSignalArray[i];

                    //If he lives after the first move, call it current best.
                    if (move == 0)
                    {
                        move = rnd.Next(1, 4);
                        max = score;
                    }else if(max < score)
                    {
                       max = score;
                       move = i;
                    }
            }
            return move; 
        }
        
        private void setInputSignalArray(ISignalArray inputArr, int[,] board)
        {
            int counter = 0;
            for(int y = 0; y < 5; y++)
            {
                for(int x = 0; x < 5; x++)
                {
                    inputArr[counter] = board[y, x];
                    counter++;
                }
            }    
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

        public static int[,] GetSurroundingMerge()
        {
            int[,] matrix = new int[5, 5];
            matrix[0, 0] = getCloseTile(-2, -2, 4);
            matrix[0, 1] = getCloseTile(-1, -2, 4);
            matrix[0, 2] = getCloseTile(0, -2, 4);
            matrix[0, 3] = getCloseTile(1, -2, 4);
            matrix[0, 4] = getCloseTile(2, -2, 4);

            matrix[1, 0] = getCloseTile(-2, -1, 4);
            matrix[1, 1] = getCloseTile(-1, -1, 4);
            matrix[1, 2] = getCloseTile(0, -1, 4);
            matrix[1, 3] = getCloseTile(1, -1, 4);
            matrix[1, 4] = getCloseTile(2, -1, 4);

            matrix[2, 0] = getCloseTile(-2, 0, 4);
            matrix[2, 1] = getCloseTile(-1, 0, 4);
            matrix[2, 2] = 100;
            matrix[2, 3] = getCloseTile(1, 0, 4);
            matrix[2, 4] = getCloseTile(2, 0, 4);

            matrix[3, 0] = getCloseTile(-2, 1, 4);
            matrix[3, 1] = getCloseTile(-1, 1, 4);
            matrix[3, 2] = getCloseTile(0, 1, 4);
            matrix[3, 3] = getCloseTile(1, 1, 4);
            matrix[3, 4] = getCloseTile(2, 1, 4);

            matrix[4, 0] = getCloseTile(-2, 2, 4);
            matrix[4, 1] = getCloseTile(-1, 2, 4);
            matrix[4, 2] = getCloseTile(0, 2, 4);
            matrix[4, 3] = getCloseTile(1, 2, 4);
            matrix[4, 4] = getCloseTile(2, 2, 4);

            return matrix;
        }

        public static int[,] GetThreeByThree(int layer)
        {
            int[,] matrix = new int[3, 3];

            matrix[0, 0] = getCloseTile(-1, -1, layer);
            matrix[0, 1] = getCloseTile(0, -1, layer);
            matrix[0, 2] = getCloseTile(1, -1, layer);

            matrix[1, 0] = getCloseTile(-1, 0, layer);
            matrix[1, 1] = 100;
            matrix[1, 2] = getCloseTile(1, 0, layer);

            matrix[2, 0] = getCloseTile(-1, 1, layer);
            matrix[2, 1] = getCloseTile(0, 1, layer);
            matrix[2, 2] = getCloseTile(1, 1, layer);

            return matrix;
        }

        public static int[,] GetXCoordinates()
        {
            int[,] matrix = new int[3, 3];

            matrix[0, 0] = xPosition - 16;
            matrix[0, 1] = xPosition;
            matrix[0, 2] = xPosition + 16;

            matrix[1, 0] = xPosition - 16;
            matrix[1, 1] = xPosition;
            matrix[1, 2] = xPosition + 16;

            matrix[2, 0] = xPosition - 16;
            matrix[2, 1] = xPosition;
            matrix[2, 2] = xPosition + 16;

            return matrix;
        }

        public static int[,] GetYCoordinates()
        {
            int[,] matrix = new int[3, 3];

            matrix[0, 0] = yPosition - 16;
            matrix[0, 1] = yPosition - 16;
            matrix[0, 2] = yPosition - 16;

            matrix[1, 0] = yPosition;
            matrix[1, 1] = yPosition;
            matrix[1, 2] = yPosition;

            matrix[2, 0] = yPosition + 16;
            matrix[2, 1] = yPosition + 16;
            matrix[2, 2] = yPosition + 16;

            return matrix;
        }

        public static void Draw(SpriteBatch spriteBatch, SpriteFont basicFont)
        {
            BaseSprite.Draw(spriteBatch);
            spriteBatch.DrawString(basicFont, "Health: " + HealthBar.GetHealth(), new Vector2(300, 10), Color.White);
        }
        #region Getters and Setters

        public int CurrentTile
        {
            get { return currentTile; }
            set { currentTile = value; }
        }

        internal static Sprite BaseSprite { get => baseSprite; set => baseSprite = value; }
        #endregion
    }
}
