using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    abstract class Entity : Sprite
    {
        #region Members
        const int INVIBILITY_FRAMES = 30;       // The number of frames after getting hit before the entity can get hit again
        const int HEALTH_BAR_ABOVE = 50;         // Pixels above the entity
        const int HEALTH_BOX_HEIGHT = 15;        // Dimensions of hp bar

        int max_health;             // HP of the thing
        int current_health;         // HP of the thing
        int attack_damage;          
        int collision_damage;       // damage dealt to other entity when collision occurs
        int type;                   // type of the character. used by the object manager for logic
        int max_speed;              // units moved per frame
        Vector2 attacking_direction;// Between -1 and 1 to determine angle of attack
        Vector2 desired_direction;  // Between -1 and 1 to determine angle of travel
        Vector2 momentum;           // Stores the entity's velocity percentages
        Vector3 hitbox;             // x and y of hitbox
        int attack_cooldown;        // The base attack cooldown after an entity attacks
        int current_attack_cooldown;// frames until the entity can attack. 0 means it can attack
        int attack_type_ID;         // Attack type ID for the entity. -1 by default
        bool attacking;             // Tells the ObjectManager to create an attack
        int unique_ID;              // The unique ID for the entity. Used for collision detection with attacks
        bool flip;                  // True if the entity is facing left. False if facing right.
        int invincible_cooldown;
        #endregion

        #region Constructors
        public Entity() : base()
        {
            Initialize(-1, new Vector3(0.0f, 0.0f, 0.0f));
        }
        public Entity(Vector3 location, Vector2 center, Texture2D pic, int i_type, Vector3 i_hitbox) : base(location, center, pic)
        {
            Initialize(i_type, i_hitbox);
        }

        // Create class with these parameters:
        private void Initialize(int i_type, Vector3 i_hitbox)
        {
            max_health = 1;
            current_health = 1;
            collision_damage = 0;
            type = i_type;
            max_speed = 0;
            attacking_direction = new Vector2(0.0f, 0.0f);
            desired_direction = new Vector2(0.0f, 0.0f);
            hitbox = i_hitbox;
            attack_cooldown = 1;
            attack_type_ID = -1;
            attacking = false;
            unique_ID = Generators.Instance().NextUniqueID();       // Give a unique ID to the entity
            flip = false;
            invincible_cooldown = 0;
            attack_damage = 0;
        }
        #endregion

        #region Properties
        public int Invincibility
        {
            set { invincible_cooldown = value; }
            get { return invincible_cooldown; }
        }

        public int MaxHealth
        {
            set { max_health = value; }
            get { return max_health; }
        }

        public int CurrentHealth
        {
            set { current_health = value; }
            get { return current_health; }
        }

        public int Damage
        {
            set { collision_damage = value; }
            get { return collision_damage; }
        }

        public int AttackDamage
        {
            set { attack_damage = value; }
            get { return attack_damage; }
        }

        public int Type
        {
            set { type = value; }
            get { return type; }
        }

        public int MaxSpeed
        {
            set { max_speed = value; }
            get { return max_speed; }
        }

        public Vector2 AttackDirection
        {
            set { attacking_direction = value; }
            get { return attacking_direction; }
        }

        public float AttackDirectionX
        {
            set { attacking_direction.X = value; }
            get { return attacking_direction.X; }
        }

        public float AttackDirectionY
        {
            set { attacking_direction.Y = value; }
            get { return attacking_direction.Y; }
        }

        public float HitboxWidth
        {
            set { hitbox.X = value; }
            get { return hitbox.X; }
        }

        public float HitboxHeight
        {
            set { hitbox.Y = value; }
            get { return hitbox.Y; }
        }

        public virtual float HitboxLeft
        {
            set { Position = new Vector3(value, this.Position.Y, this.Position.Z); }
            get { return base.Position.X; }
        }

        public virtual float HitboxRight
        {
            set { Position = new Vector3(value - this.HitboxWidth, this.Position.Y, this.Position.Z); }
            get { return base.Position.X + hitbox.X; }
        }

        public virtual float HitboxTop
        {
            set { Position = new Vector3(this.Position.X, value, this.Position.Z); }
            get { return base.Position.Y; }
        }

        public virtual float HitboxBottom
        {
            set { Position = new Vector3(this.Position.X, value - this.Width, this.Position.Z); }
            get { return base.Position.Y + hitbox.Y; }
        }

        public Vector3 TopLeftCorner
        {
            set { Position = value; }
            get { return Position; }
        }

        public Vector3 BottomLeftCorner
        {
            set { Position = new Vector3(value.X, value.Y - Height, value.Z); }
            get { return new Vector3(Position.X, Position.Y + Height - 1, 0); }
        }

        public Vector3 BottomRightCorner
        {
            set { Position = new Vector3(value.X - Width, value.Y - Height, value.Z); }
            get { return new Vector3(Position.X + Width, Position.Y + Height - 1, 0); }
        }

        public Vector3 TopRightCorner
        {
            set { Position = new Vector3(value.X - Width, value.Y, value.Z); }
            get { return new Vector3(Position.X + Width - 1, Position.Y, 0); }
        }

        public override Vector3 Center
        {
            set { Position = new Vector3(value.X - this.Width / 2, value.Y - this.Width / 2, value.Z); }
            get { return new Vector3(this.Position.X + HitboxWidth / 2, this.Position.Y + HitboxHeight / 2, this.Position.Z); }
        }

        public override float CenterX
        {
            set { Position = new Vector3(value, Position.Y, Position.Z); }
            get { return Position.X + HitboxWidth / 2; }
        }

        public override float CenterY
        {
            set { Position = new Vector3(Position.X, value, Position.Z); }
            get { return Position.Y + HitboxHeight / 2; }
        }

        // Generate a fake radius when the entity's hitbox is a rectangle
        public virtual float Radius
        {
            get { return (Height + Width) / 4; }
        }

        // Used for collision detection
        public Rectangle CollisionBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, (int)HitboxWidth, (int)HitboxHeight); }
        }

        public BoundingBox BoundBox
        {
            get { return new BoundingBox(Position, new Vector3(Position.X + this.HitboxWidth, Position.Y + this.HitboxHeight, 0)); }
        }

        public BoundingSphere BoundSphere
        {
            get { return new BoundingSphere(this.Center, this.Radius); }
        }

        public Vector2 Direction
        {
            set { desired_direction = value; }
            get { return desired_direction; }
        }

        public float DirectionX
        {
            set { desired_direction.X = value; }
            get { return desired_direction.X; }
        }

        public float DirectionY
        {
            set { desired_direction.Y = value; }
            get { return desired_direction.Y; }
        }

        public Vector2 Momentum
        {
            set { momentum = value; }
            get { return momentum; }
        }

        public int AttackCooldown
        {
            set { attack_cooldown = value; }
            get { return attack_cooldown; }
        }

        public int CurrentAttackCooldown
        {
            set { current_attack_cooldown = value; }
            get { return current_attack_cooldown; }
        }

        public int AttackTypeID
        {
            set { attack_type_ID = value; }
            get { return attack_type_ID; }
        }

        public bool Attacking
        {
            set { attacking = value; }
            get { return attacking; }
        }

        public int UniqueID
        {
            get { return unique_ID; }
        }

        public bool Flip
        {
            set { flip = value; }
            get { return flip; }
        }

        #endregion

        #region Methods

        // Updates the status of the entity.
        // Post: Reduces the attack cooldown if the entity recently attacked.
        //       Moves the entity towards its target based on its speed
        public override void Update()
        {
            UpdateSpriteBase();

            UpdateCooldowns();

            UpdatePosition();
        }

        public virtual void UpdateSpriteBase()
        {
            base.Update();
        }

        public virtual void UpdateCooldowns()
        {
            // Lower attack cooldown
            if (current_attack_cooldown > 0)
                current_attack_cooldown--;

            // Lower invincibity cooldown
            if (invincible_cooldown > 0)
                invincible_cooldown--;
        }

        public virtual void UpdatePosition()
        {
            momentum = desired_direction;
            if (momentum.X != 0 || momentum.Y != 0) // normalizing a 0,0 vector2 results in NaN
                momentum.Normalize();
            Vector2 movement = max_speed * momentum;
            Position += new Vector3(movement.X, movement.Y, 0);
        }

        // The attack type ID is returned if the entity can attack.
        // The entity can only attack when it's cooldown is complete
        // Pre:  Must be able to attack (cooldown is less than 0)
        // Post: If the entity can't attack or its attack type has not been set, nothing happens
        public virtual bool AttemptAttack()
        {
            // Can attack
            if (current_attack_cooldown <= 0)
            {
                attacking = true;
                current_attack_cooldown = attack_cooldown;
                return true;
            }
            // Can't attack
            else
            {
                return false;
            }
        }

        // Hurt this entity with the given damage
        public virtual void Hurt(int damage_taken)
        {
            if (invincible_cooldown <= 0)
            {
                current_health -= damage_taken;
                if (damage_taken > 0)
                {
                    invincible_cooldown = INVIBILITY_FRAMES;
                    Sounds.Instance().PlaySoundEffect(3);
                }
            }
        }

        // Check if the entity is dead
        public virtual bool Is_Dead()
        {
            if (current_health <= 0)
                return true;
            return false;
        }

        public bool Collided_With(Entity other)
        {
            return false;
        }

        public bool Can_Take_Damage_From(Attack other)
        {
            if (this.UniqueID == other.SourceID)
                return false;
            return true;
        }

        public virtual void BossConditionHit()
        {
            return;
        }

        // Draw the Entity to the SpriteBatch given a Camera translation
        public override void Draw(SpriteBatch batch, Camera camera)
        {
            // Draw the shadow for the entity
            //DrawShadow(batch, camera);

            // Draw the actual entity over the shadow
            DrawEntity(batch, camera);
        }

        // Draw the shadow for this entity on the given SpriteBatch given a camera translation
        protected virtual void DrawShadow(SpriteBatch batch, Camera camera)
        {
            // Parameters:
            // Texture2D
            // Destination - stretches texture to fit the rectangle
            // Sprite frame position - position on the Texture2D to be drawn
            // Tint - some transparency
            // Angle (radians)
            // Origin (center from 0,0 / position)
            // SpriteEffects - flipped or not
            // Depth order (percentage of screen from 0-1f)
            batch.Draw(Images.Instance().GetEntityTexture(100),
                       new Rectangle((int)(Position.X - camera.X), (int)(Position.Y - camera.Y), (int)this.HitboxWidth, (int)this.HitboxHeight),
                       new Rectangle(0, 0, Images.Instance().GetEntityTexture(100).Width, Images.Instance().GetEntityTexture(100).Height),
                       Color.White * 0.5f,
                       Angle,
                       Origin,
                       SpriteEffects.None,
                       0);
        }

        // Draw the entity in 2.5D over the shadow
        protected virtual void DrawEntity(SpriteBatch batch, Camera camera)
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

            if (flip == true)
            {
                batch.Draw(Images.Instance().GetEntityTexture(this.ID),
                       new Vector2(Position.X - camera.X, Position.Y - camera.Y),
                       new Rectangle(FrameCounterX * (int)Width, FrameCounterY * (int)Height, (int)Width, (int)Height),
                       Color.White,
                       Angle,
                       Origin,
                       Scale,
                       SpriteEffects.FlipHorizontally,
                       Center.Y / 100000);
            }
            else
            {
                batch.Draw(Images.Instance().GetEntityTexture(this.ID),
                       new Rectangle((int)(Position.X - camera.X), (int)(Position.Y - camera.Y), (int)Width, (int)Height),
                       new Rectangle(FrameCounterX * (int)Width, FrameCounterY * (int)Height, (int)Width, (int)Height),
                       Color.White,
                       Angle,
                       Origin,
                       SpriteEffects.None,
                       Center.Y / 100000);
            }
        }

        public void DrawHealthBar(SpriteBatch batch, Camera camera)
        {
            batch.Draw(Images.Instance().GetHUDTexture(12),
                       new Rectangle((int)(Position.X - camera.X - HitboxWidth / 2), (int)(Position.Y - camera.Y) - HEALTH_BAR_ABOVE, (int)(HitboxWidth), HEALTH_BOX_HEIGHT),
                       Color.Red);

            batch.Draw(Images.Instance().GetHUDTexture(10),
                       new Rectangle((int)(Position.X - camera.X - HitboxWidth / 2), (int)(Position.Y - camera.Y) - HEALTH_BAR_ABOVE, (int)(((float)current_health / max_health) * HitboxWidth), HEALTH_BOX_HEIGHT),
                       Color.Red);
        }

        protected virtual int AnimationRow()
        {
            return 0;
        }

        #endregion
    }
}
