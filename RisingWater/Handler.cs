using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    class Handler
    {
        /// <summary>
        /// Contains all entities. 
        /// Each frame, each entity gets Ticked.
        /// Each render frame, each entity gets Rendered.
        /// </summary>
        public LinkedList<Entity> entities;

        /// <summary>
        /// Handler constructor.
        /// </summary>
        public Handler()
        {
            entities = new();
        }

        /// <summary>
        /// Each frame, each entity is ticked.
        /// Afterwards, each hitbox is checked against each other.
        /// complexity is O(sum(N))
        /// Number of iterations is N + sum(N)
        /// </summary>
        public void Tick()
        {
            //entity tick
            foreach(Entity entity in entities)
            {
                entity.Tick();
            }

            TickHitboxes();
        }

        /// <summary>
        /// Updates hitboxes. 
        /// Modified manual 2D LinkedList foreach that reduces complexity from O(n^2) to O(sum(n))
        /// </summary>
        void TickHitboxes()
        {
            
            //hitboxes update
            //modified manual foreach that reduces complexity from O(n^2) to O(sum(n))
            LinkedListNode<Entity> nodeI = entities.First;
            while (nodeI != null && nodeI.Next != null)
            {
                LinkedListNode<Entity> nodeJ = nodeI.Next;

                do
                {
                    nodeI.Value.Hitbox.Intersects(nodeJ.Value.Hitbox, (Vector2 push) =>
                    {
                        
                        nodeI.Value.Interact(nodeJ.Value, push);
                        nodeJ.Value.Interact(nodeI.Value, -push);
                    });
                    nodeJ = nodeJ.Next;
                } while (nodeJ != null);
                nodeI = nodeI.Next;
            }
        }

        /// <summary>
        /// Renders each entity.
        /// </summary>
        public void Render()
        {
            foreach(Entity entity in entities)
            {
                entity.Render();
            }    
        }

        /// <summary>
        /// Adds an entity to the entities list.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The added entity</returns>
        public Entity AddEntity(Entity entity)
        {
            entities.AddLast(entity);

            return entity;
        }

        /// <summary>
        /// Removes an entity from the entities list. 
        /// Warning: Complexity O(n)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The removed entity, or null if it fails</returns>
        public Entity RemoveEntity(Entity entity)
        {
            if(entities.Remove(entity))
            {
                return entity;
            }
            return null;
        }
    }
}
