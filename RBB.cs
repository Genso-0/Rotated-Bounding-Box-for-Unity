using UnityEngine;

namespace RotatedBoundingVolume
{
    /// <summary>
    /// A rotated bounding box. It uses a local axis aligned bounding box as reference.
    /// </summary>
    public static class RBB
    {
        static readonly int[] cubeEdges = { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 1, 5, 2, 6, 3, 7, 0, 4 };
        static Vector3[] buffer1 = new Vector3[8];
        static Vector3[] buffer2 = new Vector3[8];
    
        static void SetVerts(ref Vector3[] buffer, in Bounds bounds, in Vector3 offset, in Quaternion rotation)
        {
            var max = bounds.size * 0.5f;
            var min = -max;
            buffer[0] = offset + (rotation * min);
            buffer[1] = offset + (rotation * new Vector3(max.x, min.y, min.z));
            buffer[2] = offset + (rotation * new Vector3(max.x, min.y, max.z));
            buffer[3] = offset + (rotation * new Vector3(min.x, min.y, max.z));
            buffer[4] = offset + (rotation * new Vector3(min.x, max.y, min.z));
            buffer[5] = offset + (rotation * new Vector3(max.x, max.y, min.z));
            buffer[6] = offset + (rotation * max);
            buffer[7] = offset + (rotation * new Vector3(min.x, max.y, max.z));
        }
        #region intersection
        /// <summary>
        ///  Checks to see if two rotated bounded boxes intersect.
        /// </summary>
        /// <param name="firstBounds"></param>
        /// <param name="secondBounds"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool Intersect(Bounds firstBounds, Bounds secondBounds, Transform first, Transform second)
        {
            return Intersects(firstBounds, first.position, first.rotation, secondBounds, second.position, second.rotation);
        }
        //https://gamedev.stackexchange.com/questions/44500/how-many-and-which-axes-to-use-for-3d-obb-collision-with-sat
        /// <summary>
        /// Checks to see if two rotated bounded boxes intersect.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="firstOffset"></param>
        /// <param name="firstRot"></param>
        /// <param name="second"></param>
        /// <param name="secondOffset"></param>
        /// <param name="secondRot"></param>
        /// <returns></returns>
        public static bool Intersects(in Bounds first, in Vector3 firstOffset, in Quaternion firstRot, in Bounds second, in Vector3 secondOffset, in Quaternion secondRot)
        {
            SetVerts(ref buffer1, first, firstOffset, firstRot);
            SetVerts(ref buffer2, second, secondOffset, secondRot);
            Vector3 aRight = firstRot * Vector3.right;
            Vector3 bRight = secondRot * Vector3.right;
            Vector3 aForward = firstRot * Vector3.forward;
            Vector3 bForward = secondRot * Vector3.forward;
            Vector3 aUp = firstRot * Vector3.up;
            Vector3 bUp = secondRot * Vector3.up;
            if (Separated(ref buffer1, ref buffer2, aRight))
                return false;
            if (Separated(ref buffer1, ref buffer2, aUp))
                return false;
            if (Separated(ref buffer1, ref buffer2, aForward))
                return false;

            if (Separated(ref buffer1, ref buffer2, bRight))
                return false;
            if (Separated(ref buffer1, ref buffer2, bUp))
                return false;
            if (Separated(ref buffer1, ref buffer2, bForward))
                return false;

            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aRight, bRight)))
                return false;
            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aRight, bUp)))
                return false;
            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aRight, bForward)))
                return false;

            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aUp, bRight)))
                return false;
            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aUp, bUp)))
                return false;
            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aUp, bForward)))
                return false;

            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aForward, bRight)))
                return false;
            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aForward, bUp)))
                return false;
            if (Separated(ref buffer1, ref buffer2, Vector3.Cross(aForward, bForward)))
                return false;

            return true;
        }
        static bool Separated(ref Vector3[] vertsA, ref Vector3[] vertsB, in Vector3 axis)
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
        /// Gets the corners suround the bounding volume with an offset equal to half the size of the perimeterRBB.
        /// Think of it as placing smaller boxes at the corners of a bigger box.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="rbb"></param>
        /// <param name="edgeSize"></param>
        /// <returns></returns>
        public static Vector3[] GetBoundCorners(in Bounds bounds, in Vector3 offset, in Quaternion rot, in Vector3 edgeSize)
        {
            var halfSizeY = edgeSize.y * .51f;
            var halfSizeX = edgeSize.x * .51f;
            var halfSizeZ = edgeSize.z * .51f;
            SetVerts(ref buffer1, bounds, offset, rot);
            return new Vector3[8]
            {
            //lower corners
             buffer1[0] + (rot * new Vector3(-halfSizeX, -halfSizeY, -halfSizeZ)),//back left lower corner
             buffer1[1] + (rot * new Vector3(halfSizeX, -halfSizeY, -halfSizeZ)),//back right lower corner
             buffer1[2] + (rot * new Vector3(halfSizeX, -halfSizeY, halfSizeZ)),//foward right lower corner
             buffer1[3] + (rot * new Vector3(-halfSizeX, -halfSizeY, halfSizeZ)),//forward left lower corner

            //upper corners
             buffer1[4] + (rot * new Vector3(-halfSizeX, halfSizeY, -halfSizeZ)),//back left upper corner
             buffer1[5] + (rot * new Vector3(halfSizeX, halfSizeY, -halfSizeZ)),//back right upper corner
             buffer1[6] + (rot * new Vector3(halfSizeX, halfSizeY, halfSizeZ)),//forward right upper corner
             buffer1[7] + (rot * new Vector3(-halfSizeX, halfSizeY, halfSizeZ)),//forward left upper corner
            };
        }
        /// <summary>
        /// Used to draw bounds in the OnDrawGizmos method.
        /// </summary>
        public static void DrawBounds(in Bounds bounds, in Vector3 offset, in Quaternion rot, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(offset, 0.1f);
            SetVerts(ref buffer1, bounds, offset, rot);

            for (int i = 0; i < cubeEdges.Length; i += 2)
            {
                //Gizmos.color = GetVertexColor(cubeEdges[i]);
                //Gizmos.DrawSphere(verts[cubeEdges[i]], .1f);
                Gizmos.color = color;
                Gizmos.DrawLine(buffer1[cubeEdges[i]], buffer1[cubeEdges[i + 1]]);
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
