using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Player : Entity
    {
        // Player Type = 0
        #region Members
        int slot1, slot2; // Weapon/Item Slots
                          // -1 = empty
                          // 0 = sword
                          // 1 = ???
                          // etc
        int step_timer;

        const int FRAMES_BETWEEN_STEPS = 15;
        #endregion

        #region Constructors
        public Player() : base()
        {
            Initialize(-1, -1);
        }
        public Player(Vector3 location, Vector2 center, Texture2D pic, Vector3 i_hitbox, int i_slot1, int i_slot2) : base(location, center, pic, 0, i_hitbox)
        {
            Initialize(i_slot1, i_slot2);
        }

        // Create class with these parameters:
        private void Initialize(int i_slot1, int i_slot2)
        {
            ID = 0;
            slot1 = i_slot1;
            slot2 = i_slot2;
            step_timer = 0;
            MaxSpeed = 4;
            HitboxWidth = 64;
            HitboxHeight = 64;
            AttackCooldown = 10; // defaults to 1/6 secs.
            AttackTypeID = 31; // Sword by default
            MaxHealth = 25;
            CurrentHealth = 25;
            Damage = 0;
            AttackDamage = 0;
            OriginX = HitboxWidth / 2.0f;
            OriginY = HitboxHeight / 2.0f;

            // Animation
            SpriteSheetColumns = 10;
            SpriteSheetRows = 4;
            Width = 64;
            Height = 64;
            FrameTimer = 3;             // How fast frames change when animating
        }
        #endregion

        #region Properties
        public int SlotOne
        {
            set { slot1 = value; }
            get { return slot1; }
        }

        public int SlotTwo
        {
            set { slot2 = value; }
            get { return slot2; }
        }
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

            // Play stepping sounds when moving
            if (step_timer >= 0)
            {
                step_timer--;
            }
            else
            {
                step_timer = FRAMES_BETWEEN_STEPS;
                if (Direction.X != 0 || Direction.Y != 0)
                {
                    PlayStepSound();
                }
            }

            base.Update();
        }

        private void PlayStepSound()
        {
            int step_num = Generators.Instance().NextRandom(0, 3);

            switch (step_num)
            {
                case 0: Sounds.Instance().PlaySoundEffect(101); break;
                case 1: Sounds.Instance().PlaySoundEffect(102); break;
                case 2: Sounds.Instance().PlaySoundEffect(103); break;
                case 3: Sounds.Instance().PlaySoundEffect(104); break;
            }
        }

        public override void Hurt(int damage_taken)
        {
            // Play the hurt sound
            if(Invincibility <= 0 && damage_taken > 0)
                Sounds.Instance().PlaySoundEffect(2);

            base.Hurt(damage_taken);
        }

        protected override int AnimationRow()
        {
            if (Momentum.X == 0)
            {
                if (Momentum.Y == 0)
                {
                    // Not Moving
                    return 0;
                }
                else if (Momentum.Y < 0)
                {
                    // Moving Up
                    return 1;
                }
                else
                {
                    // Moving Down
                    return 1;
                }
            }
            else
            {
                // Moving Sideways
                if(Momentum.X < 0)
                {
                    Flip = true;
                }
                else
                {
                    Flip = false;
                }

                return 1;
            }
        }
        #endregion
    }
}
