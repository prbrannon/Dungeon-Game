using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Tank : Enemy
    {
        bool stunned;
        bool charging;
        bool waiting;
        int charge_cooldown;
        int stunned_cooldown;
        int time_waited;
        const int MAX_COOLDOWN = 240;
        const int STUN_COOLDOWN = 240;
        const int NORMAL_SPEED = 1;
        const int CHARGE_SPEED = 7;
        const int VERTICAL_ATTACK_RANGE = 64;
        const int CHARGE_DAMAGE = 8;
        const int CHARGE_WAIT_TIME = 60;


        #region Constructors
        public Tank()
            : base()
        {
            Initialize();
        }

        public Tank(Vector3 location, Vector2 center, Texture2D pic, Vector3 i_hitbox)
            : base(location, center, pic, i_hitbox)
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            ID = 10;
            MaxSpeed = NORMAL_SPEED;
            SightRange = 256;
            HitboxWidth = 128;
            HitboxHeight = 128;
            AttackCooldown = 1;
            AttackTypeID = -1;
            MaxHealth = 69;
            CurrentHealth = 69;
            Damage = 0;
            AttackDamage = 0;
            OriginX = HitboxWidth / 2.0f;
            OriginY = HitboxHeight / 2.0f;

            // Animation
            SpriteSheetColumns = 4;
            SpriteSheetRows = 2;
            Width = 128;
            Height = 128;
            FrameTimer = 12;
            FrameCounterY = 0;

            charge_cooldown = MAX_COOLDOWN;
            stunned_cooldown = 0;
            time_waited = 0;
            charging = false;
            stunned = false;
            waiting = false;
        }
        #endregion
        public override float Radius
        {
            get { return 32; }
        }

        

        #region Methods
        public override void Update()
        {
            Vector2 distance;

            if (FrameCounterY != AnimationRow())
            {
                FrameCounterY = AnimationRow();
                FrameCounterX = 0;
            }

            // Progress the frame counter
            UpdateSpriteBase();

            // Do entity stuff
            base.UpdateCooldowns();

            // Update stun and charge cooldowns for the tank
            UpdateCooldowns();
            
            // Able to charge
            if (charge_cooldown <= 0)
            {
                // Done waiting
                if (time_waited >= CHARGE_WAIT_TIME)
                {
                    Sounds.Instance().PlaySoundEffect(11);
                    time_waited = 0;
                    waiting = false;
                    charge_cooldown = MAX_COOLDOWN;
                    charging = true;
                    Damage = CHARGE_DAMAGE;
                }
                else
                {
                    // Is target in close enough??? WHO KNOWS?!?!
                    if (TargetInRange())
                    {
                        if(waiting == false)
                            Sounds.Instance().PlaySoundEffect(10);

                        waiting = true; // Start waiting and prepare to charge
                    }
                }
            }
            
            // Move in certain ways if stunned or not
            if (stunned == false)
            {
                // Currently in a charge
                if (charging)
                {
                    // Don't change direction and GO FAST
                    distance = Direction;
                    MaxSpeed = CHARGE_SPEED;
                }

                // Moving around normally
                else
                {
                    distance = new Vector2(Target.X - this.CenterX, Target.Y - this.CenterY);

                    if (waiting)
                        MaxSpeed = 0;
                    else
                        MaxSpeed = NORMAL_SPEED;
                }
            }
            else
            {
                if (stunned_cooldown <= 0)
                {
                    stunned = false;
                }
                distance = new Vector2(Target.X - this.CenterX, Target.Y - this.CenterY);
                MaxSpeed = 0;
            }

            // Get the distance between this and the target
            if (distance.X != 0 || distance.Y != 0) // normalizing a 0,0 vector2 results in NaN
                distance.Normalize();
            this.Direction = distance;

            UpdatePosition();
        }

        private bool TargetInRange()
        {
            double distance = Math.Sqrt(Math.Pow(Target.X - CenterX, 2) + Math.Pow(Target.Y - CenterY, 2));

            // In range
            if (distance <= SightRange)
            {
                return true;
            }
            // Not in range
            else
            {
                return false;
            }
        }

        public override void UpdateCooldowns()
        {
            if (charge_cooldown >= 0 && stunned == false)
                charge_cooldown--;

            if (stunned_cooldown >= 0)
                stunned_cooldown--;

            if (waiting)
                time_waited++;
        }

        public override void BossConditionHit()
        {
            if (charging)
            {
                charging = false;
                stunned = true;
                stunned_cooldown = STUN_COOLDOWN;
                Damage = 0;
            }
        }

        protected override int AnimationRow()
        {
            if (stunned)
                return 1;
            else
            {
                double angle = Math.Atan2((double)Momentum.Y, (double)Momentum.X);

                // Right
                if (angle < Math.PI * 0.5 && angle > Math.PI * -0.5)
                {
                    Flip = false;
                }
                // Left
                else
                {
                    Flip = true;
                }

                return 0;
            }
        }

        public override void Hurt(int damage_taken)
        {
            if (stunned && Invincibility <= 0)
            {
                CurrentHealth -= damage_taken;
                if (damage_taken > 0)
                {
                    Invincibility = 15;
                }
            }
        }
        #endregion
    }
}
