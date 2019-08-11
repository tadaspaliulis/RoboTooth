using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Mapping
{
    /// <summary>
    /// Contains the current knowledge about the
    /// world map
    /// </summary>
    public class Map
    {
        public List<Wall> _walls = new List<Wall>();
    }

    public class Wall
    {
        public Vector2 Position { get; /*private*/ set; }

        /// <summary>
        /// How far the wall extends from its 'central'
        /// position to the left, where left is determined with
        /// the help of the FaceNormal.
        /// </summary>
        public float LengthLeft { get; /*private*/ set; }

        /// <summary>
        /// How far the wall extends from its 'central'
        /// position to the right, where right is determined with
        /// the help of the FaceNormal.
        /// </summary>
        public float LengthRight { get; /*private*/ set; }

        /// <summary>
        /// The orientation of the wall.
        /// </summary>
        public Vector2 FaceNormal { get; /*private*/set; }

        public event EventHandler WallUpdated;
    }
}
