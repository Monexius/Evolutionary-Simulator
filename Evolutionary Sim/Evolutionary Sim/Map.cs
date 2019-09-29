﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        public const int MapWidth = 1000;
        public const int MapHeight = 500;

      
        public static int generations;
        public static int caveThreshold = 5;
        public static int wallChancePerSquare = 0;

        static private Texture2D texture;

        public static Dictionary<int,Rectangle> tiles = new Dictionary<int, Rectangle>();
        
        static private int[,] mapSquares = new int[MapWidth, MapHeight]; // total amount of squares

        static private Random rnd = new Random();

        #endregion

        #region Initialization
        static public void Initialize(Texture2D tileTexture, int cycles, int caveDensity, int wallChance)
        {
            generations = cycles;
            wallChancePerSquare = wallChance;
            caveThreshold = caveDensity;
            texture = tileTexture;

            tiles.Clear();
            tiles.Add(1,new Rectangle(Tile(0), Tile(0), TileWidth, TileHeight)); //Grass Tile 1
            tiles.Add(2,new Rectangle(Tile(0), Tile(1), TileWidth, TileHeight)); //Water Tile 1

            tiles.Add(3, new Rectangle(Tile(1), Tile(0), TileWidth, TileHeight)); //Corner Top Left
            tiles.Add(4, new Rectangle(Tile(2), Tile(0), TileWidth, TileHeight)); //Top
            tiles.Add(5, new Rectangle(Tile(3), Tile(0), TileWidth, TileHeight)); //Corner Top RIght
            tiles.Add(6, new Rectangle(Tile(4), Tile(0), TileWidth, TileHeight)); //In Corner Top Left
            tiles.Add(7, new Rectangle(Tile(5), Tile(0), TileWidth, TileHeight)); //In Top
            tiles.Add(8, new Rectangle(Tile(6), Tile(0), TileWidth, TileHeight)); //In Corner Top Right

            tiles.Add(9,  new Rectangle(Tile(1), Tile(1), TileWidth, TileHeight)); //Left
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




            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++) // initialising each tile
                {
                    mapSquares[x, y] = 1;
                }
            }

            GenerateRandomMap();
            GenerateCaves();
            GenerateTerrain();
            generateDetails(); 
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

        static public int GetTileAtSquare(int tileX, int tileY) // gets the tile index [x,y]
        {
            if ((tileX >= 0) && (tileX < MapWidth) && // gets square till end of screen
               (tileY >= 0) && (tileY < MapHeight))
            {
                return mapSquares[tileX, tileY];
            }
            else
            {
                return -1;
            }
        }

        static public void SetTileAtSquare(int tileX, int tileY, int tile) // allows the index of a tile to be changed
        {
            if ((tileX >= 0) && (tileX < MapWidth) &&
               (tileY >= 0) && (tileY < MapHeight))
            {
                mapSquares[tileX, tileY] = tile;
            }
        }

        static public int GetTileAtPixel(int pixelX, int pixelY) // gets a tile if a pixel is in it
        {
            return GetTileAtSquare(GetSquareByPixelX(pixelX),
                                   GetSquareByPixelY(pixelY));
        }

        static public int GetTileAtPixel(Vector2 pixelLocation)
        {
            return GetTileAtPixel((int)pixelLocation.X,
                                  (int)pixelLocation.Y);
        }
       
        static public bool IsWallTile(int tileX, int tileY) // checks if tile is a wall
        {
            int tileIndex = GetTileAtSquare(tileX, tileY);

            if (tileIndex == -1)
            {
                return false;
            }

            return tileIndex >= 2; // water tile and up
        }

        static public bool IsWallTile(Vector2 square) // checks if tile is solid
        {
            return IsWallTile((int)square.X, (int)square.Y);
        }

        static public bool IsWallTileByPixel(Vector2 pixelLocation)
        {
            return IsWallTile(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y));
        }
        #endregion

        #region Drawing
        static public void Draw(SpriteBatch spriteBatch)
        {
            int startX = GetSquareByPixelX((int)Camera.Position.X); // decides how many squares should be drawn
            int endX = GetSquareByPixelX((int)Camera.Position.X + Camera.ViewPortWidth);

            int startY = GetSquareByPixelY((int)Camera.Position.Y);
            int endY = GetSquareByPixelY((int)Camera.Position.Y + Camera.ViewPortHeight);

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
                    {
                        spriteBatch.Draw(texture, SquareScreenRectangle(x, y), tiles[GetTileAtSquare(x, y)], Color.White); // get square gets the position in which to draw
                    }
                }
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
                            if (IsWallTile(x + 1, y)) // Right
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x - 1, y)) // Left
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x, y - 1)) // Up
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x, y + 1)) // Down
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x + 1, y - 1)) // Right + Up
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x + 1, y + 1)) // Right + Down
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x - 1, y - 1)) // Left + Up
                            {
                                boxCounter++;
                            }
                             if (IsWallTile(x - 1, y + 1)) // Left + Up
                            {
                                boxCounter++;
                            }
                        if (IsWallTile(x, y))// if water
                        {
                            if (boxCounter < caveThreshold)
                            {
                                mapSquares[x, y] = 1;// grass tile
                            }
                        }
                        if (!IsWallTile(x, y)) // if floor
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
        //static public void spawnGrass(int x, int y)
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
        //            if (rnd.Next(0, 80) <= wallChancePerSquare) // chance to spawn grass
        //                mapSquares[x, y] = 3;

        //    }
        //}

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
                    if (GetTileAtSquare(x, y) == 1)
                    {
                        //if (GetTileAtSquare(x + 1, y) != 2 && GetTileAtSquare(x + 1, y + 1) == 2 && GetTileAtSquare(x, y + 1) == 2 && GetTileAtSquare(x - 1, y + 1) == 2 && GetTileAtSquare(x - 1, y) != 2
                        //    && GetTileAtSquare(x - 1, y - 1) != 2 && GetTileAtSquare(x, y - 1) != 2 && GetTileAtSquare(x + 1, y - 1) != 2) // If bottom
                        //{
                        //    mapSquares[x, y] = 7;
                        //}
                        if (GetTileAtSquare(x, y - 1) == 2) // If top
                        {
                            mapSquares[x, y] = 4;
                        }
                        if (GetTileAtSquare(x, y + 1) == 2 ) // If bottom
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
                    //spawnGrass(x, y);

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

