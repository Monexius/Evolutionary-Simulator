using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Animations
{
    class camera
    {
        #region Declarations
        private static Vector2 position = Vector2.Zero;
        private static Vector2 viewPortSize = Vector2.Zero; // number of pixels to the right and below of the camera position (top left)
        private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0); // boundaries of the map
        public Matrix transform;
        public static float zoom = 1.0f;
        public static float rotation = 0.0f;
        #endregion

        #region Properties
        //public static void CameraZoom(float zooms)
        //{
        //    zoom = zooms;
        //}
        //public static float Zoom
        //{
        //    get { return zoom; }
        //    set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; } // Stops image from flipping
        //}
        //public float Rotation
        //{
        //    get { return rotation; }
        //    set { rotation = value; }
        //}
        //public Matrix get_transformation(GraphicsDevice graphicsDevice)
        //{
        //    transform =       // Thanks to o KB o for this solution
        //      Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
        //                                 Matrix.CreateRotationZ(Rotation) *
        //                                 Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) *
        //                                 Matrix.CreateTranslation(new Vector3(ViewPortWidth * 0.5f, ViewPortHeight * 0.5f, 1));
        //    return transform;
        //}
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
        public static void ZoomAction(float x)
        {
            //Zoom += x;
        }

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
