using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    /// <summary>
    /// Walls. Note that floors and ceilings are Walls.
    /// </summary>
    class Wall : Entity
    {
        /// <summary>
        /// Dimensions of the wall.
        /// </summary>
        public Vector2 Dimensions;

        /// <summary>
        /// Constructor. Empty Wall constructors are BANNED. Get dunked on.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dimensions"></param>
        public Wall(Vector2 position, Vector2 dimensions)
        {
            this.Position = position;
            this.Dimensions = dimensions;
            this.Hitbox = new Hitbox(position, dimensions);
        }

        /// <summary>
        /// Do nothing. Walls don't move.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="push"></param>
        public override void Interact(Entity entity, Vector2 push)
        {
            //throw new NotImplementedException();
        }

        public override void Render()
        {
            Vector2 renderPos = Utils.Renderable(Position);
            Vector2 renderDims = Utils.Renderable(Dimensions);
            Raylib.DrawRectangle(
                (int)renderPos.X, (int)renderPos.Y,
                (int)renderDims.X, (int)renderDims.Y,
                Color.GRAY);
        }
    }
}
