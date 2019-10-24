using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
           // sprite = new Sprite(new Vector2(5, -5), spriteSheet, new Rectangle(32, 16, 0, 48), new Vector2(1, 1));
            
            keyboardManager = new KeyboardManager();
            screenTransition = new ScreenTransition();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            circleTexture2D = Content.Load<Texture2D>("GreenCircle");
            spriteSheet = Content.Load<Texture2D>("Ev_TileSet");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera.WorldRectangle = new Rectangle(0, 0, 3600, 3600); // define border of camera for map
            Camera.ViewPortWidth = GraphicsDevice.Viewport.Width;
            Camera.ViewPortHeight = GraphicsDevice.Viewport.Height;
            for (int x = 0; x < Map.mapFruitBushesX.Length; x++)
            {
                sprite = new Sprite(new Vector2(Map.mapFruitBushesX[x] * 16, Map.mapFruitBushesY[x] * 16), spriteSheet, new Rectangle(0, 96, 16, 16));
            }
            
            // Bigger Continents, neighbouring squares needed to change, chance of a water tile
            getMap(spriteSheet); // initialise map
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            Map.Draw(spriteBatch, false);
            ScreenTransition.Draw(spriteBatch, gameTime);
            //sprite.Draw(spriteBatch);
            //foreach(Sprite sprite in)
            //{
            //    //Make a list class
            //}
            sprite.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}