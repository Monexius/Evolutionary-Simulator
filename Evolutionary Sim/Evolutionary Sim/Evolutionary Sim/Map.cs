using Evolutionary_Sim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Animations
{
    class Map
    {
        #region Declarations
        public const int TileWidth = 16;
        public const int TileHeight = 16;
        public const int MapWidth = 500;
        public const int MapHeight = 1000;

        public static int generations;
        public static int caveThreshold = 5;
        public static int wallChancePerSquare = 0;


        static private Texture2D texture;

        public static Dictionary<int, Rectangle> tiles = new Dictionary<int, Rectangle>();

        static public int[,] mapSquares = new int[MapWidth, MapHeight]; // total amount of squares
        static public int[,] layer2 = new int[MapWidth, MapHeight]; // total amount of squares
        static public int[,] layer3 = new int[MapWidth, MapHeight]; // total amount of squares

        public static ArrayList mapFruitBushesX = new ArrayList();
        public static ArrayList mapFruitBushesY = new ArrayList();

        static private Random rnd = new Random();

        public static MapFruit fruit;

        #endregion
        //public int mapFruitBushesX { get; set; }
        //public int mapFruitBushesY { get; set; }

        #region Initialization
        static public void Initialize(Texture2D tileTexture, int cycles, int caveDensity, int wallChance)
        {
            generations = cycles;
            wallChancePerSquare = wallChance;
            caveThreshold = caveDensity;
            texture = tileTexture;

            tiles.Clear();
            tiles.Add(0, new Rectangle(Tile(0), Tile(10), TileWidth, TileHeight));//Empty
            tiles.Add(1, new Rectangle(Tile(0), Tile(0), TileWidth, TileHeight)); //Grass Tile 1
            tiles.Add(2, new Rectangle(Tile(0), Tile(1), TileWidth, TileHeight)); //Water Tile 1

            tiles.Add(3, new Rectangle(Tile(1), Tile(0), TileWidth, TileHeight)); //Corner Top Left
            tiles.Add(4, new Rectangle(Tile(2), Tile(0), TileWidth, TileHeight)); //Top
            tiles.Add(5, new Rectangle(Tile(3), Tile(0), TileWidth, TileHeight)); //Corner Top RIght
            tiles.Add(6, new Rectangle(Tile(4), Tile(0), TileWidth, TileHeight)); //In Corner Top Left
            tiles.Add(7, new Rectangle(Tile(5), Tile(0), TileWidth, TileHeight)); //In Top
            tiles.Add(8, new Rectangle(Tile(6), Tile(0), TileWidth, TileHeight)); //In Corner Top Right

            tiles.Add(9, new Rectangle(Tile(1), Tile(1), TileWidth, TileHeight)); //Left
            tiles.Add(10, new Rectangle(Tile(2), Tile(1), TileWidth, TileHeight)); //Grass Tile 1
            tiles.Add(11, new Rectangle(Tile(3), Tile(1), TileWidth, TileHeight)); //Right
            tiles.Add(12, new Rectangle(Tile(4), Tile(1), TileWidth, TileHeight)); //In Right
            tiles.Add(13, new Rectangle(Tile(5), Tile(1), TileWidth, TileHeight)); //Water Tile 1
            tiles.Add(14, new Rectangle(Tile(6), Tile(1), TileWidth, TileHeight)); //In Left

            tiles.Add(15, new Rectangle(Tile(1), Tile(2), TileWidth, TileHeight)); //Corner Bottom Left
            tiles.Add(16, new Rectangle(Tile(2), Tile(2), TileWidth, TileHeight)); //Bottom
            tiles.Add(17, new Rectangle(Tile(3), Tile(2), TileWidth, TileHeight)); //Corner Bottom Right
            tiles.Add(18, new Rectangle(Tile(4), Tile(2), TileWidth, TileHeight)); //In Corner Bottom Left
            tiles.Add(19, new Rectangle(Tile(5), Tile(2), TileWidth, TileHeight)); //In Bottom
            tiles.Add(20, new Rectangle(Tile(6), Tile(2), TileWidth, TileHeight)); //In Corner Bottom Right
            //Plants
            tiles.Add(21, new Rectangle(Tile(0), Tile(5), TileWidth, TileHeight)); // PLant Bush
            //Fruit
            tiles.Add(40, new Rectangle(Tile(0), Tile(6), TileWidth, TileHeight)); // Apple
            tiles.Add(41, new Rectangle(Tile(1), Tile(6), TileWidth, TileHeight)); // Pear
            tiles.Add(42, new Rectangle(Tile(2), Tile(6), TileWidth, TileHeight)); // Cherry

            tiles.Add(22, new Rectangle(Tile(0), Tile(2), TileWidth, TileHeight)); //Mud Tile 1

            GenerateRandomMap();
            GenerateCaves();
            GenerateRandomMud();
            spawnMudArea();
            GenerateTerrain();

            generateDetails();
            //spawnMudArea();
        }
        #endregion

        #region Information about Map Squares
        public static int Tile(int value)
        {
            return value * 16;
        }
        static public int GetSquareByPixelX(int pixelX) // convert pixel co-ordinates to square references
        {
            return pixelX / TileWidth;
        }
        static public int GetSquareByPixelY(int pixelY)
        {
            return pixelY / TileHeight;
        }
        static public Vector2 GetSquareAtPixel(Vector2 pixelLocation) // returns the square position where the pixel reference is at
        {
            return new Vector2(GetSquareByPixelX((int)pixelLocation.X), GetSquareByPixelY((int)pixelLocation.Y));
        }
        static public Vector2 GetSquareCenter(int squareX, int squareY) // returns the center of a square
        {
            return new Vector2((squareX * TileWidth) + (TileWidth / 2), (squareY * TileHeight + (TileHeight / 2)));
        }
        static public Vector2 GetSquareCenter(Vector2 square)
        {
            return GetSquareCenter((int)square.X, (int)square.Y);
        }
        static public Rectangle SquareWorldRectangle(int x, int y) // checks all the pixels within a square
        {
            return new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
        }
        static public Rectangle SquareWorldRectangle(Vector2 square)
        {
            return SquareWorldRectangle((int)square.X, (int)square.Y);
        }
        static public Rectangle SquareScreenRectangle(int x, int y) // does the same but in screen co ordinates
        {
            return Camera.Transform(SquareWorldRectangle(x, y));
        }
        static public Rectangle SquareScreenRectangle(Vector2 square)
        {
            return SquareScreenRectangle((int)square.X, (int)square.Y);
        }
        #endregion

        #region Information about Map Tiles

        static public int GetTileAtSquare(int tileX, int tileY, int[,] layer) // gets the tile index [x,y]
        {
            if ((tileX >= 0) && (tileX < MapWidth) && // gets square till end of screen
               (tileY >= 0) && (tileY < MapHeight))
            {
                return layer[tileX, tileY];
            }
            else
            {
                return -1;
            }
        }

        static public void SetTileAtSquare(int tileX, int tileY, int tile, int[,] layer) // allows the index of a tile to be changed
        {
            if ((tileX >= 0) && (tileX < MapWidth) &&
               (tileY >= 0) && (tileY < MapHeight))
            {
                layer[tileX, tileY] = tile;
            }
        }

        static public int GetTileAtPixel(int pixelX, int pixelY, int[,] layer) // gets a tile if a pixel is in it
        {
            return GetTileAtSquare(GetSquareByPixelX(pixelX),
                                   GetSquareByPixelY(pixelY),
                                   layer);
        }

        static public int GetTileAtPixel(Vector2 pixelLocation, int[,] layer)
        {
            return GetTileAtPixel((int)pixelLocation.X,
                                  (int)pixelLocation.Y,
                                  layer);
        }

        static public bool IsWallTile(int tileX, int tileY, int[,] layer) // checks if tile is a wall
        {
            int tileIndex = GetTileAtSquare(tileX, tileY, layer);

            if (tileIndex == -1)
            {
                return false;
            }

            return tileIndex >= 2; // water tile and up
        }

        static public bool IsWallTile(Vector2 square, int[,] layer) // checks if tile is solid
        {
            return IsWallTile((int)square.X, (int)square.Y, layer);
        }

        static public bool IsWallTileByPixel(Vector2 pixelLocation, int[,] layer)
        {
            return IsWallTile(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y),
                layer);
        }
        #endregion

        #region Drawing
        static public void Draw(SpriteBatch spriteBatch, bool repeat, Texture2D textur)
        {
            int startX = GetSquareByPixelX((int)Camera.Position.X); // decides how many squares should be drawn
            int endX = GetSquareByPixelX((int)Camera.Position.X + Camera.ViewPortWidth);

            int startY = GetSquareByPixelY((int)Camera.Position.Y);
            int endY = GetSquareByPixelY((int)Camera.Position.Y + Camera.ViewPortHeight);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight)) // Layer 1
                    {
                        spriteBatch.Draw(texture, SquareScreenRectangle(x, y), tiles[GetTileAtSquare(x, y, mapSquares)], Color.White); // get square gets the position in which to draw
                    }
                }
            }
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight)) // Layer 2
                    {
                        spriteBatch.Draw(texture, SquareScreenRectangle(x, y), tiles[GetTileAtSquare(x, y, layer2)], Color.White); // get square gets the position in which to draw
                    }
                }
            }
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight)) // Layer 3
                    {
                        spriteBatch.Draw(texture, SquareScreenRectangle(x, y), tiles[GetTileAtSquare(x, y, layer3)], Color.White); // get square gets the position in which to draw
                    }
                }
            }
            spriteBatch.Draw(textur, new Rectangle(1396, 7, 56, 55), Color.White);
            int[,] surroundingTiles = Agent.GetSurroundingTiles();
            for (int x = 0; x < 3; x++)
            { //surroundingTiles[x, y]
                for (int y = 0; y < 3; y++)
                {
                    spriteBatch.Draw(texture, new Rectangle(1400 + (16 * x),10 + (16 * y),16,16), tiles[surroundingTiles[y,x]], Color.White);
                }
            }
        }

        public static void ClearArray()
        {
            mapSquares = new int[MapWidth, MapHeight];
            layer2 = new int[MapWidth, MapHeight];
            layer3 = new int[MapWidth, MapHeight];

        }
        #endregion

        #region Map Generation
        static public void GenerateRandomMap()
        {
            //My input
            bool isFreeSpace = false;
            int roomHeight = 6;
            int roomWidth = 6;

            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++) // 50x50
                {
                    mapSquares[x, y] = 1; // grass tile

                    //if ((x == 0) || (y == 0) || (x == MapWidth - 1) || (y == MapHeight - 1))
                    //{
                    //    mapSquares[x, y] = WallTile; // generates walls around the map
                    //    continue;
                    //}
                    //if ((x == 1) || (y == 1) || (x == MapWidth - 2) || (y == MapHeight - 2)) // within the border
                    //{
                    //    continue;
                    //}
                    //if (x > 7 && y > 7)
                    //{
                    //    if (rnd.Next(0, 100) <= wallChancePerSquare) // chance to spawn wall
                    //    {
                    //        for (int l = 0; l < roomWidth; l++) // get area around the tile
                    //        {
                    //            for (int b = 0; b < roomHeight; b++)
                    //            {
                    //                if (IsWallTile(x - l, y - b) || IsWallTile(x + l, y + b)
                    //                   || IsWallTile(x + l, y - b) || IsWallTile(-+l, y + b))
                    //                {
                    //                    mapSquares[x, y] = FoodTile;
                    //                    isFreeSpace = false;
                    //                }
                    //            }
                    //        }


                    //    }
                    //}

                    if (rnd.Next(0, 100) <= wallChancePerSquare)
                    { // chance to spawn wall
                        for (int l = 0; l < roomWidth; l++) // get area around the tile
                        {
                            for (int b = 0; b < roomHeight; b++)
                            {
                                mapSquares[x, y] = 2; // water tile
                                isFreeSpace = false;

                            }
                        }
                    }
                    //if (rnd.Next(0, 100) <= wallChancePerSquare)
                    //{ // chance to spawn wall
                    //    for (int l = 0; l < roomWidth; l++) // get area around the tile
                    //    {
                    //        for (int b = 0; b < roomHeight; b++)
                    //        {
                    //            mapSquares[x, y] = 22; // water tile
                    //            isFreeSpace = false;

                    //        }
                    //    }
                    //}
                }
        }

        static public void GenerateRandomMud()
        {
            int roomHeight = 6;
            int roomWidth = 6;
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++) // 50x50
                {
                    if (rnd.Next(0, 100) <= wallChancePerSquare && GetTileAtSquare(x, y, mapSquares) == 1)
                    { // chance to spawn wall
                        for (int l = 0; l < roomWidth; l++) // get area around the tile
                        {
                            for (int b = 0; b < roomHeight; b++)
                            {
                                mapSquares[x, y] = 22; // mud tile
                            }
                        }
                    }
                }
            }
        }
        static public void GenerateCaves()
        {
            for (int g = 0; g < generations; g++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++) // 50x50
                    {
                        int boxCounter = 0;
                        if (IsWallTile(x + 1, y, mapSquares)) // Right
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x - 1, y, mapSquares)) // Left
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x, y - 1, mapSquares)) // Up
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x, y + 1, mapSquares)) // Down
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x + 1, y - 1, mapSquares)) // Right + Up
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x + 1, y + 1, mapSquares)) // Right + Down
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x - 1, y - 1, mapSquares)) // Left + Up
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x - 1, y + 1, mapSquares)) // Left + Up
                        {
                            boxCounter++;
                        }
                        if (IsWallTile(x, y, mapSquares))// if water
                        {
                            if (boxCounter < caveThreshold)
                            {
                                mapSquares[x, y] = 1;// grass tile
                            }
                        }
                        if (!IsWallTile(x, y, mapSquares)) // if floor
                        {
                            if (boxCounter >= caveThreshold)
                            {
                                mapSquares[x, y] = 2; // water tile
                            }
                        }
                    }
                }
            }
        }
        public static int counter;
        static public void spawnBush(int x, int y)
        {

            if (GetTileAtSquare(x, y, mapSquares) == 1)// if floor
            {
                bool surroundBarrier = false;
                if (GetTileAtSquare(x + 1, y, mapSquares) == 2 || GetTileAtSquare(x + 1, y, layer2) == 21) // Right
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x - 1, y, mapSquares) == 2 || GetTileAtSquare(x - 1, y, layer2) == 21) // Left
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x, y - 1, mapSquares) == 2 || GetTileAtSquare(x, y - 1, layer2) == 21) // Up
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x, y + 1, mapSquares) == 2 || GetTileAtSquare(x, y + 1, layer2) == 21) // Down
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x + 1, y - 1, mapSquares) == 2 || GetTileAtSquare(x + 1, y - 1, layer2) == 21) // Right + Up
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x + 1, y + 1, mapSquares) == 2 || GetTileAtSquare(x + 1, y + 1, layer2) == 21) // Right + Down
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x - 1, y - 1, mapSquares) == 2 || GetTileAtSquare(x - 1, y - 1, layer2) == 21) // Left + Up
                {
                    surroundBarrier = true;
                }
                if (GetTileAtSquare(x - 1, y + 1, mapSquares) == 2 || GetTileAtSquare(x - 1, y + 1, layer2) == 21) // Left + Up
                {
                    surroundBarrier = true;
                }
                if (surroundBarrier == false)
                    if (rnd.Next(32, 99) <= wallChancePerSquare)
                    { // chance to spawn grass
                        counter++;
                        layer2[x, y] = 21;
                        mapFruitBushesX.Add(x);
                        mapFruitBushesY.Add(y);

                        if (rnd.Next(32, 99) > 50)
                        {
                            layer3[x, y] = 40;
                        }
                        else
                        {
                            layer3[x, y] = 41;
                        }


                    }
            }
        }
        static public void spawnMudArea()
        {
            int threshold = 3;
            for (int g = 0; g < 5; g++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++) // 50x50
                    {
                        int boxCounter = 0;
                        if (GetTileAtSquare(x + 1, y, mapSquares) == 22) // Right
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x - 1, y, mapSquares) == 22) // Left
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x, y - 1, mapSquares) == 22) // Up
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x, y + 1, mapSquares) == 22) // Down
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x + 1, y - 1, mapSquares) == 22) // Right + Up
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x + 1, y + 1, mapSquares) == 22) // Right + Down
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x - 1, y - 1, mapSquares) == 22) // Left + Up
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x - 1, y + 1, mapSquares) == 22) // Left + Up
                        {
                            boxCounter++;
                        }
                        if (GetTileAtSquare(x, y, mapSquares) == 22)// if grass
                        {
                            if (boxCounter < threshold)
                            {
                                mapSquares[x, y] = 1;// grass tile
                            }
                        }

                    }
                }
            }
        }

        //static public void spawnWater(int x, int y)
        //{
        //    if (GetTileAtSquare(x, y) == 1)// if floor
        //    {
        //        bool surroundBarrier = false;
        //        if (GetTileAtSquare(x + 1, y) == 2) // Right
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x - 1, y) == 2) // Left
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x, y - 1) == 2) // Up
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x, y + 1) == 2) // Down
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x + 1, y - 1) == 2) // Right + Up
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x + 1, y + 1) == 2) // Right + Down
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x - 1, y - 1) == 2) // Left + Up
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (GetTileAtSquare(x - 1, y + 1) == 2) // Left + Up
        //        {
        //            surroundBarrier = true;
        //        }
        //        if (surroundBarrier == false)
        //            if (rnd.Next(0, 1200) <= wallChancePerSquare) // chance to spawn grass
        //                mapSquares[x, y] = 4;

        //    }
        //}
        static public void generateDetails()
        {

            /*
             * Right, Right Bottom, Down, Left Bottom, Left, Left Top, Top, Top Right
             * 
             * 
             * 
             * */
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (GetTileAtSquare(x, y, mapSquares) == 1)
                    {
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If bottom
                        //{
                        //    mapSquares[x, y] = 7;
                        //}
                        if (GetTileAtSquare(x, y - 1, mapSquares) == 2) // If top
                        {
                            mapSquares[x, y] = 4;
                        }
                        if (GetTileAtSquare(x, y + 1, mapSquares) == 2) // If bottom
                        {
                            mapSquares[x, y] = 7;
                        }
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If bottom
                        //{
                        //    mapSquares[x, y] = 7;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If bottom
                        //{
                        //    mapSquares[x, y] = 7;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If bottom
                        //{
                        //    mapSquares[x, y] = 7;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //   && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If bottom
                        //{
                        //    mapSquares[x, y] = 7;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //   && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If left
                        //{
                        //    mapSquares[x, y] = 9;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //  && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If left
                        //{
                        //    mapSquares[x, y] = 9;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) == 2
                        //  && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If left
                        //{
                        //    mapSquares[x, y] = 9;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) == 2
                        //  && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If left
                        //{
                        //    mapSquares[x, y] = 9;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        // && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If right
                        //{
                        //    mapSquares[x, y] = 11;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        // && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If right
                        //{
                        //    mapSquares[x, y] = 11;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        // && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If right
                        //{
                        //    mapSquares[x, y] = 11;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        // && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If right
                        //{
                        //    mapSquares[x, y] = 11;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If top
                        //{
                        //    mapSquares[x, y] = 4;
                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If top
                        //{
                        //    mapSquares[x, y] = 4;
                        //}
                        //if (GetTileAtSquare(x, y - 1) == 2 ) // If top
                        //{
                        //    mapSquares[x, y] = 4;
                        //}
                        //if(GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If top
                        //{
                        //    mapSquares[x, y] = 4;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Top Right
                        //{
                        //    mapSquares[x, y] = 5;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //   && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Top Right
                        //{
                        //    mapSquares[x, y] = 5;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //   && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Top Right
                        //{
                        //    mapSquares[x, y] = 5;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //  && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Top Right
                        //{
                        //    mapSquares[x, y] = 5;
                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Bottom Right Corner
                        //{
                        //    mapSquares[x, y] = 17;

                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Bottom Right Corner
                        //{
                        //    mapSquares[x, y] = 17;

                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //   && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Bottom Right Corner
                        //{
                        //    mapSquares[x, y] = 17;

                        //}
                        //if (GetTileAtSquare(x + 1, y) == 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //   && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Bottom Right Corner
                        //{
                        //    mapSquares[x, y] = 17;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //        && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Bottom Left Corner
                        //{
                        //    mapSquares[x, y] = 15;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //        && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Bottom Left Corner
                        //{
                        //    mapSquares[x, y] = 15;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //       && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Bottom Left Corner
                        //{
                        //    mapSquares[x, y] = 15;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) == 2
                        //      && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Bottom Left Corner
                        //{
                        //    mapSquares[x, y] = 15;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //        && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If In Corner Top Right
                        //{
                        //    mapSquares[x, y] = 8;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //       && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If In Corner Top Left
                        //{
                        //    mapSquares[x, y] = 6;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) == 2
                        //      && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If Corner Top Left
                        //{
                        //    mapSquares[x, y] = 3;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) == 2
                        //      && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) == 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If Corner Top Left
                        //{
                        //    mapSquares[x, y] = 3;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //       && GetTileAtSquare(x - 1, y - 1) == 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If In Corner Bottom Right
                        //{
                        //    mapSquares[x, y] = 20;

                        //}
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) != 2 && GetTileAtSquare(x, y + 1) != 2 && GetTileAtSquare(x - 1, y + 1) != 2 && GetTileAtSquare(x - 1, y) != 2
                        //      && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) == 2) // If In Corner Bottom Left
                        //{
                        //    mapSquares[x, y] = 18;

                        //}

                    }
                }
            }
        }
        static public void GenerateTerrain()
        { /////////////////////////////////////////Grass
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    spawnBush(x, y);
                }
            }
            /////////////////////////////////////////Water
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    //spawnWater(x, y);
                }
            }

        }
        #endregion
    }
}


