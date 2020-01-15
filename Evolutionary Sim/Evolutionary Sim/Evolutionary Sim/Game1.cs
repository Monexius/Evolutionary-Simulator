using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Evolutionary_Sim
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D spriteSheet;
        Texture2D circleTexture2D;
        ScreenTransition screenTransition;
        KeyboardManager keyboardManager;
        Map map;
        Camera camera;
        Sprite sprite;
        MapFruit fruit;
        List<MapFruit> fruits;
        Agent player;

        SpriteFont basicFont;
        public static float currentTime = 0f;
        int rounded;
        // create a public list for the Fruit.cs so each instance of the object can be drawn in the draw with a foreach loop
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1600;
            camera = new Camera();
            graphics.ApplyChanges();
            map = new Map();
            fruits = new List<MapFruit>();
            keyboardManager = new KeyboardManager();
            screenTransition = new ScreenTransition();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            circleTexture2D = Content.Load<Texture2D>("GreenCircle");
            spriteSheet = Content.Load<Texture2D>("Ev_TileSet");
            basicFont = Content.Load<SpriteFont>("font");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera.WorldRectangle = new Rectangle(0, 0, 3600, 3600); // define border of camera for map
            Camera.ViewPortWidth = GraphicsDevice.Viewport.Width;
            Camera.ViewPortHeight = GraphicsDevice.Viewport.Height;
            getMap(spriteSheet); // initialise map
            Agent.Initialize(spriteSheet, new Rectangle(16,48,18,18),1, new Vector2(64, 64));
        
            // Bigger Continents, neighbouring squares needed to change, chance of a water tile
            //for (int x = 0; x < Map.mapFruitBushesX.Count; x++)
            //{
            //    fruits.Add(new MapFruit(spriteSheet, new Rectangle(0, 96, 16, 16), 1, new Vector2((Map.mapFruitBushesX.IndexOf(x)), (Map.mapFruitBushesY.IndexOf(x)))));
            //}
            Debug.WriteLine(fruits.Count);
            ScreenTransition.Initialise(circleTexture2D);
            ScreenTransition.RunAnimation();

            //Algorithm for spawning sprites here
            //Player.Initialize(spriteSheet, new Rectangle(0, 0, 16, 16), 2, new Vector2(300, 300)); // get tank and turret rectangle, then number of frames for animation
        }

        protected override void UnloadContent()
        {
           
        }
        
        protected override void Update(GameTime gameTime)
        {
            keyboardManager.HandleInput(gameTime, spriteSheet);
            base.Update(gameTime);
        }
        public void getMap(Texture2D spriteSheet)
        {
            Map.Initialize(spriteSheet, 2, 4, 33);
        }
        protected override void Draw(GameTime gameTime)
        {
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            rounded = (int)Math.Round(currentTime, 0);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            Map.Draw(spriteBatch, false);
            Agent.Draw(spriteBatch);
            ScreenTransition.Draw(spriteBatch, gameTime);
            spriteBatch.DrawString(basicFont, "NEAT Simulation", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(basicFont, "Game Time: " + rounded.ToString(), new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(basicFont, "Generation: 0", new Vector2(10, 50), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}