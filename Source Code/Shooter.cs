using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    // Enemy that doesn't move and acts like a turrent when the player is in range
    // Does not shoot when the player is out of range
    class Shooter : Enemy
    {
        #region Constructors
        public Shooter() : base()
        {
            Initialize();
        }

        public Shooter(Vector3 location, Vector2 center, Texture2D pic, Vector3 i_hitbox) : base(location, center, pic, i_hitbox)
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            ID = 2;
            MaxSpeed = 0;
            SightRange = 250;
            HitboxWidth = 64;
            HitboxHeight = 64;
            AttackCooldown = 60;
            AttackTypeID = 21;
            MaxHealth = 3;
            CurrentHealth = 3;
            Damage = 0;
            AttackDamage = 2;
            OriginX = HitboxWidth / 2.0f;
            OriginY = HitboxHeight / 2.0f;

            // Animation
            SpriteSheetColumns = 1;
            SpriteSheetRows = 8;
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
            base.Update();

            // Get the distance between this and the target
            Vector2 distance = new Vector2(Target.X - this.CenterX, Target.Y - this.CenterY);

            // If the target is within range and the current direction, attempt to attack
            if (distance.Length() <= SightRange && Active)
            {
                FrameCounterY = AnimationRow();
                AttemptAttack();
            }

            // Spin around when idle one time per animation cycle
            else if(CurrentFrameTimer == FrameTimer - 1)
            {
                FrameCounterY = (FrameCounterY + 1) % SpriteSheetRows;
            }

            // Set the direction of Shooter to be towards the target.
            // Doesn't need to be normalized because MaxSpeed = 0
            this.Direction = distance;
        }

        protected override int AnimationRow()
        {
            // Angle should fall into 1 of 8 catagories
            double angle = Math.Atan2(Target.Y - this.CenterY, Target.X - this.CenterX);
            
            
            // Down Left
            if (angle >= Math.PI * 0.625 && angle < Math.PI * 0.875)
            {
                return 0;
            }
            // Down
            else if (angle >= Math.PI * 0.375 && angle < Math.PI * 0.625)
            {
                return 1;
            }
            // Down Right
            else if (angle >= Math.PI * 0.125 && angle < Math.PI * 0.375)
            {
                return 2;
            }
            // Right
            else if (angle >= Math.PI * -0.125 && angle < Math.PI * 0.125)
            {
                return 3;
            }
            // Up Right
            else if (angle >= Math.PI * -0.375 && angle < Math.PI * -0.125)
            {
                return 4;
            }
            // Up
            else if (angle >= Math.PI * -0.625 && angle < Math.PI * -0.375)
            {
                return 5;
            }
            // Up Left
            else if (angle >= Math.PI * -0.875 && angle < Math.PI * -0.625)
            {
                return 6;
            }
            // Left
            else
            {
                return 7;
            }
        }

        // Attack with an arrow
        // Pre: none
        // Post: CurrentAttackCooldown is set to the AttackCooldown. A new entity is created for the attack
        protected override bool AttemptAttack()
        {
            if (Target.X == 0 && Target.Y == 0)
                return false;
            return base.AttemptAttack();
        }
        #endregion
    }
}
