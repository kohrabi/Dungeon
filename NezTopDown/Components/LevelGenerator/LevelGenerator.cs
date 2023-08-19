using LDtk;
using LDtkTypes;
using LDtkTypes;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace NezTopDown.Components.LevelGenerator
{

    public class LevelGenerator : Component
    {
        public int MaxRooms = 8;
        public Vector2 StartPos;
        private FastList<Entrance> availablePaths = new FastList<Entrance>();
        Dictionary<Entrances, FastList<Guid>> _availableEntrances = new();

        int roomCount = 0;
        int minRoom = 4;
        LDtkWorld _rooms;
        LDtkFile _file;
        LDtkManager _renderer;

        public LevelGenerator(LDtkFile file, Guid world, Vector2 startPoint)
        {
            _file = file;
            _rooms = _file.LoadWorld(world);
            // Putting all Entrances in a Dictionary.
            // This makes sure we always choose the right entrance Connection
            foreach (Entrances entrance in Enum.GetValues(typeof(Entrances))) 
                _availableEntrances.Add(entrance, new FastList<Guid>());

            foreach (var level in _rooms.Levels)
            {
                foreach(var entity in level.GetEntities<Entrance>())
                {
                    _availableEntrances[entity.Entrances].Add(level.Iid);
                }
            }

            StartPos = startPoint;
        }

        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();
            availablePaths.Clear();
            _availableEntrances.Clear();
            _rooms = null;
            _file = null;
            _renderer = null;
        }

        public override void OnAddedToEntity()
        {
            _renderer = Entity.AddComponent(new LDtkManager(_file, _rooms.Iid));
            _file = null;
            CreateFirstRoom(StartPos);
            roomCount = 1;
            while (roomCount <= MaxRooms && availablePaths.Length != 0)
            {
                Entrance nextEntrance = availablePaths.Buffer[Nez.Random.Range(0, availablePaths.Length)];
                if (AttemptCreateRoom(nextEntrance))
                {
                    roomCount++;
                //AttemptCreateCorridor(nextEntrance);
                }
                //if (roomCount >= minRoom)
                availablePaths.Remove(nextEntrance);
            }

            foreach (var colliderCheck in Entity.Scene.FindEntitiesWithTag(1000))
                colliderCheck.Destroy();
            _rooms = null;
            availablePaths.Clear();
            _availableEntrances.Clear();
            base.OnAddedToEntity();
        }

        private bool AttemptCreateRoom(Entrance pat)
        {
            Entrances levelEntrance = GetOppositeEntrance(pat.Entrances);
            Guid levelIid = _availableEntrances[levelEntrance]
                                .Buffer[Nez.Random.Range(0, _availableEntrances[levelEntrance].Length)];
            LDtkLevel level = _rooms.LoadLevel(levelIid);

            Entrance entrance = pat;
            foreach (var e in level.GetEntities<Entrance>())
            {
                if (e.Entrances == levelEntrance)
                {
                    entrance = e;
                    break;
                }
            }
            Vector2 position = pat.Position - (entrance.Position - level.Position.ToVector2());
            if (levelEntrance == Entrances.Right)
                position += new Vector2(-16f, 0);
            if (levelEntrance == Entrances.Bottom)
                position += new Vector2(0, -16f);
            if (levelEntrance == Entrances.Left)
                position += new Vector2(16f, 0);
            if (levelEntrance == Entrances.Top)
                position += new Vector2(0, 16f);
            var entity = Entity.Scene.CreateEntity("LevelColliderCheck");
            entity.Tag = 1000;
            entity.SetParent(Entity);
            entity.Position = position + level.Size.ToVector2() / 2;
            var collider = entity.AddComponent(new BoxCollider(level.PxWid, level.PxHei));

            Physics.AddCollider(collider);
            
            if (collider.CollidesWithAny(out var res))
            {
                entity.Destroy();
                return false;
            }

            _renderer.AddLevelToRender(levelIid, position);
            foreach (var e in level.GetEntities<Entrance>())
            {
                e.Position += position - level.Position.ToVector2();
                availablePaths.Add(e);
            }

            return true;
            
        }

        Entrances GetOppositeEntrance(Entrances e)
        {
            if (e == Entrances.Left)
                return Entrances.Right;
            if (e == Entrances.Right)
                return Entrances.Left;
            if (e == Entrances.Bottom)
                return Entrances.Top;
            if (e == Entrances.Top)
                return Entrances.Bottom;
            Console.Error.WriteLine("This is illegal, i have already cover every entrances");
            return Entrances.Left;
        }

        private bool CheckEntranceOverlaps(Entrance e1, Entrance e2)
        {
            if (e1.Entrances == Entrances.Left && e2.Entrances == Entrances.Right) 
                return true;
            if (e1.Entrances == Entrances.Right && e2.Entrances == Entrances.Left)
                return true;
            if (e1.Entrances == Entrances.Top && e2.Entrances == Entrances.Bottom)
                return true;
            if (e1.Entrances == Entrances.Bottom && e2.Entrances == Entrances.Top)
                return true;
            return false;
        }

        private void CreateFirstRoom(Vector2 startPos)
        {
            var level = _rooms.Levels.ElementAt(Nez.Random.Range(0, _rooms.Levels.Count()));

            _renderer.AddLevelToRender(level, startPos);

            var colliderCheck = Entity.Scene.CreateEntity("LevelColliderCheck");
            colliderCheck.Tag = 1000;
            colliderCheck.SetParent(Entity);
            colliderCheck.Position = startPos + level.Size.ToVector2() / 2;
            var collider = colliderCheck.AddComponent(new BoxCollider(level.PxWid, level.PxHei));
            Physics.AddCollider(collider);


            foreach (var e in level.GetEntities<Entrance>())
            {
                e.Position += (startPos - level.Position.ToVector2());
                //if (entity.Entrances == Entrances.Right)
                //    entity.Position += new Vector2(16f, 0);
                //if (entity.Entrances == Entrances.Bottom)
                //    entity.Position += new Vector2(0, 16f);
                availablePaths.Add(e);
            }
        }

        //private bool AttemptCreateCorridor(Entrance entrance)
        //{
        //    var room = Entity.Scene.CreateEntity("room");
        //    var width = Nez.Random.Range(3, 5) * 10f;
        //    var height = Nez.Random.Range(3, 5) * 10f;
        //    Vector2 newPos = Vector2.Zero;
        //    if (entrance.path == AvailablePath.Up)
        //        newPos = new Vector2(0, -height / 2);
        //    if (entrance.path == AvailablePath.Down)
        //        newPos = new Vector2(0, height / 2);
        //    if (entrance.path == AvailablePath.Right)
        //        newPos = new Vector2(width / 2, 0);
        //    if (entrance.path == AvailablePath.Left)
        //        newPos = new Vector2(-width / 2, 0);
        //    room.Position = entrance.position + newPos;
        //    room.AddComponent(new PrototypeSpriteRenderer(width, height));
        //    var collider = room.AddComponent(new BoxCollider(width, height));


        //    if (entrance.path == AvailablePath.Up)
        //        availablePaths.Add(new Entrance(room.Position + new Vector2(0, -height / 2), AvailablePath.Up));
        //    if (entrance.path == AvailablePath.Down)
        //        availablePaths.Add(new Entrance(room.Position + new Vector2(0, height / 2), AvailablePath.Down));
        //    if (entrance.path == AvailablePath.Left)
        //        availablePaths.Add(new Entrance(room.Position + new Vector2(-width / 2, 0), AvailablePath.Left));
        //    if (entrance.path == AvailablePath.Right)
        //        availablePaths.Add(new Entrance(room.Position + new Vector2(width / 2, 0), AvailablePath.Right));
        //    return true;

            
        //}

    }
}
