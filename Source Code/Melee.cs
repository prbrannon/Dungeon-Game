using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Melee : Attack
    {
        bool halfway;
        int lunge_distance;

        #region Constructors
        public Melee()
            : base()
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            halfway = false;
            lunge_distance = 0;
            Multihit = true;
            return;
        }
        #endregion

        public int LungeDistance
        {
            set { lunge_distance = value; }
            get { return lunge_distance; }
        }

        #region Properties

        #endregion

        #region Methods
        public override void Update()
        {
            // Get the tip of the attack to point in the correct direction
            float tip_direction = (float)Math.Atan2(Direction.Y, Direction.X);
            Angle = tip_direction;

            // Check if sword is halfway through its animation
            if (halfway == false && RemainingFrames < MaxFrames / 2)
                halfway = true;

            // Change the position of the attack
            if (halfway)
            {
                // Pulling back
                Center = new Vector3(SourceCenterX + ((float)RemainingFrames / MaxFrames) * (LungeDistance * 2 * (float)Math.Cos(Angle)),
                                     SourceCenterY + ((float)RemainingFrames / MaxFrames) * (LungeDistance * 2 * (float)Math.Sin(Angle)),
                                     0);
            }
            else
            {
                // Lunging out
                Center = new Vector3(SourceCenterX + (1 - (float)RemainingFrames / MaxFrames) * (LungeDistance * 2 * (float)Math.Cos(Angle)),
                                     SourceCenterY + (1 - (float)RemainingFrames / MaxFrames) * (LungeDistance * 2 * (float)Math.Sin(Angle)),
                                     0);
            }

            base.Update();
        }

        // Override drawing to not draw a shadow
        public override void Draw(SpriteBatch batch, Camera camera)
        {
            // Draw the actual entity over the shadow
            DrawEntity(batch, camera);
        }

        // Draw the entity in 2.5D over the shadow
        protected override void DrawEntity(SpriteBatch batch, Camera camera)
        {
            // Parameters:
            // Texture2D
            // Position - modified to draw above the shadow
            // Sprite frame position - position on the Texture2D to be drawn
            // Tint
            // Angle (radians)
            // Origin (center from 0,0 / position)
            // Scale (percentage-wise)
            // SpriteEffects - flipped or not
            // Depth order (percentage of screen from 0-1f) (sufficiently large to draw sprites in right order)
            batch.Draw(Images.Instance().GetEntityTexture(this.ID),
                       new Vector2(this.Position.X - camera.X, this.Position.Y - camera.Y),
                       new Rectangle(0, 0, Images.Instance().GetEntityTexture(this.ID).Width, Images.Instance().GetEntityTexture(this.ID).Height),
                       Color.White,
                       Angle,
                       Origin,
                       Scale,
                       SpriteEffects.None,
                       Center.Y / 100000);
        }
        #endregion
    }
}
