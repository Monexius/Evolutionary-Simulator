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

        int lastScrollState;
        MouseState lastMouseState = new MouseState();
        KeyboardState lastKeyboardState = new KeyboardState();

        public void HandleInput(GameTime gameTime, Texture2D spriteSheet)
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
                Camera.ZoomAction(-0.1f);
            }
            else if (scrollState.ScrollWheelValue > lastScrollState)
            {
                //scrollx = Camera.WorldRectangle.X;
                //scrollx += 5;
                //scrolly = Camera.WorldRectangle.Y;
                //scrolly += 5;
                Camera.ZoomAction(0.1f);
            }
            //Player.Update(gameTime);
            if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                map = new Map();
                Map.Initialize(spriteSheet, 2, 4, 32);
                //Tiles,Smoothness of caves, tiles needed, Chance to spawn
                ScreenTransition.RunAnimation();
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

            ScreenTransition.Update(gameTime);
            lastScrollState = scrollState.ScrollWheelValue;
            lastMouseState = mouseState;
        }
    }
}
