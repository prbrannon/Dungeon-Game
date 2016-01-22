using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class ObjectManager
    {
        #region Constants
        const int TILESIZE = 64;
        const int ROOMSIZEX = 640;
        const int ROOMSIZEY = 640;

        const int WARRIOR_LOC_X = 0;
        const int RANGER_LOC_X = 100;
        const int MAGE_LOC_X = 200;
        const int WARRIOR_LOC_Y = 0;
        const int RANGER_LOC_Y = 100;
        const int MAGE_LOC_Y = 200;
        const int MAIN_MENU_LENGTH = 2;             // The number of options on the main menu
        const int CHARACTER_MENU_LENGTH = 3;        // The number of selectable characters
        const int MAIN_MENU_TITLE_Y = 100;          // Height of the title on the main menu
        const int MAIN_MENU_BUTTONS_Y = 400;        // Height of the button
        const int CHARACTER_MENU_TITLE_Y = 100;     // Height of the title on the character select menu
        const int CHARACTER_PICTURE_Y = 300;        // Height of the characters on the character select menu
        const int GAME_OVER_HEIGHT = 100;
        const int LEVEL_COMPLETE_HEIGHT = 100;
        const int DEFAULT_MENU_DELAY = 30;          // Frames before another button may be selected
        const int ROOMS_PER_LEVEL = 10;

        const int DAMAGE_BOUNCE = 50;               // pixels moved when damage

        // HUD drawing
        const int FULL_STATUS_BAR_WIDTH = 246;      // Pixels inside of the HP and STAMINA overlay
        const int OVERLAY_WIDTH = 5;                // width of the bar on the overlay
        const int OVERLAY_HEIGHT = 12;              // height of the bar and HP+STAMINA text on the overlay
        const int HP_STAMINA_GAP = 10;              // pixels between the bars
        #endregion

        #region Members
        List<Player> players;
        List<Enemy> enemies;
        List<Attack> attacks;
        List<Player> character_classes;
        public Floor current_floor;
        Camera camera;
        int screen_size_x, screen_size_y;
        GraphicsDevice graphics_device;

        // Game modes:
        // 0 - Main menu
        // 1 - Character Select
        // 2 - Playing
        // 3 - WON
        // 4 - LOSER
        // etc.
        int game_mode;
        public bool quit;

        // Main menu
        int menu_selection;
        int menu_delay;

        #endregion

        #region Constructors
        public ObjectManager(int i_screenSizeX, int i_screenSizeY, GraphicsDevice gfx_device)
        {
            Initialize(i_screenSizeX, i_screenSizeY, gfx_device);
        }

        private void Initialize(int i_screenSizeX, int i_screenSizeY, GraphicsDevice gfx_device)
        {
            graphics_device = gfx_device;

            enemies = new List<Enemy>();
            players = new List<Player>();
            attacks = new List<Attack>();
            character_classes = new List<Player>();
            current_floor = new Floor();

            camera = new Camera();
            camera.ScreenWidth = i_screenSizeX;
            camera.ScreenHeight = i_screenSizeY;
            camera.RoomCenter = new Point(ROOMSIZEX / 2, ROOMSIZEY / 2);

            screen_size_x = i_screenSizeX;
            screen_size_y = i_screenSizeY;

            game_mode = 0;
            quit = false;

            menu_selection = 0;

            //PopulateCharacterList();
        }

        private void PopulateCharacterList()
        {
            Player warrior = new Player();
            warrior.AttackTypeID = 31;
            warrior.AttackCooldown = 60;
            warrior.Position = new Vector3(WARRIOR_LOC_X, WARRIOR_LOC_Y, 0);

            character_classes.Add(warrior);

            Player ranger = new Player();
            ranger.AttackTypeID = 21;
            ranger.AttackCooldown = 60;
            ranger.Position = new Vector3(RANGER_LOC_X, RANGER_LOC_Y, 0);

            character_classes.Add(ranger);

            Player mage = new Player();
            mage.AttackTypeID = 41;
            mage.AttackCooldown = 60;
            mage.Position = new Vector3(MAGE_LOC_X, MAGE_LOC_Y, 0);

            character_classes.Add(mage);
        }

        #endregion

        #region Methods

        #region INPUTS

        public void Enter()
        {
            // Main menu
            if (game_mode == 0)
            {
                // Start
                if (menu_selection == 0)
                {
                    EnterCharacterSelect();
                }

                // Quit
                else if(menu_selection == 1)
                {
                    Quit();
                }
            }
            else if (game_mode == 1)
            {
                StartGame();
            }
            else if (game_mode == 3)
            {
                IncreaseLevel();
            }
            else if (game_mode == 4)
            {
                game_mode = 0;

                ResetFloor();
            }
        }

        public void SetVerticalMovementDirection(float direction, int index)
        {
            switch(game_mode)
            {
                case 0:
                    if(direction < 0)
                        MoveMenu(0, -1);
                    else if (direction > 0)
                        MoveMenu(0, 1);
                    break;
                case 2:
                    this.Get_Player(index).DirectionY = direction;
                    break;
            }
        }

        public void SetHorizontalMovementDirection(float direction, int index)
        {
            switch (game_mode)
            {
                case 1:
                    MoveMenu((int)direction, 0);
                    break;
                case 2:
                    this.Get_Player(index).DirectionX = direction;
                    break;
            }
        }

        public void SetVerticalAttackingDirection(float direction, int index)
        {
            if (game_mode == 2)
            {
                this.Get_Player(index).AttackDirectionY = direction;

                if (direction != 0)
                {
                    this.Get_Player(index).AttemptAttack();
                }
            }
        }

        public void SetHorizontalAttackingDirection(float direction, int index)
        {
            if (game_mode == 2)
            {
                this.Get_Player(index).AttackDirectionX = direction;

                if (direction != 0)
                {
                    this.Get_Player(index).AttemptAttack();
                }
            }
        }

        // Returns the player at the given index
        // Used for inputs
        private Entity Get_Player(int index)
        {
            return players.ElementAt(index);
        }

        #endregion

        #region MENU + GAME MODE

        // Called to begin character selection mode
        private void EnterCharacterSelect()
        {
            game_mode = 1;

            menu_selection = 0;
        }

        // Begin the game after all character stats are set
        private void StartGame()
        {
            game_mode = 2;

            attacks.Clear();
            players.Clear();
            Add_Player(menu_selection, new Vector3(ROOMSIZEX / 2, ROOMSIZEY / 2, 0));
            RevealAdjacentRooms((int)current_floor.CurrentRoomPosition.X, (int)current_floor.CurrentRoomPosition.Y);

            // Warrior
            if (menu_selection == 0)
            {
                // Sword
                players[players.Count - 1].AttackTypeID = 31;
                players[players.Count - 1].AttackCooldown = 45;
                players[players.Count - 1].AttackDamage = 2;
            }

            // Ranger
            else if (menu_selection == 1)
            {
                // Arrows
                players[players.Count - 1].AttackTypeID = 21;
                players[players.Count - 1].AttackCooldown = 30;
                players[players.Count - 1].AttackDamage = 2;
            }

            // Mage
            else if (menu_selection == 2)
            {
                // Staff
                players[players.Count - 1].AttackTypeID = 41;
                players[players.Count - 1].AttackCooldown = 100;
                players[players.Count - 1].AttackDamage = 3;
            }
        }

        // Called when beatin da bawse
        private void Win()
        {
            Sounds.Instance().StopMusic();
            Sounds.Instance().PlayMusic(3, false);
            game_mode = 3;
        }

        // Called when naht a life
        private void Lose()
        {
            Sounds.Instance().StopMusic();
            Sounds.Instance().PlayMusic(2, false);
            game_mode = 4;
        }

        // TODO
        private void Quit()
        {
            quit = true;
        }

        // Move the menu selection based of the given inputs
        private void MoveMenu(int directionX, int directionY)
        {
            if (menu_delay > 0)
                return;

            switch (game_mode)
            {
                // Main menu
                case 0:
                    if(directionY < 0)
                    {
                        Sounds.Instance().PlaySoundEffect(200);
                        menu_selection--;
                        if(menu_selection < 0)
                            menu_selection = MAIN_MENU_LENGTH - 1;
                    }
                    else if(directionY > 0)
                    {
                        Sounds.Instance().PlaySoundEffect(200);
                        menu_selection++;
                        if (menu_selection >= MAIN_MENU_LENGTH)
                            menu_selection = 0;
                    }
                    break;

                // Character select menu
                case 1:
                    if (directionX < 0)
                    {
                        Sounds.Instance().PlaySoundEffect(200);
                        menu_selection--;
                        if (menu_selection < 0)
                            menu_selection = CHARACTER_MENU_LENGTH - 1;
                    }
                    else if (directionX > 0)
                    {
                        Sounds.Instance().PlaySoundEffect(200);
                        menu_selection++;
                        if (menu_selection >= CHARACTER_MENU_LENGTH)
                            menu_selection = 0;
                    }
                    break;
            }

            menu_delay = DEFAULT_MENU_DELAY;
        }

        public void SetGameMode(int new_mode)
        {
            game_mode = new_mode;
        }

        public int GetGameMode()
        {
            return game_mode;
        }

        #endregion

        #region LEVEL CONTROLS
        public void ResetFloor()
        {
            Unload_Room();
            current_floor.Reset();
            Sounds.Instance().StopMusic();
            Sounds.Instance().PlayMusic(0, true);
            StartGame();
        }

        private void ResetFloor(int num_normal_rooms)
        {
            Unload_Room();
            current_floor.Reset(num_normal_rooms);
            Sounds.Instance().StopMusic();
            Sounds.Instance().PlayMusic(0, true);
            StartGame();
        }

        public void RevealEntireMap()
        {
            for (int row = 0; row < current_floor.Height; row++)
            {
                for (int col = 0; col < current_floor.Width; col++)
                {
                    if (current_floor.Get_Room(col, row).Exists)
                        RevealAdjacentRooms(col, row);
                }
            }
        }

        // Reveal any connected room for the given location
        // Pre: the room should exist
        private void RevealAdjacentRooms(int x, int y)
        {
            if (current_floor.RoomExistsAt(x, y))
            {
                current_floor.Get_Room(x, y).Visited = true;

                current_floor.Get_Room(x, y).Visible = true;

                // Right
                if (current_floor.Get_Room(x, y).RightDoor == true)
                    current_floor.Get_Room(x + 1, y).Visible = true;
                // Left
                if (current_floor.Get_Room(x, y).LeftDoor == true)
                    current_floor.Get_Room(x - 1, y).Visible = true;
                // Up
                if (current_floor.Get_Room(x, y).TopDoor == true)
                    current_floor.Get_Room(x, y - 1).Visible = true;
                // Down
                if (current_floor.Get_Room(x, y).BottomDoor == true)
                    current_floor.Get_Room(x, y + 1).Visible = true;
            }
        }

        // Loads enemies into the object manager from a saved room
        private void Load_Room()
        {
            // Add a copy of each enemy to the list
            if (current_floor.CurrentRoom.Completed == false)
            {
                for (int index = 0; index < current_floor.CurrentRoom.Enemy_List().Count; index++)
                {
                    enemies.Add(current_floor.CurrentRoom.Enemy_List()[index]);
                }
            }
        }

        // Saves the status of remaining enemies and empties the object manager's list
        private void Unload_Room()
        {
            // Update the condition of any dead enemies
            current_floor.CurrentRoom.Update_Enemy_List(enemies);

            // Remove all the entities in the room
            enemies.RemoveRange(0, enemies.Count);
            attacks.RemoveRange(0, attacks.Count);
        }

        // Add a player of the given type at the desired location
        public void Add_Player(int ID, Vector3 center)
        {
            players.Add(new Player());
            players[players.Count - 1].Center = center;
        }

        #endregion

        #region UPDATES

        public void Update()
        {
            switch(game_mode)
            {
                // UPDATE MENU LOGIC
                case 0:
                    if(menu_delay > 0)
                        menu_delay--;
                
                break;


                case 1:
                    if (menu_delay > 0)
                        menu_delay--;

                    //UpdateCharacterSelectPlayers();

                break;

                // UPDATE IN-GAME LOGIC
                case 2:
                    UpdateEnemies();

                    UpdatePlayers();

                    UpdateOthers();

                    FixCollisions();

                    // Change the current room if the player is out of bounds
                    ChangeRoomIfAble();
            
                    // Change the camera's location to draw the frame
                    UpdateCamera();

                    // Remove any dead entities
                    Remove_Dead_Things();

                    break;
            }
        }

        private void UpdateCharacterSelectPlayers()
        {
            foreach (Player player in character_classes)
            {
                // warrior
                if (menu_selection == 0)
                {
                    player.DirectionX = 1;
                    player.AttackDirectionY = -1;
                    player.Update();
                    player.Attacking = true;
                    player.AttemptAttack();
                    player.Position = new Vector3(WARRIOR_LOC_X, WARRIOR_LOC_Y, 0);
                }

                // ranger
                if (menu_selection == 1)
                {
                    player.DirectionX = 1;
                    player.AttackDirectionY = -1;
                    player.AttemptAttack();
                    player.Update();
                    player.Position = new Vector3(RANGER_LOC_X, RANGER_LOC_Y, 0);

                }

                // mage
                if (menu_selection == 2)
                {
                    player.DirectionX = 1;
                    player.AttackDirectionY = -1;
                    player.AttemptAttack();
                    player.Update();
                    player.Position = new Vector3(MAGE_LOC_X, MAGE_LOC_Y, 0);
                }
            }
        }

        private void IncreaseLevel()
        {
            game_mode = 2;
            ResetFloor(current_floor.NumNormalRooms + ROOMS_PER_LEVEL);
        }

        // Update enemy positions and make them attack if they can
        private void UpdateEnemies()
        {
            // Update Enemy Positions:
            for (int nme = enemies.Count - 1; nme >= 0; nme--)
            {
                // Search for a target player
                if (enemies[nme].Active)
                {
                    // Set the enemy's target position as the current target player's position
                    enemies[nme].Target = new Vector2(players[enemies[nme].TargetPlayer].CenterX, players[enemies[nme].TargetPlayer].CenterY);
                }
                else
                {
                    // Search for a player to target
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (enemies[nme].SightRange >= Distance_Between_Sprites(enemies[nme], players[i]))
                        {
                            enemies[nme].TargetPlayer = i;
                            enemies[nme].Active = true;
                        }
                    }
                }

                // Attack if primed for attack
                if (enemies[nme].Attacking)
                {
                    MakeAttack(enemies[nme]);
                }

                enemies[nme].Update();

                #region BOSS LOGIC
                // Enemy is the tank boss
                if (enemies[nme].ID == 10)
                {
                    enemies[nme].Active = true;
                    enemies[nme].TargetPlayer = 0;
                    if (FixEnemyToWallCollisions(enemies[nme]))
                        enemies[nme].BossConditionHit();
                }
                #endregion
            }
        }

        // Update player positions. Change room if able
        private void UpdatePlayers()
        {
            // Update Player Position:
            foreach (Player player in players)
            {
                if (player.CurrentHealth <= 0)
                {
                    Sounds.Instance().PlaySoundEffect(0);
                    Lose();
                }

                // Attack if primed for attack
                if (player.Attacking)
                {
                    MakeAttack(player);
                }

                player.Update();
            }
        }
        
        // Update attack entities
        private void UpdateOthers()
        {
            // Update other entitys' Positions:
            foreach (Attack thing in attacks)
            {
                // Melee attack
                if (thing.ID / 10 == 3)
                {
                    foreach (Player player in players)
                    {
                        if (player.UniqueID == thing.SourceID)
                        {
                            thing.SourceCenterX = player.CenterX;
                            thing.SourceCenterY = player.CenterY;
                        }
                    }

                    foreach (Enemy nme in enemies)
                    {
                        if (nme.UniqueID == thing.SourceID)
                        {
                            thing.SourceCenterX = nme.CenterX;
                            thing.SourceCenterY = nme.CenterY;
                        }
                    }
                }

                thing.Update();
            }
        }

        // Change the rooom if one of the players is out of bounds
        private void ChangeRoomIfAble()
        {
            // Change the room if the player is leaving the current room
            foreach (Entity player in players)
            {
                if (player.HitboxLeft < 0) // Entering Left Room
                {
                    if (current_floor.CanMoveLeft())
                    {
                        Unload_Room();
                        current_floor.MoveLeft();
                        player.Position = new Vector3(ROOMSIZEX - player.HitboxWidth - 10, player.Position.Y, 0);
                        RevealAdjacentRooms((int)current_floor.CurrentRoomPosition.X, (int)current_floor.CurrentRoomPosition.Y);
                        Load_Room();
                        if (current_floor.CurrentRoom.Type == 2)
                        {
                            Sounds.Instance().StopMusic();
                            Sounds.Instance().PlayMusic(1 , true);
                        }
                    }
                    else
                        player.Position = new Vector3(0, player.Position.Y, 0);
                }
                else if (player.HitboxRight > ROOMSIZEX) // Entering Right Room
                {
                    if (current_floor.CanMoveRight())
                    {
                        Unload_Room();
                        current_floor.MoveRight();
                        player.Position = new Vector3(10, player.Position.Y, 0);
                        RevealAdjacentRooms((int)current_floor.CurrentRoomPosition.X, (int)current_floor.CurrentRoomPosition.Y);
                        Load_Room();
                        if (current_floor.CurrentRoom.Type == 2)
                        {
                            Sounds.Instance().StopMusic();
                            Sounds.Instance().PlayMusic(1, true);
                        }
                    }
                    else
                        player.Position = new Vector3(ROOMSIZEX - player.HitboxWidth, player.Position.Y, 0);
                }

                if (player.HitboxTop < 0) // Entering Above Room
                {
                    if (current_floor.CanMoveUp())
                    {

                        Unload_Room();
                        current_floor.MoveUp();
                        player.Position = new Vector3(player.Position.X, ROOMSIZEY - player.HitboxHeight - 10, 0);
                        RevealAdjacentRooms((int)current_floor.CurrentRoomPosition.X, (int)current_floor.CurrentRoomPosition.Y);
                        Load_Room();
                        if (current_floor.CurrentRoom.Type == 2)
                        {
                            Sounds.Instance().StopMusic();
                            Sounds.Instance().PlayMusic(1, true);
                        }
                    }
                    else
                        player.Position = new Vector3(player.Position.X, 0, 0);
                }
                else if (player.HitboxBottom > ROOMSIZEY) // Entering Below Room
                {
                    if (current_floor.CanMoveDown())
                    {

                        Unload_Room();
                        current_floor.MoveDown();
                        player.Position = new Vector3(player.Position.X, 10, 0);
                        RevealAdjacentRooms((int)current_floor.CurrentRoomPosition.X, (int)current_floor.CurrentRoomPosition.Y);
                        Load_Room();
                        if (current_floor.CurrentRoom.Type == 2)
                        {
                            Sounds.Instance().StopMusic();
                            Sounds.Instance().PlayMusic(1, true);
                        }
                    }
                    else
                        player.Position = new Vector3(player.Position.X, ROOMSIZEY - player.HitboxHeight, 0);
                }
            }
        }

        // Spawn a new attack entity from the given ntt
        // AttackTypeID changes the way the ntt attacks
        private void MakeAttack(Entity ntt)
        {
            switch (ntt.AttackTypeID)
            {
                // Shoot an arrow
                case 21: Attack pewpew = new Arrow();
                    pewpew.Center = ntt.Center;
                    pewpew.Direction = ntt.AttackDirection;
                    pewpew.SourceID = ntt.UniqueID;
                    pewpew.Damage = ntt.AttackDamage;
                    attacks.Add(pewpew);
                    Sounds.Instance().PlaySoundEffect(21);
                    break;

                // Swing a sword
                case 31: Attack shwing = new Sword();
                    shwing.Center = ntt.Center;
                    shwing.Direction = ntt.AttackDirection;
                    shwing.SourceID = ntt.UniqueID;
                    attacks.Add(shwing);
                    shwing.Damage = ntt.AttackDamage;
                    Sounds.Instance().PlaySoundEffect(31);
                    break;

                // Shoot a ball of fire
                case 41: Attack mason_haduken = new Fireball();
                    mason_haduken.Center = ntt.Center;
                    mason_haduken.Direction = ntt.AttackDirection;
                    mason_haduken.SourceID = ntt.UniqueID;
                    attacks.Add(mason_haduken);
                    mason_haduken.Damage = ntt.AttackDamage;
                    Sounds.Instance().PlaySoundEffect(41);
                    break;


                // No weapon available
                default: break;
            }

            ntt.Attacking = false;
        }

        private void UpdateCamera()
        {
            camera.TargetCenter = new Vector2(players[0].CenterX, players[0].CenterY);
            camera.Update();
        }

        // Removes dead entites after updates
        private void Remove_Dead_Things()
        {
            // Note: traverse backwards so that removing things doesn't ruin the order

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies.ElementAt(i).Is_Dead())
                {
                    Sounds.Instance().PlaySoundEffect(1);
                    enemies.RemoveAt(i);
                    CheckForCompleteRoom();
                }
            }

            for (int i = attacks.Count - 1; i >= 0; i--)
            {
                if (attacks.ElementAt(i).Is_Dead())
                    attacks.RemoveAt(i);
            }
        }

        private void CheckForCompleteRoom()
        {
            if (enemies.Count == 0)
            {
                if (current_floor.CurrentRoom.Type == 2)
                {
                    game_mode = 3;
                }
                else
                {
                    if (current_floor.CurrentRoom.Completed == false)
                    {
                        current_floor.CurrentRoom.Completed = true;
                    }
                }
            }
        }

        private int Distance_Between_Sprites(Sprite one, Sprite two)
        {
            return (int)Math.Sqrt(Math.Abs(one.CenterX - two.CenterX) * Math.Abs(one.CenterX - two.CenterX) +
                                   Math.Abs(one.CenterY - two.CenterY) * Math.Abs(one.CenterY - two.CenterY));
        }


        #endregion

        #region COLLISION

        // Move entities back into valid positions and apply any damage
        private void FixCollisions()
        {
            // players to players
            // players to enemies
            // players to attacks
            // players to walls
            for (int i = 0; i < players.Count; i++)
            {
                // Collisions/Damage with enemies themselves
                FixPlayerToEnemyCollisions(players[i]);

                // Push other players out of the way
                FixPlayerToPlayerCollisions(players[i], i);

                // Collisiosn/Damage with Attack entities
                FixPlayerToAttackCollisions(players[i]);
                
                // Fix any wall conditions (tiles or room border)
                FixPlayerToWallCollisions(players[i]);
            }

            // enemies to enemies
            // enemies to walls
            // enemies to attack
            for (int i = 0; i < enemies.Count; i++)
            {
                FixEnemyToEnemyCollisions(enemies[i], i);
                FixEnemyToWallCollisions(enemies[i]);
                FixEnemyToAttackCollisions(enemies[i]);
            }

            // attacks to attacks
            // attack to walls
            foreach (Attack ntt in attacks)
            {
                //FixAttackToAttackCollisions(ntt);
                FixAttackToWallCollisions(ntt);
            }
        }

        #region Player Collisions
        // Fix collisions between the given player and all enemies
        private void FixPlayerToEnemyCollisions(Entity player)
        {
            // Iterate through each other enemy
            for (int i = 0; i < enemies.Count; i++)
            {
                // Check if the bounding boxes overlap
                BoundingBox other_box = enemies[i].BoundBox;
                if (player.BoundBox.Intersects(other_box))
                {
                    // Check if the circles overlap
                    float min_length = player.Radius + enemies[i].Radius;
                    float actual_length = (float)Math.Sqrt(Math.Pow(player.CenterX - enemies[i].CenterX, 2) + Math.Pow(player.CenterY - enemies[i].CenterY, 2));

                    // Correct the collision if the entities overlap
                    if (actual_length < min_length)
                    {
                        Point collision_point = new Point((int)(((player.CenterX * enemies[i].Radius) + (enemies[i].CenterX * player.Radius)) / (player.Radius + enemies[i].Radius)),
                                                    (int)(((player.CenterY * enemies[i].Radius) + (enemies[i].CenterY * player.Radius)) / (player.Radius + enemies[i].Radius)));
                        
                        // Find the angle of that distance
                        double ntt_angle = Math.Atan2((double)player.CenterY - collision_point.Y, (double)player.CenterX - collision_point.X);
                        double other_angle = Math.Atan2((double)enemies[i].CenterY - collision_point.Y, (double)enemies[i].CenterX - collision_point.X);

                        // The value from the center point to move the entity. Bounce back based on damage TODO
                        float ntt_seperation = player.Radius + 1;
                        float other_seperation = enemies[i].Radius + 1;

                        // Apply damage
                        if (enemies[i].Damage > 0)
                        {
                            player.Hurt(enemies[i].Damage);
                            //ntt_seperation += DAMAGE_BOUNCE;
                        }
                        if (player.Damage > 0)
                        {
                            enemies[i].Hurt(player.Damage);
                            //other_seperation += DAMAGE_BOUNCE;
                        }

                        // Find the location of the entity's center with respect to the center point
                        Vector3 ntt_translation = new Vector3(ntt_seperation * (float)Math.Cos(ntt_angle), ntt_seperation * (float)Math.Sin(ntt_angle), 0);
                        Vector3 other_translation = new Vector3(other_seperation * (float)Math.Cos(other_angle), other_seperation * (float)Math.Sin(other_angle), 0);

                        // Correct the positions of each entity
                        player.Center = new Vector3(collision_point.X + ntt_translation.X, collision_point.Y + ntt_translation.Y, ntt_translation.Z);
                        enemies[i].Center = new Vector3(collision_point.X + other_translation.X, collision_point.Y + other_translation.Y, other_translation.Z);
                    }
                }
            }
        }

        // Fix collisions between the given player and all other players
        private void FixPlayerToPlayerCollisions(Entity player, int index)
        {
            // Iterate through each other enemy
            for (int i = players.Count - 1; i > index; i--)
            {
                // Check if the bounding boxes overlap
                BoundingBox other_box = players[i].BoundBox;
                if (player.BoundBox.Intersects(other_box))
                {
                    // Check if the circles overlap
                    float min_length = player.Radius + players[i].Radius;
                    float actual_length = (float)Math.Sqrt(Math.Pow(player.CenterX - players[i].CenterX, 2) + Math.Pow(player.CenterY - players[i].CenterY, 2));

                    // Correct the collision if the entities overlap
                    if (actual_length < min_length)
                    {
                        // Mean of the two centers
                        float center_X = (player.CenterX + players[i].CenterX) / 2;
                        float center_Y = (player.CenterY + players[i].CenterY) / 2;

                        // Calculate distance from the center point to the centers of each entity
                        Vector3 ntt_vector = new Vector3(player.CenterX - center_X, player.CenterY - center_Y, 0);
                        Vector3 other_vector = new Vector3(players[i].CenterX - center_X, players[i].CenterY - center_Y, 0);

                        // Find the angle of that distance
                        double ntt_angle = Math.Atan2((double)ntt_vector.Y, (double)ntt_vector.X);
                        double other_angle = Math.Atan2((double)other_vector.Y, (double)other_vector.X);

                        // The value from the center point to move the entity. Add one so the entities don't slow each other
                        float ntt_seperation = player.Radius + 1;
                        float other_seperation = players[i].Radius + 1;

                        // Find the location of the entity's center with respect to the center point
                        Vector3 ntt_translation = new Vector3(ntt_seperation * (float)Math.Cos(ntt_angle), ntt_seperation * (float)Math.Sin(ntt_angle), 0);
                        Vector3 other_translation = new Vector3(other_seperation * (float)Math.Cos(other_angle), other_seperation * (float)Math.Sin(other_angle), 0);

                        // Correct the positions of each entity
                        player.Center = new Vector3(center_X + ntt_translation.X, center_Y + ntt_translation.Y, ntt_translation.Z);
                        players[i].Center = new Vector3(center_X + other_translation.X, center_Y + other_translation.Y, other_translation.Z);
                    }
                }
            }
        }

        // Fix collisions between the given player and all other attacks
        private void FixPlayerToAttackCollisions(Entity player)
        {
            // Iterate through each other enemy
            for (int i = attacks.Count - 1; i >= 0; i--)
            {
                // Check to make sure attacks can't hit their owner
                if (attacks[i].SourceID == player.UniqueID)
                {
                    continue;
                }

                // Check if the bounding boxes overlap
                BoundingBox other_box = attacks[i].BoundBox;
                if (player.BoundBox.Intersects(other_box))
                {
                    // Check if the circles overlap
                    float min_length = player.Radius + attacks[i].Radius;
                    float actual_length = (float)Math.Sqrt(Math.Pow(player.CenterX - attacks[i].CenterX, 2) + Math.Pow(player.CenterY - attacks[i].CenterY, 2));

                    // Correct the collision if the entities overlap
                    if (actual_length < min_length)
                    {
                        // Apply damage
                        if (attacks[i].Damage > 0)
                            player.Hurt(attacks[i].Damage);
                        if (attacks[i].Multihit == false)
                        {
                            attacks[i].RemainingFrames = 0;
                        }
                    }
                }
            }
        }

        private void FixPlayerToWallCollisions(Entity ntt)
        {
            #region SolidTiles

            /*
             * 1. Check adjacent boxes
             * 2. If they are walls, check if the entity touches them
             * 3. If the circles collide, move the entity away
             * 4. Repeat until there are no collisions
             * */

            // Check if the entity's box touches a wall
            int wall_index_x;
            int wall_index_y;

            // Corners first:
            // Top Left
            wall_index_x = (int)ntt.TopLeftCorner.X / TILESIZE;
            wall_index_y = (int)ntt.TopLeftCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                }
            }

            // Top Right
            wall_index_x = (int)ntt.TopRightCorner.X / TILESIZE;
            wall_index_y = (int)ntt.TopRightCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                }
            }

            // Bottom Left
            wall_index_x = (int)ntt.BottomLeftCorner.X / TILESIZE;
            wall_index_y = (int)ntt.BottomLeftCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                }
            }

            // Bottom Right
            wall_index_x = (int)ntt.BottomRightCorner.X / TILESIZE;
            wall_index_y = (int)ntt.BottomRightCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                }
            }

            // Look at any points other than corners (for larger entities) TODO if we get bigger entities

            #endregion

            #region Room Walls
            // Outside of the room:
            // Top
            if (ntt.HitboxTop < 0)
            {
                // Check if outside doors
                if (ntt.HitboxLeft < 256 || ntt.HitboxRight > 384)
                {
                    // Push the ntt back in
                    ntt.HitboxTop = 0;
                }
            }
            // Bottom
            else if (ntt.HitboxBottom > 640)
            {
                // Check if outside doors
                if (ntt.HitboxLeft < 256 || ntt.HitboxRight > 384)
                {
                    // Push the ntt back in
                    ntt.HitboxBottom = 640;
                }
            }

            // Left
            if (ntt.HitboxLeft < 0)
            {
                // Check if outside doors
                if (ntt.HitboxTop < 256 || ntt.HitboxBottom > 384)
                {
                    // Push the ntt back in
                    ntt.HitboxLeft = 0;
                }
            }
            // Right
            else if (ntt.HitboxRight > 640)
            {
                // Check if outside doors
                if (ntt.HitboxTop < 256 || ntt.HitboxBottom > 384)
                {
                    // Push the ntt back in
                    ntt.HitboxRight = 640;
                }
            }
            #endregion
        }

        #endregion

        #region Enemy Collisions
        // Fix collisions between the given enemy and all other enemies
        private void FixEnemyToEnemyCollisions(Entity ntt, int index)
        {
            // Iterate through each other enemy
            for (int i = enemies.Count - 1; i > index; i--)
            {
                // Check if the bounding boxes overlap
                BoundingBox other_box = enemies[i].BoundBox;
                if (ntt.BoundBox.Intersects(other_box))
                {
                    // Check if the circles overlap
                    float min_length = ntt.Radius + enemies[i].Radius;
                    float actual_length = (float) Math.Sqrt(Math.Pow(ntt.CenterX - enemies[i].CenterX, 2) + Math.Pow(ntt.CenterY - enemies[i].CenterY, 2));

                    // Correct the collision if the entities overlap
                    if (actual_length < min_length)
                    {
                        // Mean of the two centers
                        float center_X = (ntt.CenterX + enemies[i].CenterX) / 2;
                        float center_Y = (ntt.CenterY + enemies[i].CenterY) / 2;
                        
                        // Calculate distance from the center point to the centers of each entity
                        Vector3 ntt_vector = new Vector3(ntt.CenterX - center_X, ntt.CenterY - center_Y, 0);
                        Vector3 other_vector = new Vector3(enemies[i].CenterX - center_X, enemies[i].CenterY - center_Y, 0);

                        // Find the angle of that distance
                        double ntt_angle = Math.Atan2((double)ntt_vector.Y, (double)ntt_vector.X);
                        double other_angle = Math.Atan2((double)other_vector.Y, (double)other_vector.X);

                        // The value from the center point to move the entity. Add one so the entities don't slow each other
                        float ntt_seperation = ntt.Radius + 1;
                        float other_seperation = enemies[i].Radius + 1;

                        // Find the location of the entity's center with respect to the center point
                        Vector3 ntt_translation = new Vector3(ntt_seperation * (float)Math.Cos(ntt_angle), ntt_seperation * (float)Math.Sin(ntt_angle), 0);
                        Vector3 other_translation = new Vector3(other_seperation * (float)Math.Cos(other_angle), other_seperation * (float)Math.Sin(other_angle), 0);

                        // Correct the positions of each entity
                        ntt.Center = new Vector3(center_X + ntt_translation.X, center_Y + ntt_translation.Y, ntt_translation.Z);
                        enemies[i].Center = new Vector3(center_X + other_translation.X, center_Y + other_translation.Y, other_translation.Z);
                    }
                }
            }
        }

        private bool FixEnemyToWallCollisions(Entity ntt)
        {
            bool collision = false;

            #region Room Walls
            // Outside of the room:
            // Top
            if (ntt.HitboxTop < 0)
            {
                // Push the ntt back in
                ntt.HitboxTop = 0;
                collision = true;
            }
            // Bottom
            else if (ntt.HitboxBottom > 640)
            {
                // Push the ntt back in
                ntt.HitboxBottom = 640;
                collision = true;
            }

            // Left
            if (ntt.HitboxLeft < 0)
            {
                // Push the ntt back in
                ntt.HitboxLeft = 0;
                collision = true;
            }
            // Right
            else if (ntt.HitboxRight > 640)
            {
                // Push the ntt back in
                ntt.HitboxRight = 640;
                collision = true;
            }
            #endregion

            #region SolidTiles

            /*
             * 1. Check adjacent boxes
             * 2. If they are walls, check if the entity touches them
             * 3. If the circles collide, move the entity away
             * 4. Repeat until there are no collisions
             * */

            // Check if the entity's box touches a wall
            int wall_index_x;
            int wall_index_y;

            // Corners first:
            // Top Left
            wall_index_x = (int)ntt.TopLeftCorner.X / TILESIZE;
            wall_index_y = (int)ntt.TopLeftCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                    collision = true;
                }
            }

            // Top Right
            wall_index_x = (int)ntt.TopRightCorner.X / TILESIZE;
            wall_index_y = (int)ntt.TopRightCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                    collision = true;
                }
            }

            // Bottom Left
            wall_index_x = (int)ntt.BottomLeftCorner.X / TILESIZE;
            wall_index_y = (int)ntt.BottomLeftCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                    collision = true;
                }
            }

            // Bottom Right
            wall_index_x = (int)ntt.BottomRightCorner.X / TILESIZE;
            wall_index_y = (int)ntt.BottomRightCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    FixWallCollision(ntt, ((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE);
                    collision = true;
                }
            }

            // Look at any points other than corners (for larger entities) TODO if we get bigger entities

            #endregion

            return collision;
        }

        // Fix collisions between the given enemy and all other attacks
        private void FixEnemyToAttackCollisions(Entity nme)
        {
            // Iterate through each other enemy
            for (int i = attacks.Count - 1; i >= 0; i--)
            {
                // Check to make sure attacks can't hit their owner
                if (attacks[i].SourceID == nme.UniqueID)
                {
                    continue;
                }

                // Check if the bounding boxes overlap
                BoundingBox other_box = attacks[i].BoundBox;
                if (nme.BoundBox.Intersects(other_box))
                {
                    // Check if the circles overlap
                    float min_length = nme.Radius + attacks[i].Radius;
                    float actual_length = (float)Math.Sqrt(Math.Pow(nme.CenterX - attacks[i].CenterX, 2) + Math.Pow(nme.CenterY - attacks[i].CenterY, 2));

                    // Correct the collision if the entities overlap
                    if (actual_length < min_length)
                    {
                        // Apply damage
                        if (attacks[i].Damage > 0)
                        {
                            nme.Hurt(attacks[i].Damage);
                        }
                        if (attacks[i].Multihit == false)
                        {
                            attacks[i].RemainingFrames = 0;
                        }
                    }
                }
            }
        }

        #endregion

        #region Attack Collisions
        private void FixAttackToAttackCollisions(Entity ntt)
        {

        }

        private void FixAttackToWallCollisions(Attack ntt)
        {
            #region Room Walls
            // Outside of the room:
            if (ntt.HitboxTop < 0  || ntt.HitboxBottom > 640 ||
                ntt.HitboxLeft < 0 || ntt.HitboxRight > 640)
            {
                // Destroy the attack if it is a projectile
                if(IsProjectile(ntt))
                    ntt.RemainingFrames = 0;
            }
            #endregion

            #region SolidTiles

            /*
             * 1. Check adjacent boxes
             * 2. If they are walls, check if the entity touches them
             * 3. If the circles collide, move the entity away
             * 4. Repeat until there are no collisions
             * */

            // Check if the entity's box touches a wall
            int wall_index_x;
            int wall_index_y;

            // Corners first:
            // Top Left
            wall_index_x = (int)ntt.TopLeftCorner.X / TILESIZE;
            wall_index_y = (int)ntt.TopLeftCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    if (IsProjectile(ntt))
                        ntt.RemainingFrames = 0;
                }
            }

            // Top Right
            wall_index_x = (int)ntt.TopRightCorner.X / TILESIZE;
            wall_index_y = (int)ntt.TopRightCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    if (IsProjectile(ntt))
                        ntt.RemainingFrames = 0;
                }
            }

            // Bottom Left
            wall_index_x = (int)ntt.BottomLeftCorner.X / TILESIZE;
            wall_index_y = (int)ntt.BottomLeftCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    if (IsProjectile(ntt))
                        ntt.RemainingFrames = 0;
                }
            }

            // Bottom Right
            wall_index_x = (int)ntt.BottomRightCorner.X / TILESIZE;
            wall_index_y = (int)ntt.BottomRightCorner.Y / TILESIZE;

            // Is there a wall at the ntt_point?
            if (wall_index_x < current_floor.CurrentRoom.MaxX && wall_index_x >= 0 &&
                wall_index_y < current_floor.CurrentRoom.MaxY && wall_index_y >= 0 &&
                IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_index_x, wall_index_y)))
            {
                if (CircleTouchesWall(ntt.Center, ntt.Radius, new Vector3(((float)wall_index_x + 0.5f) * TILESIZE, ((float)wall_index_y + 0.5f) * TILESIZE, 0.0f)))
                {
                    if (IsProjectile(ntt))
                        ntt.RemainingFrames = 0;
                }
            }

            // Look at any points other than corners (for larger entities) TODO if we get bigger entities

            #endregion
        }

        #endregion

        private bool IsProjectile(Entity ntt)
        {
            if (ntt.ID == 21 || ntt.ID == 41)
                return true;
            return false;
        }

        // Circle collision. Wall is a circle
        private bool CircleTouchesWall(Vector3 point, float radius, Vector3 wall_center)
        {
            int wall_coord_x = (int)(wall_center.X / TILESIZE);
            int wall_coord_y = (int)(wall_center.Y / TILESIZE);

            if (IsWall(current_floor.CurrentRoom.Get_Tile_Type(wall_coord_x, wall_coord_y)) == false)
            {
                return false;
            }

            double distance = Math.Sqrt(Math.Pow(point.X - wall_center.X, 2) + Math.Pow(point.Y - wall_center.Y, 2));

            if (distance < TILESIZE / 2 + radius)
                return true;
            else
                return false;
        }

        private void FixWallCollision(Entity ntt, float wall_center_x, float wall_center_y)
        {
            // Find angle from wall's center to ntt
            double angle = Math.Atan2(ntt.CenterY - wall_center_y, ntt.CenterX - wall_center_x);

            // Move the ntt away from the wall
            ntt.Center = new Vector3(wall_center_x + (ntt.Radius + TILESIZE / 2 + 1) * (float)Math.Cos(angle),
                                     wall_center_y + (ntt.Radius + TILESIZE / 2 + 1) * (float)Math.Sin(angle),
                                     0);
        }
        
        private bool IsWall(int tile_type)
        {
            if (tile_type == 1)
                return true;
            else
                return false;
        }

        #endregion

        #region DRAWING

        public void Draw(SpriteBatch batch)
        {
            switch (game_mode)
            {
                // Main menu
                case 0:
                    DrawMainMenu(batch);

                    break;
                case 1:
                    DrawCharacterSelection(batch);

                    break;
                
                // Playing the game
                case 2:
                    DrawRoom(batch);

                    DrawColumnBases(batch);

                    DrawEntities(batch);

                    DrawColumnTops(batch);

                    DrawEnemyHealthBar(batch);

                    DrawHUD(batch);

                    break;
                case 3:
                    DrawRoom(batch);

                    DrawColumnBases(batch);

                    DrawEntities(batch);

                    DrawColumnTops(batch);

                    DrawLevelComplete(batch);
                    break;
                case 4:
                    DrawRoom(batch);

                    DrawColumnBases(batch);

                    DrawEntities(batch);

                    DrawColumnTops(batch);

                    DrawGameOver(batch);
                    break;
            }
        }

        private void DrawLevelComplete(SpriteBatch batch)
        {
            batch.Begin();

            // Game over screen
            DrawFullPicture(batch, 600, new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(600).Width / 2, LEVEL_COMPLETE_HEIGHT));

            batch.End();
        }

        private void DrawGameOver(SpriteBatch batch)
        {
            batch.Begin();

            // Game over screen
            DrawFullPicture(batch, 500, new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(500).Width / 2, GAME_OVER_HEIGHT));

            batch.End();
        }

        // Draw the main menu
        private void DrawMainMenu(SpriteBatch batch)
        {
            float menu_height = MAIN_MENU_BUTTONS_Y;

            batch.Begin();

            // Title
            DrawFullPicture(batch, 100, new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(100).Width / 2, MAIN_MENU_TITLE_Y));

            // Start
            DrawButton(batch, 0, new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(0).Width / 2, menu_height));
            menu_height += Images.Instance().GetMenuTexture(0).Height / 2;

            // Quit
            DrawButton(batch, 1, new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(1).Width / 2, menu_height));

            batch.End();
        }

        private void DrawFullPicture(SpriteBatch batch, int picture_ID, Vector2 position)
        {
            batch.Draw(Images.Instance().GetMenuTexture(picture_ID),
                       position,
                       Color.White);
        }

        // Draw a button of the given type at the desired position
        private void DrawButton(SpriteBatch batch, int button_type, Vector2 position)
        {
            // Draw the inactive button
            if(ButtonActive(button_type) == false)
                batch.Draw(Images.Instance().GetMenuTexture(button_type),
                           position,
                           new Rectangle(0, 0, Images.Instance().GetMenuTexture(1).Width, Images.Instance().GetMenuTexture(1).Height / 2),
                           Color.White);

            // Draw the active button
            else
                batch.Draw(Images.Instance().GetMenuTexture(button_type),
                           position,
                           new Rectangle(0, Images.Instance().GetMenuTexture(button_type).Height / 2, Images.Instance().GetMenuTexture(1).Width, Images.Instance().GetMenuTexture(1).Height),
                           Color.White);
        }


        // Draw an arrow at the given location with the desired rotation
        private void DrawArrow(SpriteBatch batch, Vector2 position, float angle_radians)
        {
            batch.Draw(Images.Instance().GetMenuTexture(400),
                       position,
                       new Rectangle(0, 0, Images.Instance().GetMenuTexture(400).Width, Images.Instance().GetMenuTexture(400).Height),
                       Color.White,
                       angle_radians,
                       new Vector2(Images.Instance().GetMenuTexture(400).Width / 2, Images.Instance().GetMenuTexture(400).Height / 2),
                       1.0f,
                       SpriteEffects.None,
                       0.0f);
        }

        // Check if the given button is active
        private bool ButtonActive(int button_type)
        {
            if (button_type == menu_selection)
                return true;
            return false;
        }

        // Draw the character select screen
        private void DrawCharacterSelection(SpriteBatch batch)
        {
            batch.Begin();

            // Character select text
            DrawFullPicture(batch,
                            200,
                            new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(200).Width / 2, CHARACTER_MENU_TITLE_Y));

            // Characters
            DrawFullPicture(batch,
                            300,
                            new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(300).Width / 2, CHARACTER_PICTURE_Y));

            //DrawMovingCharacter(batch);

            // Arrow to show current selection
            DrawArrow(batch,
                      new Vector2(screen_size_x / 2 - Images.Instance().GetMenuTexture(200).Width / 2 + Images.Instance().GetMenuTexture(200).Width / (CHARACTER_MENU_LENGTH * 2) + Images.Instance().GetMenuTexture(200).Width / CHARACTER_MENU_LENGTH * menu_selection,
                                  CHARACTER_PICTURE_Y + Images.Instance().GetMenuTexture(300).Height - Images.Instance().GetMenuTexture(200).Height / 2),
                      (float)Math.PI * 3 / 2);

            batch.End();
        }

        private void DrawMovingCharacter(SpriteBatch batch)
        {
            foreach (Entity ntt in character_classes)
            {
                ntt.Draw(batch, camera);
            }
            foreach (Entity attack in attacks)
            {
                attack.Draw(batch, camera);
            }
        }

        // Draw the current room
        // Pre: the batch must be active.
        private void DrawRoom(SpriteBatch batch)
        {
            batch.Begin();

            current_floor.CurrentRoom.Draw(batch, camera);

            batch.End();
        }

        private void DrawColumnBases(SpriteBatch batch)
        {
            batch.Begin();

            current_floor.CurrentRoom.DrawColumnBase(batch, camera);

            batch.End();
        }

        private void DrawColumnTops(SpriteBatch batch)
        {
            batch.Begin();

            current_floor.CurrentRoom.DrawColumnTops(batch, camera);

            batch.End();
        }

        // Draw all the entities in the current room
        // Pre: the batch must be active.
        private void DrawEntities(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            foreach (Entity ntt in enemies)
            {
                ntt.Draw(batch, camera);
            }

            foreach (Entity ntt in players)
            {
                ntt.Draw(batch, camera);
            }

            foreach (Entity ntt in attacks)
            {
                ntt.Draw(batch, camera);
            }

            batch.End();
        }

        // Draw the HUD
        // Pre: the batch must be active.
        private void DrawHUD(SpriteBatch batch)
        {
            batch.Begin();

            DrawMap(batch, 0, 0);

            DrawPlayerStatus(batch, 700, 50);

            batch.End();
        }

        private void DrawEnemyHealthBar(SpriteBatch batch)
        {
            batch.Begin();

            foreach (Enemy enemy in enemies)
            {
                enemy.DrawHealthBar(batch, camera);
            }

            batch.End();
        }

        // Draw the map in the given location
        // Pre: the batch must be active.
        private void DrawMap(SpriteBatch batch, int pos_x, int pos_y)
        {
            // Draw the grid of rooms
            for (int x = 0; x < current_floor.Width; x++)
            {
                for (int y = 0; y < current_floor.Height; y++)
                {
                    if (current_floor.Get_Room(x, y).Visible == true)
                    {
                        if (current_floor.Get_Room(x, y).Visited == true)
                        {
                            batch.Draw(Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)),
                                       new Vector2(pos_x + x * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Width,
                                                   pos_y + y * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Height),
                                       Color.White);
                        }
                        else
                        {
                            batch.Draw(Images.Instance().GetHUDTexture(-4),
                                       new Vector2(pos_x + x * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Width,
                                                   pos_y + y * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Height),
                                       Color.White);
                        }
                    }
                    else
                    {
                        batch.Draw(Images.Instance().GetHUDTexture(-1),
                                   new Vector2(pos_x + x * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Width,
                                               pos_y + y * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Height),
                                   Color.White);
                    }
                }
            }

            // Draw the current room marker
            batch.Draw(Images.Instance().GetHUDTexture(-2),
                                new Vector2(pos_x + current_floor.CurrentRoomPosition.X * Images.Instance().GetHUDTexture(-2).Width,
                                            pos_y + current_floor.CurrentRoomPosition.Y * Images.Instance().GetHUDTexture(-2).Height),
                                Color.White);

            // Draw connections between rooms
            // Note: Only need to check for two connections to optimize
            for (int x = 0; x < current_floor.Width; x++)
            {
                for (int y = 0; y < current_floor.Height; y++)
                {
                    // Check for bottom connection
                    if (current_floor.Get_Room(x, y).BottomDoor)
                    {
                        // Check if it should be drawn
                        if (current_floor.Get_Room(x, y).Visited == true ||
                           (current_floor.RoomExistsAt(x, y + 1) && current_floor.Get_Room(x, y + 1).Visited == true))
                        {
                            batch.Draw(Images.Instance().GetHUDTexture(-3),
                                     new Vector2(pos_x + (x + 0.5f) * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Width - Images.Instance().GetHUDTexture(-3).Width / 2,
                                                 pos_y + (y + 1) * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Height - Images.Instance().GetHUDTexture(-3).Width / 2),
                                     Color.White);
                        }
                    }

                    // Check for right connection
                    if (current_floor.Get_Room(x, y).RightDoor)
                    {
                        // Check if it should be drawn
                        if (current_floor.Get_Room(x, y).Visited == true ||
                           (current_floor.RoomExistsAt(x + 1, y) && current_floor.Get_Room(x + 1, y).Visited == true))
                        {
                            batch.Draw(Images.Instance().GetHUDTexture(-3),
                                     new Vector2(pos_x + (x + 1) * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Width - Images.Instance().GetHUDTexture(-3).Width / 2,
                                                 pos_y + (y + 0.5f) * Images.Instance().GetHUDTexture(current_floor.Get_Room_Type(x, y)).Height - Images.Instance().GetHUDTexture(-3).Width / 2),
                                     Color.White);
                        }
                    }
                }
            }
        }

        // Draw health bars and weapon
        private void DrawPlayerStatus(SpriteBatch batch, int pos_x, int pos_y)
        {
            #region Health Bar
            // Draw black background
            batch.Draw(Images.Instance().GetHUDTexture(12),
                       new Rectangle(pos_x + OVERLAY_WIDTH,
                                     pos_y + OVERLAY_HEIGHT,
                                     FULL_STATUS_BAR_WIDTH,
                                     Images.Instance().GetHUDTexture(10).Height),
                       Color.White);

            // Draw health bar
            batch.Draw(Images.Instance().GetHUDTexture(10),
                       new Rectangle(pos_x + OVERLAY_WIDTH,
                                     pos_y + OVERLAY_HEIGHT,
                                     (int)(((float)players[0].CurrentHealth / players[0].MaxHealth) * FULL_STATUS_BAR_WIDTH),
                                     Images.Instance().GetHUDTexture(10).Height),
                       Color.Red);

            // Draw Health bar overlay
            batch.Draw(Images.Instance().GetHUDTexture(11),
                       new Rectangle(pos_x,
                                     pos_y,
                                     Images.Instance().GetHUDTexture(11).Width,
                                     Images.Instance().GetHUDTexture(11).Height),
                       Color.White);

            #endregion

            #region Stamina Bar

            // Draw black background
            batch.Draw(Images.Instance().GetHUDTexture(12),
                       new Rectangle(pos_x + OVERLAY_WIDTH,
                                     pos_y + OVERLAY_HEIGHT + HP_STAMINA_GAP + Images.Instance().GetHUDTexture(11).Height,
                                     FULL_STATUS_BAR_WIDTH,
                                     Images.Instance().GetHUDTexture(12).Height),
                       Color.White);

            if (players[0].CurrentAttackCooldown == 0)
            {
                // Draw Stamina bar
                batch.Draw(Images.Instance().GetHUDTexture(10),
                           new Rectangle(pos_x + OVERLAY_WIDTH,
                                         pos_y + OVERLAY_HEIGHT + HP_STAMINA_GAP + Images.Instance().GetHUDTexture(11).Height,
                                         (FULL_STATUS_BAR_WIDTH),
                                         Images.Instance().GetHUDTexture(10).Height),
                           Color.Green);
            }
            else
            {
                // Draw Stamina bar
                batch.Draw(Images.Instance().GetHUDTexture(10),
                           new Rectangle(pos_x + OVERLAY_WIDTH,
                                         pos_y + OVERLAY_HEIGHT + HP_STAMINA_GAP + Images.Instance().GetHUDTexture(11).Height,
                                         (int)((1 - ((float)players[0].CurrentAttackCooldown / players[0].AttackCooldown)) * FULL_STATUS_BAR_WIDTH),
                                         Images.Instance().GetHUDTexture(10).Height),
                           Color.Green);
            }
            // Draw Health bar overlay
            batch.Draw(Images.Instance().GetHUDTexture(13),
                       new Rectangle(pos_x,
                                     pos_y + HP_STAMINA_GAP + Images.Instance().GetHUDTexture(11).Height,
                                     Images.Instance().GetHUDTexture(13).Width,
                                     Images.Instance().GetHUDTexture(13).Height),
                       Color.White);
            #endregion
        }

        #endregion

        #endregion
    }
}
