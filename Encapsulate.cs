using UnityEngine;

namespace RotatedBoundingVolume
{
    /// <summary>
    /// Finds the collective bounds of all mesh renderers in an objects hierarchy.
    /// </summary>
    public static class Encapsulate
    {
        /// <summary>
        /// Returns a Rotated Bounding Box that encapsulates mesh renderers in a hierarchy.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static RBB EncapsulateMeshRendererBounds(Transform t, Vector3 offset)
        {
            var renderers = t.gameObject.GetComponentsInChildren<MeshRenderer>();
            var mergedBounds = new Bounds(t.position + offset, Vector3.zero);
            var currentRotation = t.rotation;
            t.rotation = Quaternion.identity;
            foreach (var rend in renderers)
            {
                mergedBounds.Encapsulate(rend.bounds);
            }
            t.rotation = currentRotation;
            return new RBB(t, mergedBounds);
        }
        /// <summary>
        /// Gets the corners suround the bounding volume with an offset equal to half the size of the perimeterRBB.
        /// Think of it as placing smaller boxes at the corners of a bigger box.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="mainRBB"></param>
        /// <param name="perimeterRBB"></param>
        /// <returns></returns>
        public static Vector3[] GetBoundCorners(Transform t, RBB mainRBB, RBB perimeterRBB)
        {
            var halfHeight = perimeterRBB.Size.y * .5f;
            var sizeX = perimeterRBB.Size.x *.5f;
            var sizeZ = perimeterRBB.Size.z *.5f;

            //lower corners
            var c0 = mainRBB.GetVertex(0) - (t.rotation * new Vector3(sizeX, -halfHeight, sizeZ));//Bottom left corner
            var c1 = mainRBB.GetVertex(1) - (t.rotation * new Vector3(sizeX, -halfHeight, -sizeZ));//Top left corner
            var c2 = mainRBB.GetVertex(2) + (t.rotation * new Vector3(sizeX, halfHeight, sizeZ));//Top right corner
            var c3 = mainRBB.GetVertex(3) + (t.rotation * new Vector3(sizeX, halfHeight, -sizeZ));//Top right corner

            //upper corners
            var c4 = mainRBB.GetVertex(4) - (t.rotation * new Vector3(sizeX, halfHeight, sizeZ));//Top right corner
            var c5 = mainRBB.GetVertex(5) - (t.rotation * new Vector3(sizeX, halfHeight, -sizeZ));//Top right corner
            var c6 = mainRBB.GetVertex(6) + (t.rotation * new Vector3(sizeX, -halfHeight, sizeZ));//Top right corner
            var c7 = mainRBB.GetVertex(7) + (t.rotation * new Vector3(sizeX, -halfHeight, -sizeZ));//Top right corner
             
            return new Vector3[8] { c0, c1, c2, c3, c4, c5, c6, c7 };
        }
    }
}
