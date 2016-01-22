using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Room
    {
        #region Members
        // Storage:
        int[][] tiles;
        const int TILE_MAX_X = 10;
        const int TILE_MAX_Y = 10;

        // Generation:
        bool exists;                                    // Determines if a room exists in a floor and can be entered
        bool door_right, door_left, door_up, door_down; // Do the 4 walls have doors?
        int type;                                       // The type of room this is (spawn, normal, boss, etc.)
        int variant;                                    // The specific layout type for the room

        // Game Logic:
        bool completed;                                 // False when first entering the room. True when all enemies are dead
        bool visible;                                   // False at first. Revealed in the object manager
        bool visited;                                   // False at first. Set to true when first entered.
        List<Enemy> enemies;                            // Store the enemies for the room.

        #endregion

        #region Constructors
        public Room()
        {
            Initialize(-1);
        }

        public Room(int i_type)
        {
            Initialize(i_type);
        }

        private void Initialize(int i_type)
        {
            // Create 10X10 array of tiles
            tiles = new int[TILE_MAX_X][];
            for (int i = 0; i < TILE_MAX_X; i++)
            {
                tiles[i] = new int[TILE_MAX_Y];
            }

            exists = false;
            door_right = false;
            door_left = false;
            door_up = false;
            door_down = false;

            type = i_type;

            completed = false;
            visible = false;
            visited = false;
            enemies = new List<Enemy>();

            Clear();

            CreateRoom(type);
        }

        #endregion

        #region Random Room Generation

        // Create a room of the given type.
        // 0 = spawn
        // 1 = normal
        // 2 = boss
        private void CreateRoom(int i_type)
        {
            switch (i_type)
            {
                case -1: break;
                case 0: MakeSpawnRoom(); break;
                case 1: MakeNormalRoom(); break;
                case 2: MakeBossRoom(); break;
            }
        }

        // Create the layout of a spawn room
        private void MakeSpawnRoom()
        {
            Room temp = Levels.Instance().GetRandomRoom(0);

            for (int i = 0; i < TILE_MAX_X; i++)
            {
                for (int j = 0; j < TILE_MAX_Y; j++)
                {
                    tiles[i][j] = temp.Get_Tile_Type(i, j);
                }
            }

            completed = true;

            TransferSprites(temp);
        }

        // Create the layout of a normal room
        private void MakeNormalRoom()
        {
            Room temp = Levels.Instance().GetRandomRoom(1);

            for (int i = 0; i < TILE_MAX_X; i++)
            {
                for (int j = 0; j < TILE_MAX_Y; j++)
                {
                    tiles[i][j] = temp.Get_Tile_Type(i, j);
                }
            }

            TransferSprites(temp);
        }

        // Create the layout of a boss room
        private void MakeBossRoom()
        {
            Room temp = Levels.Instance().GetRandomRoom(2);

            for (int i = 0; i < TILE_MAX_X; i++)
            {
                for (int j = 0; j < TILE_MAX_Y; j++)
                {
                    tiles[i][j] = temp.Get_Tile_Type(i, j);
                }
            }

            TransferSprites(temp);
        }

        // Copy any sprites into this room from the given room
        private void TransferSprites(Room from_this)
        {
            for (int i = 0; i < from_this.Enemy_List().Count; i++)
            {
                Add_Entity(from_this.Enemy_List()[i].ID, from_this.Enemy_List()[i].Position);
            }
        }

        // Add an entity to the room
        public void Add_Entity(int ID, Vector3 center)
        {
            switch (ID)
            {
                case 1: enemies.Add(new Chaser());
                    enemies[enemies.Count - 1].Center = center;
                    break;
                case 2: enemies.Add(new Shooter());
                    enemies[enemies.Count - 1].Center = center;
                    break;
                case 3: enemies.Add(new BatMan());
                    enemies[enemies.Count - 1].Center = center;
                    break;
                case 10: enemies.Add(new Tank());
                    enemies[enemies.Count - 1].Center = center;
                    break;
            }
        }

        // Remove an entity from storage when it is killed
        // Only called when exiting a room to "save" any progress
        private void Remove_entity(int index)
        {
            if (index < enemies.Count)
            {
                enemies.RemoveAt(index);
            }
        }

        private void Clear()
        {
            for (int y = 0; y < TILE_MAX_Y; y++)
            {
                for (int x = 0; x < TILE_MAX_X; x++)
                {
                    tiles[x][y] = 0;
                }
            }

            enemies.Clear();
        }

        #endregion

        #region Methods

        // Update the saved info for the enemies in this room.
        // Deletes any dead enemies. Does not change initial positions for living enemies
        public void Update_Enemy_List(List<Enemy> list)
        {
            for (int index = list.Count - 1; index >= 0; index--)
            {
                if (index < enemies.Count)
                {
                    // Remove any dead enemies
                    if (list[index].CurrentHealth <= 0)
                    {
                        enemies.RemoveAt(index);
                    }
                }
            }
        }

        // Returns the stored list of enemies. Enemies that were killed before aren't returned
        public List<Enemy> Enemy_List()
        {
            return enemies;
        }

        public void Draw(SpriteBatch batch, Camera camera)
        {
            // Draw the edge walls
            batch.Draw(Images.Instance().GetRoomTexture(100),
                       new Vector2(288 - camera.X, 288 - camera.Y),
                       new Rectangle(0, 0, Images.Instance().GetRoomTexture(100).Width, Images.Instance().GetRoomTexture(100).Height),
                       Color.White,
                       0.0f,
                       new Vector2(Images.Instance().GetRoomTexture(100).Width / 2, Images.Instance().GetRoomTexture(100).Height / 2),
                       1.0f,
                       SpriteEffects.None,
                       0.0f);

            // Draw each door frame

            if (completed == true)
            {
                #region Blocked Doors
                // Left
                if (door_left == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(-65 - camera.X, 288 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               (float)(1.5 * Math.PI),
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }

                // Right
                if (door_right == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(640 - camera.X, 288 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               (float)(0.5 * Math.PI),
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }

                // Up
                if (door_up == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(288 - camera.X, -64 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               0,
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }

                // Down
                if (door_down == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(288 - camera.X, 640 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               0,
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.FlipVertically,
                               0.99f);
                }

                #endregion
            }
            else
            {
                #region Blocked Doors or Not Completed Doors
                // Left
                if (door_left == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(-65 - camera.X, 288 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               (float)(1.5 * Math.PI),
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }
                else
                {
                    batch.Draw(Images.Instance().GetRoomTexture(101),
                               new Vector2(-65 - camera.X, 288 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(101).Width,
                                             Images.Instance().GetRoomTexture(101).Height),
                               Color.RosyBrown,
                               (float)(1.5 * Math.PI),
                               new Vector2(Images.Instance().GetRoomTexture(101).Width / 2, Images.Instance().GetRoomTexture(101).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }

                // Right
                if (door_right == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(640 - camera.X, 288 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               (float)(0.5 * Math.PI),
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }
                else
                {
                    batch.Draw(Images.Instance().GetRoomTexture(101),
                               new Vector2(640 - camera.X, 288 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(101).Width,
                                             Images.Instance().GetRoomTexture(101).Height),
                               Color.RosyBrown,
                               (float)(0.5 * Math.PI),
                               new Vector2(Images.Instance().GetRoomTexture(101).Width / 2, Images.Instance().GetRoomTexture(101).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }

                // Up
                if (door_up == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(288 - camera.X, -64 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               0,
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }
                else
                {
                    batch.Draw(Images.Instance().GetRoomTexture(101),
                               new Vector2(288 - camera.X, -64 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(101).Width,
                                             Images.Instance().GetRoomTexture(101).Height),
                               Color.RosyBrown,
                               0,
                               new Vector2(Images.Instance().GetRoomTexture(101).Width / 2, Images.Instance().GetRoomTexture(101).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.99f);
                }

                // Down
                if (door_down == false)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(102),
                               new Vector2(288 - camera.X, 640 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(102).Width,
                                             Images.Instance().GetRoomTexture(102).Height),
                               Color.White,
                               0,
                               new Vector2(Images.Instance().GetRoomTexture(102).Width / 2, Images.Instance().GetRoomTexture(102).Height / 2),
                               1.0f,
                               SpriteEffects.FlipVertically,
                               0.99f);
                }
                else
                {
                    batch.Draw(Images.Instance().GetRoomTexture(101),
                               new Vector2(288 - camera.X, 640 - camera.Y),
                               new Rectangle(0,
                                             0,
                                             Images.Instance().GetRoomTexture(101).Width,
                                             Images.Instance().GetRoomTexture(101).Height),
                               Color.RosyBrown,
                               0,
                               new Vector2(Images.Instance().GetRoomTexture(101).Width / 2, Images.Instance().GetRoomTexture(101).Height / 2),
                               1.0f,
                               SpriteEffects.FlipVertically,
                               0.99f);
                }

                #endregion
            }

            // Draw the floor tiles for the room:
            for (int x = 0; x < TILE_MAX_X; x++)
            {
                for (int y = 0; y < TILE_MAX_Y; y++)
                {
                    batch.Draw(Images.Instance().GetRoomTexture(tiles[x][y]),
                               new Vector2(x * Images.Instance().GetRoomTexture(tiles[x][y]).Width - camera.X, y * Images.Instance().GetRoomTexture(tiles[x][y]).Height - camera.Y),
                               new Rectangle(0, 0, Images.Instance().GetRoomTexture(tiles[x][y]).Width, Images.Instance().GetRoomTexture(tiles[x][y]).Height),
                               Color.White,
                               0.0f,
                               new Vector2(Images.Instance().GetRoomTexture(tiles[x][y]).Width / 2, Images.Instance().GetRoomTexture(tiles[x][y]).Height / 2),
                               1.0f,
                               SpriteEffects.None,
                               0.0f);
                }
            }
        }

        public void DrawColumnBase(SpriteBatch batch, Camera camera)
        {
            for (int x = 0; x < TILE_MAX_X; x++)
            {
                for (int y = 0; y < TILE_MAX_Y; y++)
                {
                    // Draw the column
                    if (IsWall(tiles[x][y]))
                    {
                        batch.Draw(Images.Instance().GetRoomTexture(50),
                                   new Vector2(x * Images.Instance().GetRoomTexture(tiles[x][y]).Width - camera.X, y * Images.Instance().GetRoomTexture(tiles[x][y]).Height - (Images.Instance().GetRoomTexture(50).Height - Images.Instance().GetRoomTexture(tiles[x][y]).Height) - camera.Y),
                                   new Rectangle(0, 0, Images.Instance().GetRoomTexture(50).Width, Images.Instance().GetRoomTexture(50).Height),
                                   Color.White,
                                   0.0f,
                                   new Vector2(Images.Instance().GetRoomTexture(tiles[x][y]).Width / 2, Images.Instance().GetRoomTexture(tiles[x][y]).Height / 2),
                                   1.0f,
                                   SpriteEffects.None,
                                   (float)y / TILE_MAX_Y);
                    }
                }
            }
        }

        public void DrawColumnTops(SpriteBatch batch, Camera camera)
        {
            for (int x = 0; x < TILE_MAX_X; x++)
            {
                for (int y = 0; y < TILE_MAX_Y; y++)
                {
                    // Draw the column
                    if (IsWall(tiles[x][y]))
                    {
                        batch.Draw(Images.Instance().GetRoomTexture(50),
                                   new Vector2(x * Images.Instance().GetRoomTexture(tiles[x][y]).Width - camera.X, y * Images.Instance().GetRoomTexture(tiles[x][y]).Height - (Images.Instance().GetRoomTexture(50).Height - Images.Instance().GetRoomTexture(tiles[x][y]).Height) - camera.Y),
                                   new Rectangle(0, 0, Images.Instance().GetRoomTexture(50).Width, Images.Instance().GetRoomTexture(50).Height - Images.Instance().GetRoomTexture(tiles[x][y]).Height / 2),
                                   Color.White,
                                   0.0f,
                                   new Vector2(Images.Instance().GetRoomTexture(tiles[x][y]).Width / 2, Images.Instance().GetRoomTexture(tiles[x][y]).Height / 2),
                                   1.0f,
                                   SpriteEffects.None,
                                   (float)y / TILE_MAX_Y);
                    }
                }
            }
        }
            

        bool IsWall(int tileType)
        {
            if (tileType == 1)
                return true;
            return false;
        }

        #endregion

        #region Properties

        public int MaxX
        {
            get { return TILE_MAX_X; }
        }

        public int MaxY
        {
            get { return TILE_MAX_Y; }
        }

        public void Set_Tile_Type(int x, int y, int type)
        {
            tiles[x][y] = type;
        }

        public int Get_Tile_Type(int x, int y)
        {
            return tiles[x][y];
        }

        public bool Is_Connected()
        {
            if (door_right || door_left || door_up || door_down)
                return true;
            return false;
        }

        public bool Exists
        {
            set { exists = value; }
            get { return exists; }
        }

        public bool RightDoor
        {
            set { door_right = value; }
            get { return door_right; }
        }

        public bool LeftDoor
        {
            set { door_left = value; }
            get { return door_left; }
        }
        public bool TopDoor
        {
            set { door_up = value; }
            get { return door_up; }
        }
        public bool BottomDoor
        {
            set { door_down = value; }
            get { return door_down; }
        }
        public int Type
        {
            set { type = value; }
            get { return type; }
        }
        public int Variant
        {
            get { return variant; }
        }
        public bool Completed
        {
            set { completed = value; }
            get { return completed; }
        }
        public bool Visible
        {
            set { visible = value; }
            get { return visible; }
        }
        public bool Visited
        {
            set { visited = value; }
            get { return visited; }
        }

        #endregion
    }
}
