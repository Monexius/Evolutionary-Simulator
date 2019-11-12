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
       
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1200;
            camera = new Camera();
            graphics.ApplyChanges();
            map = new Map();
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

            // Bigger Continents, neighbouring squares needed to change, chance of a water tile
            Map.Initialize(spriteSheet, 1, 4, 35);
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            Map.Draw(spriteBatch);
            ScreenTransition.Draw(spriteBatch, gameTime);
            //Player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
