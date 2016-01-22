using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Members
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ObjectManager ObjManager;

        KeyboardState old_keyboard_state;
        GamePadState old_gamepad_state;

        int controller_type;


        // Debugging:
        SpriteFont Font1;

        #endregion

        #region Constructors
        public Game1()
        {
            // TODO: Use to change resolution: http://msdn.microsoft.com/en-us/library/bb447674.aspx
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            //Changes the settings that were just applied
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Sounds.Instance().PlayMusic(0, true);
            controller_type = 0; // No controller set
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Debugging:
            Font1 = Content.Load<SpriteFont>("Arial");

            Images.Instance().Load_Textures(Content); // Load all the textures for the game using the static Images class
            Sounds.Instance().Load_Sounds(Content);   // Load all sounds for the game using the static Sounds class
            Levels.Instance().Load_Rooms();          // Read from the levels file and make a list of available rooms

            ObjManager = new ObjectManager(SCREEN_WIDTH, SCREEN_HEIGHT, graphics.GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Methods
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (ObjManager.quit == true)
            {
                this.Exit();
            }

            KeyboardState new_keyboard_state = Keyboard.GetState();
            GamePadState new_gamepad_state = GamePad.GetState(PlayerIndex.One);

            #region SetControllerType
            if (controller_type == 0)
            {
                if (new_keyboard_state.IsKeyDown(Keys.Enter) && old_keyboard_state.IsKeyUp(Keys.Enter))
                {
                    SetControllerType(1);
                }
                if (new_gamepad_state.IsButtonDown(Buttons.A) && old_gamepad_state.IsButtonUp(Buttons.A))
                {
                    SetControllerType(2);
                }
            }

            #endregion

            // Update the game
            else if (controller_type == 1)
            {
                #region KeyBoard
                if (new_keyboard_state.IsKeyDown(Keys.Enter) && old_keyboard_state.IsKeyUp(Keys.Enter))
                {
                    ObjManager.Enter();
                }

                #region Movement
                // Left and Right player movement
                if (new_keyboard_state.IsKeyDown(Keys.A))
                {
                    ObjManager.SetHorizontalMovementDirection(-1, 0);
                }
                else if (new_keyboard_state.IsKeyDown(Keys.D))
                {
                    ObjManager.SetHorizontalMovementDirection(1, 0);
                }
                else
                {
                    ObjManager.SetHorizontalMovementDirection(0, 0);
                }

                // Up and Down player movement
                if (new_keyboard_state.IsKeyDown(Keys.W))
                {
                    ObjManager.SetVerticalMovementDirection(-1, 0);
                }
                else if (new_keyboard_state.IsKeyDown(Keys.S))
                {
                    ObjManager.SetVerticalMovementDirection(1, 0);
                }
                else
                {
                    ObjManager.SetVerticalMovementDirection(0, 0);
                }
                #endregion

                #region Attacking
                // Left and Right player attacking
                if (new_keyboard_state.IsKeyDown(Keys.Left))
                {
                    ObjManager.SetHorizontalAttackingDirection(-1, 0);
                }
                else if (new_keyboard_state.IsKeyDown(Keys.Right))
                {
                    ObjManager.SetHorizontalAttackingDirection(1, 0);
                }
                else
                {
                    ObjManager.SetHorizontalAttackingDirection(0, 0);
                }

                // Up and Down player attacking
                if (new_keyboard_state.IsKeyDown(Keys.Up))
                {
                    ObjManager.SetVerticalAttackingDirection(-1, 0);
                }
                else if (new_keyboard_state.IsKeyDown(Keys.Down))
                {
                    ObjManager.SetVerticalAttackingDirection(1, 0);
                }
                else
                {
                    ObjManager.SetVerticalAttackingDirection(0, 0);
                }

                #endregion

                #endregion
            }
            else if (controller_type == 2)
            {
                #region GamePad
                if (new_gamepad_state.IsButtonDown(Buttons.A) && old_gamepad_state.IsButtonUp(Buttons.A))
                {
                    ObjManager.Enter();
                }

                #region Movement
                ObjManager.SetHorizontalMovementDirection(new_gamepad_state.ThumbSticks.Left.X, 0);
                ObjManager.SetVerticalMovementDirection(new_gamepad_state.ThumbSticks.Left.Y * -1, 0);

                #endregion

                #region Attacking
                ObjManager.SetHorizontalAttackingDirection(new_gamepad_state.ThumbSticks.Right.X, 0);
                ObjManager.SetVerticalAttackingDirection(new_gamepad_state.ThumbSticks.Right.Y * -1, 0);

                #endregion

                #endregion
            }

            #region Debug
            if (new_keyboard_state.IsKeyDown(Keys.R))
            {
                ResetFloor();
            }
            if (new_keyboard_state.IsKeyDown(Keys.M))
            {
                ObjManager.RevealEntireMap();
            }
            #endregion
            
            old_keyboard_state = new_keyboard_state;
            old_gamepad_state = new_gamepad_state;

            // Update all objects in the game
            ObjManager.Update();

            base.Update(gameTime);
        }

        private void ResetFloor()
        {
            ObjManager.ResetFloor();
        }

        private void SetControllerType(int newType)
        {
            controller_type = newType;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Controller not yet set
            if (controller_type == 0)
            {
                DrawControllerSelect(spriteBatch, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2));
            }
            else
            {
                ObjManager.Draw(spriteBatch);
            }

            // Debugging: 

            spriteBatch.Begin();

            //DrawText("Room index: " + ObjManager.current_floor.CurrentRoomPosition.ToString(), new Vector2(50, 500));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawControllerSelect(SpriteBatch batch, Vector2 center)
        {
            batch.Begin();

            batch.Draw(Images.Instance().GetMenuTexture(700),
                       center - new Vector2(Images.Instance().GetMenuTexture(700).Width / 2, Images.Instance().GetMenuTexture(700).Height / 2),
                       Color.White);

            batch.End();
        }

        private void DrawText(string text, Vector2 pos)
        {
            spriteBatch.DrawString(Font1, text, pos, Color.Green);
        }

        #endregion
    }
}
