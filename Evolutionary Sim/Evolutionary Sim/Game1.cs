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
        Map map;
        List<FadeItem> items = new List<FadeItem>();

        private float scale;
       
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            map = new Map();
            for (int xPixel = 32; xPixel - 32 < GraphicsDevice.Viewport.Width; xPixel += 100)
            {
                for (int yPixel = 32; yPixel - 32 < GraphicsDevice.Viewport.Height; yPixel += 100)
                {
                    items.Add(new FadeItem()
                    {
                        Xpos = xPixel,
                        Ypos = yPixel,
                        Delay = xPixel + yPixel
                    });
                    
                }
            }
            
            screenTransition = new ScreenTransition();
            base.Initialize();
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

            //Algorithm for spawning sprites here
            //Player.Initialize(spriteSheet, new Rectangle(0, 0, 16, 16), 2, new Vector2(300, 300)); // get tank and turret rectangle, then number of frames for animation


        }

        protected override void UnloadContent()
        {
           
        }
        MouseState lastMouseState = new MouseState();
        KeyboardState lastKeyboardState = new KeyboardState();

        protected override void Update(GameTime gameTime)
        {
            Vector2 moveUp = new Vector2(0, -10);
            Vector2 moveDown = new Vector2(0, 10);
            Vector2 moveRight = new Vector2(10, 0);
            Vector2 moveLeft = new Vector2(-10, 0);
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            //Player.Update(gameTime);
            if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                map = new Map();
                Map.Initialize(spriteSheet, 3, 4, 35);
               
            }
            if (keyboardState.IsKeyDown(Keys.W) && lastKeyboardState.IsKeyUp(Keys.W))
            {
                Camera.Move(moveUp);
            }
            if (keyboardState.IsKeyDown(Keys.S) && lastKeyboardState.IsKeyUp(Keys.S))
            {
                Camera.Move(moveDown);
            }
            if (keyboardState.IsKeyDown(Keys.D) && lastKeyboardState.IsKeyUp(Keys.D))
            {
                Camera.Move(moveRight);
            }
            if (keyboardState.IsKeyDown(Keys.A) && lastKeyboardState.IsKeyUp(Keys.A))
            {
                Camera.Move(moveLeft);
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Environment.Exit(0);
            }
            foreach (var item in items)
            {
                item.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            }      
            base.Update(gameTime);
            lastMouseState = mouseState;
        }

         
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            Map.Draw(spriteBatch);
            foreach (var item in items)
            {
                ScreenTransition.Draw(spriteBatch, gameTime, item.Xpos, item.Ypos, item.Scale);
            }
            //Player.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
