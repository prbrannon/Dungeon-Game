using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Chaser : Enemy
    {
        #region Constructors
        public Chaser() : base()
        {
            Initialize();
        }

        public Chaser(Vector3 location, Vector2 center, Texture2D pic, Vector3 i_hitbox) : base(location, center, pic, i_hitbox)
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            ID = 1;
            MaxSpeed = 1;
            SightRange = 250;
            HitboxWidth = 64;
            HitboxHeight = 64;
            AttackCooldown = 1;
            AttackTypeID = -1;
            MaxHealth = 4;
            CurrentHealth = 4;
            Damage = 2;
            AttackDamage = 0;
            OriginX = HitboxWidth / 2.0f;
            OriginY = HitboxHeight / 2.0f;

            // Animation
            SpriteSheetColumns = 4;
            SpriteSheetRows = 4;
            Width = 64;
            Height = 64;
            FrameTimer = 12;
            FrameCounterY = 0;
        }
        #endregion

        #region Properties
        
        #endregion

        #region Methods
        public override void Update()
        {
            if (FrameCounterY != AnimationRow())
            {
                FrameCounterY = AnimationRow();
                FrameCounterX = 0;
                Flip = false;
            }

            base.Update();

            // Get the distance between this and the target
            Vector2 distance;

            if (Active == true)
            {
                distance = new Vector2(Target.X - this.CenterX, Target.Y - this.CenterY);
            }
            else
            {
                distance = new Vector2(0, 0);
            }
            if (distance.X != 0 || distance.Y != 0) // normalizing a 0,0 vector2 results in NaN
                distance.Normalize();
            this.Direction = distance;
        }

        protected override int AnimationRow()
        {
            if (Active == false)
                return 0;

            double angle = Math.Atan2((double)Momentum.Y, (double)Momentum.X);
            
            // Down
            if (angle >= Math.PI * 0.25 && angle < Math.PI * 0.75)
            {
                return 3;
            }
            // Up
            else if(angle < Math.PI * -0.25 && angle >= Math.PI * -0.75)
            {
                return 2;
            }

            // Left
            else if (angle >= Math.PI * 0.75 || angle < Math.PI * -0.75)
            {
                Flip = true;
                return 1;
            }
            // Right
            else
            {
                Flip = false;
                return 1;
            }
                
        }
        #endregion
    }
}
