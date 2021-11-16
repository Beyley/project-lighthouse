#nullable enable
using System.Collections.Generic;
using System.Linq;
using LBPUnion.ProjectLighthouse.Types;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Match;

namespace LBPUnion.ProjectLighthouse.Helpers
{
    public class RoomHelper
    {
        public static readonly List<Room> Rooms = new();

        public static readonly RoomSlot PodSlot = new()
        {
            SlotType = SlotType.Pod,
            SlotId = 0,
        };

        public static FindBestRoomResponse? FindBestRoom(User user, string location)
        {
            bool anyRoomsLookingForPlayers = Rooms.Any(r => r.IsLookingForPlayers);

            // Look for rooms looking for players before moving on to rooms that are idle.
            foreach (Room room in Rooms.Where(r => !anyRoomsLookingForPlayers || r.IsLookingForPlayers))
            {
                if (MatchHelper.DidUserRecentlyDiveInWith(user.UserId, room.Players[0].UserId)) continue;

                Dictionary<int, string> relevantUserLocations = new();

                // Determine if all players in a room have UserLocations stored, also store the relevant userlocations while we're at it
                bool allPlayersHaveLocations = room.Players.All
                (
                    p =>
                    {
                        bool gotValue = MatchHelper.UserLocations.TryGetValue(p.UserId, out string? value) && value != null;

                        if (gotValue) relevantUserLocations.Add(p.UserId, value!);
                        return gotValue;
                    }
                );

                // If we don't have all locations then the game won't know how to communicate. Thus, it's not a valid room.
                if (!allPlayersHaveLocations) continue;

                // If we got here then it should be a valid room.

                FindBestRoomResponse response = new();

                response.Players = new List<Player>();
                foreach (User player in room.Players)
                {
                    response.Players.Add
                    (
                        new Player
                        {
                            MatchingRes = 0,
                            User = player,
                        }
                    );

                    response.Locations.Add(relevantUserLocations.GetValueOrDefault(player.UserId)); // Already validated to exist
                }

                response.Players.Add
                (
                    new Player
                    {
                        MatchingRes = 1,
                        User = user,
                    }
                );

                response.Locations.Add(location);

                response.Slots = new List<List<int>>
                {
                    new()
                    {
                        (int)room.Slot.SlotType,
                        room.Slot.SlotId,
                    },
                };

                return response;
            }

            return null;
        }

        public static Room CreateRoom(User user, RoomSlot? slot = null)
            => CreateRoom
            (
                new List<User>
                {
                    user
                },
                slot
            );
        public static Room CreateRoom(List<User> users, RoomSlot? slot = null)
        {
            Room room = new();

            room.Players = users;
            room.State = RoomState.Idle;
            room.Slot = slot ?? PodSlot;

            Rooms.Add(room);
            return room;
        }

        public static Room? FindRoomByUser(User user) => Rooms.FirstOrDefault(r => r.Players.Contains(user));
    }
}