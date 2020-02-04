using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class HealthBar
    {
        private static Texture2D texture;
        private static Vector2 position;
        private static Rectangle rectangle;
        private static int currentHealth;

        public void Initialize(Texture2D Texture, Vector2 Position, Rectangle Rectangle, int health)
        {
            texture = Texture;
            position = Position;
            rectangle = Rectangle;
            currentHealth = health;
        }

        public static int GetHealth()
        { 
            return currentHealth;
        }

        public void RemoveHealth(int hp)
        {
            currentHealth -= hp;
            rectangle.Width -= hp;
        }

        public void AddHealth(int hp)
        {
            currentHealth += hp;
            rectangle.Width += hp;
        }

        public void Update(GameTime gameTime)
        {
            
        }
 
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, rectangle, Color.White);
        }
    }
}
