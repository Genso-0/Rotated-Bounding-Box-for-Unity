using UnityEngine;

namespace RotatedBoundingVolume
{
    /// <summary>
    /// A rotated bounding box. It uses a local axis aligned bounding box as reference.
    /// </summary>
    public struct RBB
    {
        static readonly int[] cubeEdges = { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 1, 5, 2, 6, 3, 7, 0, 4 };
        public RBB(Vector3 offset, Quaternion rotation, Bounds bounds)
        {
            this.localBounds = bounds;
            OffSet =   offset; 
            Rotation = rotation; 
        }
        readonly Bounds localBounds; 
        public Vector3 OffSet;
        public Quaternion Rotation; 
        public Vector3 Center_WorldSpace { get { return (Rotation * localBounds.center) + OffSet; } }
        public Vector3 Size { get { return localBounds.size; } }
        public Vector3 Extents { get { return localBounds.extents; } } 
        
        public Vector3[] GetVertices()
        {
            var max = localBounds.size / 2;
            var min = -max;
            var center = Center_WorldSpace;
            var rot = Rotation;
            return new Vector3[8]
            {
                center + rot * min,
                center + rot * new Vector3(max.x, min.y, min.z),
                center + rot * new Vector3(max.x, min.y, max.z),
                center + rot * new Vector3(min.x, min.y, max.z),
                center + rot * new Vector3(min.x, max.y, min.z),
                center + rot * new Vector3(max.x, max.y, min.z),
                center + rot * max,
                center + rot * new Vector3(min.x, max.y, max.z),
           };
        } 
        #region intersection
        //https://gamedev.stackexchange.com/questions/44500/how-many-and-which-axes-to-use-for-3d-obb-collision-with-sat
        public static bool Intersects(RBB a, RBB b)
        {
            Vector3[] aVertices = a.GetVertices();
            Vector3[] bVertices = b.GetVertices();
            Vector3 aRight = a.Rotation * Vector3.right;
            Vector3 bRight = b.Rotation * Vector3.right;
            Vector3 aForward = a.Rotation * Vector3.forward;
            Vector3 bForward = b.Rotation * Vector3.forward;
            Vector3 aUp = a.Rotation * Vector3.up;
            Vector3 bUp = b.Rotation * Vector3.up;
            if (Separated(aVertices, bVertices, aRight))
                return false;
            if (Separated(aVertices, bVertices, aUp))
                return false;
            if (Separated(aVertices, bVertices, aForward))
                return false;

            if (Separated(aVertices, bVertices, bRight))
                return false;
            if (Separated(aVertices, bVertices, bUp))
                return false;
            if (Separated(aVertices, bVertices, bForward))
                return false;

            if (Separated(aVertices, bVertices, Vector3.Cross(aRight, bRight)))
                return false;
            if (Separated(aVertices, bVertices, Vector3.Cross(aRight, bUp)))
                return false;
            if (Separated(aVertices, bVertices, Vector3.Cross(aRight, bForward)))
                return false;

            if (Separated(aVertices, bVertices, Vector3.Cross(aUp, bRight)))
                return false;
            if (Separated(aVertices, bVertices, Vector3.Cross(aUp, bUp)))
                return false;
            if (Separated(aVertices, bVertices, Vector3.Cross(aUp, bForward)))
                return false;

            if (Separated(aVertices, bVertices, Vector3.Cross(aForward, bRight)))
                return false;
            if (Separated(aVertices, bVertices, Vector3.Cross(aForward, bUp)))
                return false;
            if (Separated(aVertices, bVertices, Vector3.Cross(aForward, bForward)))
                return false;

            return true;
        }
        static bool Separated(Vector3[] vertsA, Vector3[] vertsB, Vector3 axis)
        {
            // Handles the cross product = {0,0,0} case
            if (axis == Vector3.zero)
                return false;

            var aMin = float.MaxValue;
            var aMax = float.MinValue;
            var bMin = float.MaxValue;
            var bMax = float.MinValue;

            // Define two intervals, a and b. Calculate their min and max values
            for (var i = 0; i < 8; i++)
            {
                var aDist = Vector3.Dot(vertsA[i], axis);
                aMin = aDist < aMin ? aDist : aMin;
                aMax = aDist > aMax ? aDist : aMax;
                var bDist = Vector3.Dot(vertsB[i], axis);
                bMin = bDist < bMin ? bDist : bMin;
                bMax = bDist > bMax ? bDist : bMax;
            }

            // One-dimensional intersection test between a and b
            var longSpan = Mathf.Max(aMax, bMax) - Mathf.Min(aMin, bMin);
            var sumSpan = aMax - aMin + bMax - bMin;
            return longSpan >= sumSpan; // > to treat touching as intersection
        }
        #endregion

        /// <summary>
        /// Used to draw bounds in the OnDrawGizmos method.
        /// </summary>
        public void DrawBounds(Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(Center_WorldSpace, 0.1f);
            var verts = GetVertices();
           

            for (int i = 0; i < cubeEdges.Length; i += 2)
            {
                //Gizmos.color = GetVertexColor(cubeEdges[i]);
                //Gizmos.DrawSphere(verts[cubeEdges[i]], .1f);
                Gizmos.color = color;
                Gizmos.DrawLine(verts[cubeEdges[i]], verts[cubeEdges[i + 1]]);
            }
            Gizmos.color = Color.white;
        }
        //public Color GetVertexColor(int index)
        //{
        //    switch (index)
        //    {
        //        case 0://back left lower
        //            return Color.blue;
        //        case 1://back right lower
        //            return Color.yellow;
        //        case 2://top right lower
        //            return Color.red;
        //        case 3://forward left lower
        //            return Color.magenta;
        //        case 4://back left upper
        //            return Color.green;
        //        case 5: //back right upper
        //            return Color.cyan;
        //        case 6: //top right upper
        //            return Color.black;
        //        case 7://forward left upper
        //            return Color.grey;
        //    }
        //    return Color.white;
        //}
    }
}
