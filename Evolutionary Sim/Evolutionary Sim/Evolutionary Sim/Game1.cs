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
        Texture2D healthTexture;
        ScreenTransition screenTransition;
        KeyboardManager keyboardManager;
        Map map;
        Camera camera;
        Sprite sprite;
        MapFruit fruit;
        List<MapFruit> fruits;
        Agent agent;
        HealthBar healthBar;
        SpriteFont basicFont;

        public static float currentTime = 0f;
        public static int screenHeight = 800;
        public static int screenWidth = 1600;
        public int rounded;
        public int hp = 100;

        // create a public list for the Fruit.cs so each instance of the object can be drawn in the draw with a foreach loop
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;

            camera = new Camera();
            graphics.ApplyChanges();
            map = new Map();
            fruits = new List<MapFruit>();
            keyboardManager = new KeyboardManager();
            screenTransition = new ScreenTransition();
            agent = new Agent();
            healthBar = new HealthBar();
            base.Initialize();

        }

        protected override void LoadContent()
        {
            circleTexture2D = Content.Load<Texture2D>("GreenCircle");
            spriteSheet = Content.Load<Texture2D>("Ev_TileSet");
            basicFont = Content.Load<SpriteFont>("font");
            healthTexture = Content.Load<Texture2D>("healthBar");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera.WorldRectangle = new Rectangle(0, 0, 3600, 3600); // define border of camera for map
            Camera.ViewPortWidth = GraphicsDevice.Viewport.Width;
            Camera.ViewPortHeight = GraphicsDevice.Viewport.Height;
            getMap(spriteSheet); // initialise map
            agent.Initialize(spriteSheet, healthTexture, new Rectangle(16, 48, 18, 18), 1);
            healthBar.Initialize(healthTexture, new Vector2(390, 10), new Rectangle(0, 0, hp, healthTexture.Height + 5), hp);
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
            agent.Update(gameTime);
            keyboardManager.HandleInput(gameTime, spriteSheet, healthTexture);
            base.Update(gameTime);
        }
        public void getMap(Texture2D spriteSheet)
        {
            Map.Initialize(spriteSheet, 2, 4, 33);
        }
        protected override void Draw(GameTime gameTime)
        {

            Texture2D textur;
            textur = new Texture2D(GraphicsDevice, 1, 1);
            textur.SetData(new Color[] { Color.Black });

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            rounded = (int)Math.Round(currentTime, 0);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            Map.Draw(spriteBatch, false,textur);
            Agent.Draw(spriteBatch, basicFont);

            spriteBatch.Draw(textur, new Rectangle(1501, 10, 48, 55), Color.White);

            spriteBatch.Draw(textur, new Rectangle((screenWidth - 400), (screenHeight - 125), 86, 20), Color.White);
            spriteBatch.DrawString(basicFont, "Layer 3", new Vector2((screenWidth - 395), (screenHeight - 123)), Color.White);
            spriteBatch.Draw(textur, new Rectangle((screenWidth - 300), (screenHeight - 125), 86, 20), Color.White);
            spriteBatch.DrawString(basicFont, "Layer 2", new Vector2((screenWidth - 295), (screenHeight - 123)), Color.White);
            spriteBatch.Draw(textur, new Rectangle((screenWidth - 200), (screenHeight - 225), 86, 20), Color.White);
            spriteBatch.DrawString(basicFont, "Layer 1", new Vector2((screenWidth - 195), (screenHeight - 223)), Color.White);

            spriteBatch.DrawString(basicFont, Agent.getCloseTile(-1, -1, 1).ToString(), new Vector2(1500, 10), Color.White); //Top-Left
            spriteBatch.DrawString(basicFont, Agent.getCloseTile(0, -1, 1).ToString(), new Vector2(1520, 10), Color.White); //Top
            spriteBatch.DrawString(basicFont, Agent.getCloseTile(1, -1, 1).ToString(), new Vector2(1540, 10), Color.White); //Top-Right

            spriteBatch.DrawString(basicFont, Agent.getCloseTile(-1, 0, 1).ToString(), new Vector2(1500, 30), Color.White); //Left
            spriteBatch.DrawString(basicFont, Agent.getCloseTile(0, 0, 1).ToString(), new Vector2(1520, 30), Color.White); //Center
            spriteBatch.DrawString(basicFont, Agent.getCloseTile(1, 0, 1).ToString(), new Vector2(1540, 30), Color.White); //Right

            spriteBatch.DrawString(basicFont, Agent.getCloseTile(-1, 1, 1).ToString(), new Vector2(1500, 50), Color.White); //Bottom-Left
            spriteBatch.DrawString(basicFont, Agent.getCloseTile(0, 1, 1).ToString(), new Vector2(1520, 50), Color.White); //Bottom
            spriteBatch.DrawString(basicFont, Agent.getCloseTile(1, 1, 1).ToString(), new Vector2(1540, 50), Color.White); //Bottom-Right

            ScreenTransition.Draw(spriteBatch, gameTime);
            HealthBar.Draw(spriteBatch);

            spriteBatch.DrawString(basicFont, "NEAT Simulation", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(basicFont, "Game Time: " + rounded.ToString(), new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(basicFont, "Generation: 0", new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(basicFont, "Current Tile ID: " + agent.CurrentTile.ToString(), new Vector2(10, 70), Color.White);
            
          

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}