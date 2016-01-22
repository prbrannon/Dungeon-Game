using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Floor
    {
        #region Members
        const int DEFAULT_NUM_ROOMS = 5;
        
        List<List<Room>> rooms;         // Array of rooms within the level
        int list_width, list_height;    // Size of the rooms array
        Vector2 tl_room, br_room;       // Expanse of the block of rooms in the level
        int num_rooms;                  // The number of total rooms on the floor
        int num_normal_rooms;           // The number of non-essential rooms on the floor
        Vector2 current_room;           // Position of the player
        #endregion

        #region constructors
        public Floor()
        {
            Initialize(DEFAULT_NUM_ROOMS);
        }

        // Create a floor with a number of rooms
        // *The spawn does not count as a room
        public Floor(int num_rooms)
        {
            Initialize(num_rooms);
        }

        private void Initialize(int i_rooms)
        {
            rooms = new List<List<Room>>();
            list_width = 0;
            list_height = 0;
            tl_room = new Vector2(0, 0);
            br_room = new Vector2(0, 0);
            num_rooms = 0;
            num_normal_rooms = 0;
            current_room = new Vector2(0, 0);

            bool generate_fail = true;

            while (generate_fail)
            {
                generate_fail = GenerateFloor(i_rooms);
            }
        }

        #endregion

        #region Methods

        public void Reset()
        {
            GenerateFloor(DEFAULT_NUM_ROOMS);
        }

        public void Reset(int num_rooms)
        {
            GenerateFloor(num_rooms);
        }



        public bool CanMoveLeft()
        {
            if (rooms[(int)current_room.X][(int)current_room.Y].Completed == false)
                return false;

            // Room is within bounds
            if (current_room.X > tl_room.X)
            {
                // Rooom exists
                if(rooms[(int)current_room.X - 1][(int)current_room.Y].Exists)
                {
                    if (rooms[(int)current_room.X][(int)current_room.Y].LeftDoor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanMoveRight()
        {
            if (rooms[(int)current_room.X][(int)current_room.Y].Completed == false)
                return false;

            // Room is within bounds
            if (current_room.X < br_room.X)
            {
                // Rooom exists
                if (rooms[(int)current_room.X + 1][(int)current_room.Y].Exists)
                {
                    if (rooms[(int)current_room.X][(int)current_room.Y].RightDoor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanMoveUp()
        {
            if (rooms[(int)current_room.X][(int)current_room.Y].Completed == false)
                return false;

            // Room is within bounds
            if (current_room.Y > tl_room.Y)
            {
                // Rooom exists
                if (rooms[(int)current_room.X][(int)current_room.Y - 1].Exists)
                {
                    if (rooms[(int)current_room.X][(int)current_room.Y].TopDoor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanMoveDown()
        {
            if (rooms[(int)current_room.X][(int)current_room.Y].Completed == false)
                return false;

            // Room is within bounds
            if (current_room.Y < br_room.Y)
            {
                // Rooom exists
                if (rooms[(int)current_room.X][(int)current_room.Y + 1].Exists)
                {
                    if (rooms[(int)current_room.X][(int)current_room.Y].BottomDoor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveLeft()
        {
            current_room.X--;
        }

        public void MoveRight()
        {
            current_room.X++;
        }

        public void MoveUp()
        {
            current_room.Y--;
        }

        public void MoveDown()
        {
            current_room.Y++;
        }

        private Vector2 GetSpawnLocation()
        {
            Vector2 loc = new Vector2(-1, -1);

            for (int x = 0; x < list_width; x++)
            {
                for (int y = 0; y < list_height; y++)
                {
                    if (rooms[x][y].Type == 0)
                    {
                        loc.X = x;
                        loc.Y = y;
                        return loc;
                    }
                }
            }
            return loc; // Error. Return -1,-1
        }

        private Vector2 GetBossLocation()
        {
            Vector2 loc = new Vector2(-1, -1);

            for (int x = 0; x < list_width; x++)
            {
                for (int y = 0; y < list_height; y++)
                {
                    if (rooms[x][y].Type == 2)
                    {
                        loc.X = x;
                        loc.Y = y;
                        return loc;
                    }
                }
            }
            return loc; // Error. Return -1,-1
        }

        #endregion

        #region LEVEL GENERATION

        // LEVEL GENERATION ONLY
        // Generate the layout for this floor
        // The spawn and boss don't go towards the room count
        private bool GenerateFloor(int i_rooms)
        {
            // Clear the floor of any existing rooms:
            Clear_Floor();
            
            // Create the spawn room:
            AddRoom(0, 0, 0); // Room at [0][0] with type 0 (spawn room)

            // Create other "filler" rooms
            while (num_normal_rooms < i_rooms)
            {
                // Search for an appropriate location
                // The location must:
                // 1. Be empty
                // 2. Be next to another room
                int new_room_x = Generators.Instance().NextRandom((int)tl_room.X - 1, (int)br_room.X + 1);
                int new_room_y = Generators.Instance().NextRandom((int)tl_room.Y - 1, (int)br_room.Y + 1);

                while (HasAdjacentRoom(new_room_x, new_room_y) == false || RoomExistsAt(new_room_x, new_room_y) == true)
                {
                    new_room_x = Generators.Instance().NextRandom((int)tl_room.X - 1, (int)br_room.X + 1);
                    new_room_y = Generators.Instance().NextRandom((int)tl_room.Y - 1, (int)br_room.Y + 1);
                }

                // Shift the current block of rooms if the new room index is negative
                while (new_room_x < 0)
                {
                    ShiftRoomsRight();
                    new_room_x++;
                }
                while (new_room_y < 0)
                {
                    ShiftRoomsDown();
                    new_room_y++;
                }

                AddRoom(new_room_x, new_room_y, 1); // Add a normal room
                num_normal_rooms++;
            }

            // Add the Boss rooms
            int boss_room_x = Generators.Instance().NextRandom((int)tl_room.X - 1, (int)br_room.X + 1);
            int boss_room_y = Generators.Instance().NextRandom((int)tl_room.Y - 1, (int)br_room.Y + 1);

            while (HasAdjacentRoom(boss_room_x, boss_room_y) == false || RoomExistsAt(boss_room_x, boss_room_y) == true)
            {
                boss_room_x = Generators.Instance().NextRandom((int)tl_room.X - 1, (int)br_room.X + 1);
                boss_room_y = Generators.Instance().NextRandom((int)tl_room.Y - 1, (int)br_room.Y + 1);
            }

            // Shift the current block of rooms if the new room index is negative
            while (boss_room_x < 0)
            {
                ShiftRoomsRight();
                boss_room_x++;
            }
            while (boss_room_y < 0)
            {
                ShiftRoomsDown();
                boss_room_y++;
            }
            AddRoom(boss_room_x, boss_room_y, 2); // Add the boss room

            // TODO: Add any extra key rooms like shops or whatever

            ConnectRooms();

            current_room = GetSpawnLocation();

            return false;
        }

        // LEVEL GENERATION ONLY
        // Clear the floor of all rooms. Resets the floor.
        // Resets all members to their initial state
        private void Clear_Floor()
        {
            rooms.Clear();

            list_width = 0;
            list_height = 0;
            tl_room = new Vector2(0, 0);
            br_room = new Vector2(0, 0);
            num_rooms = 0;
            num_normal_rooms = 0;
            current_room = new Vector2(0, 0);
        }

        // LEVEL GENERATION ONLY
        // Connect each room on the floor
        // Pre: Every room should have at least one adjacent room
        private void ConnectRooms()
        {
            int num_connected_rooms = 0;

            // Connect the boss room to an adjacent room
            Vector2 bossLoc = GetBossLocation();
            if (HasAdjacentRoom((int)bossLoc.X, (int)bossLoc.Y))
            {
                bool connected = false;
                while (connected == false)
                {
                    int randomSide = Generators.Instance().NextRandom(0, 3); // Number from 0-3

                    if (randomSide == 0) // Try to connect to left room
                    {
                        if (RoomExistsAt((int)bossLoc.X - 1, (int)bossLoc.Y)) // Does the room exist?
                        {
                            rooms[(int)bossLoc.X][(int)bossLoc.Y].LeftDoor = true;
                            rooms[(int)bossLoc.X - 1][(int)bossLoc.Y].RightDoor = true;
                            num_connected_rooms += 2;
                            connected = true;
                        }
                    }

                    else if (randomSide == 1) // Try to connect to right room
                    {
                        if (RoomExistsAt((int)bossLoc.X + 1, (int)bossLoc.Y)) // Does the room exist?
                        {
                            rooms[(int)bossLoc.X][(int)bossLoc.Y].RightDoor = true;
                            rooms[(int)bossLoc.X + 1][(int)bossLoc.Y].LeftDoor = true;
                            num_connected_rooms += 2;
                            connected = true;
                        }
                    }

                    else if (randomSide == 2) // Try to connect to upper room
                    {
                        if (RoomExistsAt((int)bossLoc.X, (int)bossLoc.Y - 1)) // Does the room exist?
                        {
                            rooms[(int)bossLoc.X][(int)bossLoc.Y].TopDoor = true;
                            rooms[(int)bossLoc.X][(int)bossLoc.Y - 1].BottomDoor = true;
                            num_connected_rooms += 2;
                            connected = true;
                        }
                    }

                    else if (randomSide == 3) // Try to connect to lower room
                    {
                        if (RoomExistsAt((int)bossLoc.X, (int)bossLoc.Y + 1)) // Does the room exist?
                        {
                            rooms[(int)bossLoc.X][(int)bossLoc.Y].BottomDoor = true;
                            rooms[(int)bossLoc.X][(int)bossLoc.Y + 1].TopDoor = true;
                            num_connected_rooms += 2;
                            connected = true;
                        }
                    }
                }
            }


            // Connect the rest of the rooms
            connectTheRoom:
            while (num_connected_rooms < num_rooms)
            {
                int randomRoomNumber = Generators.Instance().NextRandom(0, num_rooms - num_connected_rooms - 1);

                for (int i = 0; i < list_width; i++)
                {
                    for (int j = 0; j < list_height; j++)
                    {
                        if (rooms[i][j].Exists && rooms[i][j].Is_Connected() == false)
                        {
                            randomRoomNumber--;

                            // Found the random room?
                            if (randomRoomNumber < 0)
                            {
                                if (HasAdjacentConnected(i, j))
                                {
                                    if(ConnectToAdjacent(i, j))
                                        num_connected_rooms++;
                                    goto connectTheRoom;
                                }
                            }
                        }
                    }
                }
            }
        }

        // LEVEL GENERATION ONLY
        // Is Boss Room?
        // Pre: Room must exist
        private bool IsBossRoom(int x, int y)
        {
            if (rooms[x][y].Type == 2)
                return true;
            return false;
        }

        // LEVEL GENERATION ONLY
        // Connect a room to an adjacent room that is not the boss room
        // Pre: Room must have an adjacent room that is connected
        private bool ConnectToAdjacent(int x, int y)
        {
            int randomSide;
            bool connected = false;
            int counter = 0;

            while (connected == false && counter < 10)
            {
                randomSide = Generators.Instance().NextRandom(0, 3); // Number from 0-3

                if (randomSide == 0) // Try to connect to left room
                {
                    if (RoomExistsAt(x - 1, y) && rooms[x - 1][y].Is_Connected() && !IsBossRoom(x - 1, y)) // Does the room exist?
                    {
                        rooms[x][y].LeftDoor = true;
                        rooms[x - 1][y].RightDoor = true;
                        return true;
                    }
                }

                else if (randomSide == 1) // Try to connect to right room
                {
                    if (RoomExistsAt(x + 1, y) && rooms[x + 1][y].Is_Connected() && !IsBossRoom(x + 1, y)) // Does the room exist?
                    {
                        rooms[x][y].RightDoor = true;
                        rooms[x + 1][y].LeftDoor = true;
                        return true;
                    }
                }

                else if (randomSide == 2) // Try to connect to upper room
                {
                    if (RoomExistsAt(x, y - 1) && rooms[x][y - 1].Is_Connected() && !IsBossRoom(x, y - 1)) // Does the room exist?
                    {
                        rooms[x][y].TopDoor = true;
                        rooms[x][y - 1].BottomDoor = true;
                        return true;
                    }
                }

                else if (randomSide == 3) // Try to connect to lower room
                {
                    if (RoomExistsAt(x, y + 1) && rooms[x][y + 1].Is_Connected() && !IsBossRoom(x, y + 1)) // Does the room exist?
                    {
                        rooms[x][y].BottomDoor = true;
                        rooms[x][y + 1].TopDoor = true;
                        return true;
                    }
                }
                counter++;
            }
            return false;
        }

        // LEVEL GENERATION ONLY
        // Returns if the given room has an adjacent room with doors
        // Pre: Room must exist
        private bool HasAdjacentConnected(int x, int y)
        {
            if (HasAdjacentRoom(x, y))
            {
                // Left
                if (RoomExistsAt(x - 1, y))
                {
                    if (rooms[x - 1][y].Is_Connected())
                        return true;
                }
                // Right
                if (RoomExistsAt(x + 1, y))
                {
                    if (rooms[x + 1][y].Is_Connected())
                        return true;
                }
                // Top
                if (RoomExistsAt(x, y - 1))
                {
                    if (rooms[x][y - 1].Is_Connected())
                        return true;
                }
                // Bottom
                if (RoomExistsAt(x, y + 1))
                {
                    if (rooms[x][y + 1].Is_Connected())
                        return true;
                }
            }

            return false;
        }

        // LEVEL GENERATION ONLY
        // Add a new room in the desired location
        // WILL OVERWRITE A ROOM
        private void AddRoom(int room_x, int room_y, int i_type)
        {
            // Create the new room
            Room added_room = new Room(i_type);

            // Expand the floor if needed
            if (list_width <= room_x)
                AddColumn(room_x);
            if (list_height <= room_y)
                AddRow(room_y);

            // Add the room to the desired location
            rooms[room_x][room_y] = added_room;
            rooms[room_x][room_y].Exists = true;

            // Update the top left and bottom right corners of the room box
            UpdateCorners();

            num_rooms++;
        }

        // LEVEL GENERATION ONLY
        // Update both corner vectors        
        private void UpdateCorners()
        {
            UpdateTLCorner();
            UpdateBRCorner();
        }

        // LEVEL GENERATION ONLY
        // Update the TL corner vector
        private void UpdateTLCorner()
        {
            int most_left = 0;

            // Find the left-most room
            for (int x = 0; x < list_width; x++)
            {
                for (int y = 0; y < list_height; y++)
                {
                    if (rooms[x][y].Exists)
                    {
                        most_left = x;
                        goto findtop;
                    }
                }
            }

            // Find the top-most room
            findtop:
            for (int y = 0; y < list_height; y++)
            {
                for (int x = 0; x < list_width; x++)
                {
                    if (rooms[x][y].Exists)
                    {
                        tl_room.X = most_left;
                        tl_room.Y = y;
                        return;
                    }
                }
            }
        }

        // LEVEL GENERATION ONLY
        // Update the BR corner vector
        private void UpdateBRCorner()
        {
            int most_right = 0;

            // Find the right-most room
            for (int x = list_width - 1; x >= 0; x--)
            {
                for (int y = list_height - 1; y >= 0; y--)
                {
                    if (rooms[x][y].Exists)
                    {
                        most_right = x;
                        goto findbottom;
                    }
                }
            }

            // Find the bottom-most room
            findbottom:
            for (int y = list_height - 1; y >= 0; y--)
            {
                for (int x = list_width - 1; x >= 0; x--)
                {
                    if (rooms[x][y].Exists)
                    {
                        br_room.X = most_right;
                        br_room.Y = y;
                        return;
                    }
                }
            }
        }

        // Determines if a room exists at the given index
        // Pre: none
        public bool RoomExistsAt(int x, int y)
        {
            // Out of bounds
            if (x < 0 || y < 0 || x >= list_width || y >= list_height)
                return false;
            // In bounds. Check the room
            if (rooms[x][y].Exists)
                return true;
            return false;
        }

        // LEVEL GENERATION ONLY
        // Check if the given index has one or more adjacent rooms that exist
        // Pre: x and y must be between -1 and the list size. The space cannot be occupied
        private bool HasAdjacentRoom(int x, int y)
        {
            // Outside of room block. Impossible to have adjacent rooms.
            if (x < tl_room.X - 1 || x > br_room.X + 1 || y < tl_room.Y - 1 || y > br_room.Y + 1)
                return false;

            // Left side outside room block
            if (x < tl_room.X)
            {
                // TL or BL Corner
                if (y < tl_room.Y || y > br_room.Y)
                    return false;

                // On the side. Check right room.
                if (rooms[x + 1][y].Exists)
                    return true;
                else return false;
            }
            // Right side outside room block
            if (x > br_room.X)
            {
                // TR or BR Corner
                if (y < tl_room.Y || y > br_room.Y)
                    return false;

                // On the side. Check left room.
                if (rooms[x - 1][y].Exists)
                    return true;
                else return false;
            }
            // Top side outside room block
            if (y < tl_room.Y)
            {
                // On the side. Check bottom room.
                if (rooms[x][y + 1].Exists)
                    return true;
                else return false;
            }
            // Bottom side outside room block
            if (y > br_room.Y)
            {
                // On the side. Check top room.
                if (rooms[x][y - 1].Exists)
                    return true;
                else return false;
            }

            // Left
            if (x - 1 >= 0)
                if (rooms[x - 1][y].Exists)
                    return true;
            // Right
            if (x + 1 < list_width)
                if (rooms[x + 1][y].Exists)
                    return true;
            // Up
            if (y - 1 >= 0)
                if (rooms[x][y - 1].Exists)
                    return true;
            // Down
            if (y + 1 < list_height)
                if (rooms[x][y + 1].Exists)
                    return true;

            return false;
        }

        // LEVEL GENERATION ONLY
        // Add empty columns to fill the floor up to the desired index
        // A column is vertical at the desired x index
        private void AddColumn(int x_index)
        {
            // Add a new column until the desired column index is reached
            for (int i = list_width; i <= x_index; i++)
            {
                rooms.Add(new List<Room>());
            }
            list_width = x_index + 1;

            for (int i = 0; i < list_width; i++)
            {
                while (rooms[i].Count() < list_height)
                    rooms[i].Add(new Room());
            }
        }

        // LEVEL GENERATION ONLY
        // Add empty rows to fill the floor up to the desired index
        // A column is vertical at the desired x index
        private void AddRow(int y_index)
        {
            // Go through each column
            for (int i = 0; i < list_width; i++)
            {
                // Add a new room until the desired row index is reached
                for (int j = list_height; j <= y_index; j++)
                {
                    rooms[i].Add(new Room());
                }
            }
            list_height = y_index + 1;
        }

        // LEVEL GENERATION ONLY
        // Shifts each column of rooms to the right by 1 index
        // Post: Rooms in the left-most column no longer exist
        private void ShiftRoomsRight()
        {
            // Insert a new column of rooms to the left
            rooms.Insert(0, new List<Room>());

            // Fill the column with empty rooms
            for (int i = 0; i < list_height; i++)
            {
                rooms[0].Add(new Room());
            }

            list_width++;

            // Shift the room block indexes over
            UpdateCorners();
        }

        // LEVEL GENERATION ONLY
        // Shifts row of rooms down by 1 index
        // Post: Rooms in the top row no longer exist
        private void ShiftRoomsDown()
        {
            // Insert a new room to the start of each column
            for (int x = 0; x < list_width; x++)
            {
                rooms[x].Insert(0, new Room());
            }

            list_height++;

            // Shift the room block indexes down one
            UpdateCorners();
        }

        #endregion

        #region Properties

        public int NumNormalRooms
        {
            get { return num_normal_rooms; }
        }
        
        // Get the room at index x, y
        // Pre: room must exist
        public Room Get_Room(int x, int y)
        {
            return rooms[x][y];
        }
        
        public Room CurrentRoom
        {
            get { return rooms[(int)current_room.X][(int)current_room.Y]; }
        }

        public Vector2 CurrentRoomPosition
        {
            get { return current_room; }
        }

        public int Width
        {
            get { return (int)br_room.X - (int)tl_room.X + 1; }
        }

        public int Height
        {
            get { return (int)br_room.Y - (int)tl_room.Y + 1; }
        }

        public int Get_Room_Type(int x, int y)
        {
            if(x < list_width && y < list_height)
                return rooms[x][y].Type;
            return -1;
        }

        #endregion
    }
}