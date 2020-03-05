using Map_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class KeyboardManager
    {
        Map map;
        Agent agent;
        Game1 game1;
        HealthBar health;
        int lastScrollState;
        
        MouseState lastMouseState = new MouseState();
        KeyboardState lastKeyboardState = new KeyboardState();

        public void HandleInput(GameTime gameTime, Texture2D spriteSheet , Texture2D healthTexture)
        {
            MouseState scrollState = Mouse.GetState();
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            Vector2 moveUp = new Vector2(0, -20);
            Vector2 moveDown = new Vector2(0, 20);
            Vector2 moveRight = new Vector2(20, 0);
            Vector2 moveLeft = new Vector2(-20, 0);

            if (scrollState.ScrollWheelValue < lastScrollState)
            {
                //scrollx = Camera.WorldRectangle.X;
                //scrollx -= 5;
                //scrolly = Camera.WorldRectangle.Y;
                //scrolly -= 5;
                camera.ZoomAction(-0.1f);
            }
            else if (scrollState.ScrollWheelValue > lastScrollState)
            {
                //scrollx = Camera.WorldRectangle.X;
                //scrollx += 5;
                //scrolly = Camera.WorldRectangle.Y;
                //scrolly += 5;
                camera.ZoomAction(0.1f);
            }
            //Player.Update(gameTime);
            if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                Map.ClearArray();
                map = new Map();
                game1.getMap(spriteSheet); // initialise map
                health.AddHealth(game1.hp);
                //health.AddHealth(game1.hp);
                //Game1.currentTime = 0f;
                //agent.Initialize(spriteSheet, healthTexture, new Rectangle(16, 48, 18, 18), 1);

                //Map.Draw(spriteBatch);
                //Tiles,Smoothness of caves, tiles needed, Chance to spawn

                ScreenTransition.RunAnimation();
            }
            if (keyboardState.IsKeyDown(Keys.W) && lastKeyboardState.IsKeyUp(Keys.W))
            {
                camera.Move(moveUp);
            }
            if (keyboardState.IsKeyDown(Keys.S) && lastKeyboardState.IsKeyUp(Keys.S))
            {
                camera.Move(moveDown);
            }
            if (keyboardState.IsKeyDown(Keys.D) && lastKeyboardState.IsKeyUp(Keys.D))
            {
                camera.Move(moveRight);
            }
            if (keyboardState.IsKeyDown(Keys.A) && lastKeyboardState.IsKeyUp(Keys.A))
            {
                camera.Move(moveLeft);
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Environment.Exit(0);
            }

            ScreenTransition.Update(gameTime);
            lastScrollState = scrollState.ScrollWheelValue;
            lastMouseState = mouseState;
        }
    }
}
