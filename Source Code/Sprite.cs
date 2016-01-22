using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    abstract class Sprite
    {
        #region members
        Vector3 position;   // top-left corner
        float scale;        // magnification
        Vector2 origin;     // center of rotation
        Vector3 size;       // picture dimensions
        float angle;        // angle in radians
        int ID_num;         // used for picking the correct texture

        // Animation Details
        int frames_x, frames_y;             // Dimensions of the sprite sheet
        int frame_count_x, frame_count_y;   // The current frame to draw
        float frame_width_x, frame_width_y; // The dimensions of one frame on the sprite sheet in pixels
        int frame_timer;                    // Tells the sprite how often to change frame
        int current_frame_timer;            // Current count for changing frames
        #endregion

        #region constructors
        public Sprite()
        {
            Initialize(new Vector3(0.0f, 0.0f, 0.0f), 1.0f,  new Vector2(0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 0);
        }
        public Sprite(Vector3 location, Vector2 center, Texture2D pic)
        {
            Initialize(location, 1.0f, center, new Vector3(pic.Width, pic.Height, 0.0f), 0);
        }

        // Create class with these parameters:
        private void Initialize(Vector3 i_position, float i_scale, Vector2 i_origin, Vector3 i_size, float i_angle)
        {
            position = i_position;
            scale = i_scale;
            origin = i_origin;
            size = i_size;
            angle = i_angle;

            // Animation
            frames_x = 0;
            frames_y = 0;
            frame_count_x = 0;
            frame_count_y = 0;
            frame_width_x = 0;
            frame_width_y = 0;
            frame_timer = 1;
            current_frame_timer = 0;
        }
        #endregion

        #region properties
        public Vector3 Position
        {
            set { position = value; }
            get { return position; }
        }

        public float Scale
        {
            set { scale = value; }
            get { return scale; }
        }

        public Vector2 Origin
        {
            set { origin = value; }
            get { return origin; }
        }

        public float OriginX
        {
            set { origin.X = value; }
            get { return origin.X; }
        }

        public float OriginY
        {
            set { origin.Y = value; }
            get { return origin.Y; }
        }

        public float Width
        {
            set { size.X = value; }
            get { return size.X; }
        }

        public float Height
        {
            set { size.Y = value; }
            get { return size.Y; }
        }

        public float Angle
        {
            set { angle = value; }
            get { return angle; }
        }

        public int ID
        {
            set { ID_num = value; }
            get { return ID_num; }
        }

        public virtual Vector3 Center
        {
            set { position = new Vector3(value.X - position.X * scale, value.Y - position.Y * scale, value.Z); }
            get { return position + size / 2; }
        }

        public virtual float CenterX
        {
            set { position = new Vector3(value, position.Y, position.Z); }
            get { return position.X + size.X / 2; }
        }

        public virtual float CenterY
        {
            set { position = new Vector3(position.X, value, position.Z); }
            get { return position.Y + size.Y / 2; }
        }

        #region Animation

        public int SpriteSheetColumns
        {
            set { frames_x = value; }
            get { return frames_x; }
        }

        public int SpriteSheetRows
        {
            set { frames_y = value; }
            get { return frames_y; }
        }

        public int FrameCounterX
        {
            set { frame_count_x = value; }
            get { return frame_count_x; }
        }

        public int FrameCounterY
        {
            set { frame_count_y = value; }
            get { return frame_count_y; }
        }

        public int FrameTimer
        {
            set { frame_timer = value; }
            get { return frame_timer; }
        }

        public int CurrentFrameTimer
        {
            set { current_frame_timer = value; }
            get { return current_frame_timer; }
        }

        #endregion

        #endregion

        #region Methods

        // Update the status of the Sprite
        public virtual void Update()
        {
            current_frame_timer++;
            if (current_frame_timer >= frame_timer)
            {
                current_frame_timer = 0;
                FrameCounterX = (FrameCounterX + 1) % SpriteSheetColumns;
            }
        }

        // Draw the Sprite to the SpriteBatch given a Camera translation
        public virtual void Draw(SpriteBatch batch, Camera camera)
        {
            // Draw function
            // Parameters:
            // Texture2D
            // Position
            // Sprite frame position - position on the Texture2D to be drawn
            // Tint
            // Angle (radians)
            // Origin (center from 0,0 / position)
            // Scale (percentage-wise)
            // SpriteEffects - flipped or not
            // Depth order (percentage of screen from 0-1f) (sufficiently large to draw sprites in right order)
            batch.Draw(Images.Instance().GetEntityTexture(this.ID),
                       new Vector2(this.Position.X - camera.X, this.Position.Y - camera.Y),
                       new Rectangle(FrameCounterX * (int)Width, FrameCounterY * (int)Height, (int)Width, (int)Height),
                       Color.White,
                       this.angle,
                       this.origin,
                       this.scale,
                       SpriteEffects.None,
                       Center.Y / 100000);
        }
        #endregion
    }
}
