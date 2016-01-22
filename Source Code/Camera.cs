using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Camera
    {
        #region Members

        const float SPEED = 5f;

        Vector2 position;
        Vector2 target_center;
        int screen_width, screen_height;
        Point room_center;
        float scale;
        Vector2 direction;

        #endregion

        #region Constructors
        public Camera()
        {
            Initialize();
        }

        private void Initialize()
        {
            position = new Vector2(0, 0);
            room_center = new Point();
            scale = 1.0f;
        }

        #endregion

        #region Properties
        public Vector2 Position
        {
            set { position = value; }
            get { return position; }
        }

        public Vector2 Center
        {
            set { position = new Vector2(value.X - screen_width / 2, value.Y - screen_height / 2); }
            get { return new Vector2(position.X + screen_width / 2, position.Y + screen_height / 2); }
        }

        public Vector2 TargetCenter
        {
            set { target_center = value; }
            get { return target_center; }
        }

        public int ScreenWidth
        {
            set { screen_width = value; }
            get { return screen_width; }
        }

        public int ScreenHeight
        {
            set { screen_height = value; }
            get { return screen_height; }
        }

        public Point RoomCenter
        {
            set { room_center = value; }
            get { return room_center; }
        }

        public float Scale
        {
            set { scale = value; }
            get { return scale; }
        }

        public Vector2 Direction
        {
            set { direction = value; }
            get { return direction; }
        }

        public float X
        {
            set { position.X = value; }
            get { return position.X; }
        }

        public float Y
        {
            set { position.Y = value; }
            get { return position.Y; }
        }
        #endregion

        #region Members

        public void Update()
        {
            /*
            // Where the camera should attempt to reach
            Vector2 target_position = target_center - new Vector2(screen_width / 2, screen_height / 2);

            position = target_position;
            
             * */

            Center = new Vector2((room_center.X + target_center.X) / 2, (target_center.Y + room_center.Y) / 2);

            /*

            // TODO - make the camera more better and stuff - it's too slow right now
            
            // Is the camera close enough to the target?
            if ((target_center.X - (SPEED + 1) > Center.X && target_center.X + (SPEED + 1) < Center.X) && ((target_center.Y - (SPEED + 1) > Center.Y && target_center.Y + (SPEED + 1) < Center.Y)))
            {
                Center = target_center;
            }
            else
            {
                // Calculate the movement angle
                double angle = Math.Atan2((double)(target_center.Y - Center.Y), (double)(target_center.X - Center.X));

                // Move the camera
                position.X += (float)Math.Cos(angle) * SPEED;
                position.Y += (float)Math.Sin(angle) * SPEED;
            }
             * */
        }

        #endregion
    }
}
