
using UnityEngine;

namespace RotatedBoundingVolume
{
    struct RBB
    {
        public RBB(Transform parent, Bounds bounds)
        {
            this.parent = parent;
            this.localBounds = bounds;
        }
        Transform parent;
        Bounds localBounds;
        public Vector3 center { get { return parent.position; } }
        public Vector3 Min { get { return ToWorldPosition(localBounds.min); } }
        public Vector3 Max { get { return ToWorldPosition(localBounds.max); } }

        /// <summary>
        /// Gets the world position of one of the vertices in the rotated bounding volume.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetVertex(int index)
        {
            switch (index)
            {
                case 0://bottom left lower
                    return Min;
                case 1://top left lower
                    return ToWorldPosition(localBounds.min + new Vector3(0, 0, localBounds.size.z));
                case 2://top right lower
                    return ToWorldPosition(localBounds.min + new Vector3(localBounds.size.x, 0, localBounds.size.z));
                case 3://bottom right lower
                    return ToWorldPosition(localBounds.min + new Vector3(localBounds.size.x, 0, 0));
                case 4://bottom left upper
                    return ToWorldPosition(localBounds.min + new Vector3(0, localBounds.size.y, 0));
                case 5: //top left upper
                    return ToWorldPosition(localBounds.min + new Vector3(0, localBounds.size.y, localBounds.size.z));
                case 6: //top right upper
                    return ToWorldPosition(localBounds.min + new Vector3(localBounds.size.x, localBounds.size.y, localBounds.size.z));
                case 7://bottom right upper
                    return ToWorldPosition(localBounds.min + new Vector3(localBounds.size.x, localBounds.size.y, 0));
            }
            return Vector3.zero;
        } 
        Vector3 ToWorldPosition(Vector3 pos) => (parent.rotation * (pos - parent.position)) + parent.position;
        /// <summary>
        /// Used to draw bounds in the OnDrawGizmos method.
        /// </summary>
        public void DrawBounds()
        {
            int[] cubeEdges = { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 1, 5, 2, 6, 3, 7, 0, 4 };
            for (int i = 0; i < cubeEdges.Length; i += 2)
                Gizmos.DrawLine(GetVertex(cubeEdges[i]), GetVertex(cubeEdges[i + 1]));
        }
    }
}
