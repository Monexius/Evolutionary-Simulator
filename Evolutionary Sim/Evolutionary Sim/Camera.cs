using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Animations
{
    class Camera
    {
        #region Declarations
        private static Vector2 position = Vector2.Zero;
        private static Vector2 viewPortSize = Vector2.Zero; // number of pixels to the right and below of the camera position (top left)
        private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0); // boundaries of the map
        #endregion

        #region Properties
        public static Vector2 Position // Top Left of screen is where the camera stays
        {
            get { return position; }
            set
            {
                position = new Vector2(MathHelper.Clamp(value.X, worldRectangle.X, worldRectangle.Width - ViewPortWidth), MathHelper.Clamp(value.Y, worldRectangle.Y, worldRectangle.Height - ViewPortHeight));
            } // clamp allows the camera to not go beyond the map's boundaries
        }

        public static Rectangle WorldRectangle
        {
            get { return worldRectangle; }
            set { worldRectangle = value; }
        }

        public static int ViewPortWidth // define the camera width
        {
            get { return (int)viewPortSize.X; }
            set { viewPortSize.X = value; }
        }

        public static int ViewPortHeight // define camera height
        {
            get { return (int)viewPortSize.Y; }
            set { viewPortSize.Y = value; }
        }

        public static Rectangle ViewPort // returns the camera boundaries
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, ViewPortWidth, ViewPortHeight);
            }
        }
        #endregion

        #region Public Methods
        public static void Move(Vector2 offset) // moves the camera on focused point
        {
            Position += offset;
        }

        public static bool ObjectIsVisible(Rectangle bounds) // does not draw objects that are offscreen
        {
            return (ViewPort.Intersects(bounds));
        }

        public static Vector2 Transform(Vector2 point)
        {
            return point - position;
        }

        public static Rectangle Transform(Rectangle rectangle)
        {
            return new Rectangle(rectangle.Left - (int)position.X,
                                  rectangle.Top - (int)position.Y,
                                  rectangle.Width,
                                  rectangle.Height);
        }
        #endregion
    }
}
