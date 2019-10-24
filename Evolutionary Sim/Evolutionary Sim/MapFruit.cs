using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    class MapFruit
    {
        public Texture2D Texture;

        private Vector2 worldLocation = Vector2.Zero;
        private Rectangle Frame;
        private Color tintColor = Color.White;
        private bool Interactive = false;
        private bool Animated = false;
        private int Damage = 0;
        private int Heal = 0;
        public bool Collidable = true;
        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;
        private int FrameHeight = 16;
        private int FrameWidth = 16;

        public MapFruit (Vector2 worldLocation, Texture2D graphic, Rectangle frame, bool interactive, bool animate, int damage, int heal)
        {
            this.worldLocation = worldLocation;
            Texture = graphic;
            Frame = frame;
            Interactive = interactive;
            Animated = animate;
            Damage = damage;
            Heal = heal;
        }

        public static void OnInteract(int x, int y)
        {

        }

        public static void Animate()
        {

        }

        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }
        public Vector2 WorldCenter
        {
            get { return worldLocation + RelativeCenter; }
        }
        public Vector2 RelativeCenter
        {
            get { return new Vector2(FrameWidth / 2, FrameHeight / 2); }
        }
        #region Collision Related Properties
        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle((int)worldLocation.X + BoundingXPadding, // returns a bounding rectangle to an entity
                                     (int)worldLocation.Y + BoundingYPadding,
                                     FrameWidth - (BoundingXPadding * 2),
                                     FrameHeight - (BoundingYPadding * 2));
            }
        }
        #endregion

        #region Collision Detection Methods
        public bool IsBoxColliding(Rectangle OtherBox)
        {
            if ((Collidable))
            {
                return BoundingBoxRect.Intersects(OtherBox);
            }
            else
            {
                return false;
            }
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            if ((Collidable))
            {
                if (Vector2.Distance(WorldCenter, otherCenter) < (CollisionRadius + otherRadius))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        #endregion

        public virtual void Draw(SpriteBatch spriteBatch)
        {   
                    spriteBatch.Draw(Texture, ScreenCenter, Source, tintColor, rotation, RelativeCenter, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
