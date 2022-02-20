using System;
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
        public static Bounds EncapsulateMeshRendererBounds(Transform t)
        {
            var renderers = t.gameObject.GetComponentsInChildren<MeshRenderer>();
            var mergedBounds = new Bounds(t.position, Vector3.zero);//For the encapsulate to work correctly we need to add the transform position to the bounds.
            var currentRotation = t.rotation;
            t.rotation = Quaternion.identity;
            foreach (var rend in renderers)
            {
                mergedBounds.Encapsulate(rend.bounds);
            }
            t.rotation = currentRotation;
            mergedBounds.center -= t.position;//make sure we remove the transform's position offset from the RBB before using RBB further down the line. Weird things happen if you dont.
            return mergedBounds;
        }
        /// <summary>
        /// Gets the corners suround the bounding volume with an offset equal to half the size of the perimeterRBB.
        /// Think of it as placing smaller boxes at the corners of a bigger box.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="rbb"></param>
        /// <param name="edgeSize"></param>
        /// <returns></returns>
        public static Vector3[] GetBoundCorners(RBB rbb, Vector3 edgeSize)
        {
            var halfSizeY = edgeSize.y * .51f;
            var halfSizeX = edgeSize.x * .51f;
            var halfSizeZ = edgeSize.z * .51f;
            var verts = rbb.GetVertices();

            return new Vector3[8]
            {
            //lower corners
             verts[0] + (rbb.Rotation * new Vector3(-halfSizeX, -halfSizeY, -halfSizeZ)),//back left lower corner
             verts[1] + (rbb.Rotation * new Vector3(halfSizeX, -halfSizeY, -halfSizeZ)),//back right lower corner
             verts[2] + (rbb.Rotation * new Vector3(halfSizeX, -halfSizeY, halfSizeZ)),//foward right lower corner
             verts[3] + (rbb.Rotation * new Vector3(-halfSizeX, -halfSizeY, halfSizeZ)),//forward left lower corner

            //upper corners
             verts[4] + (rbb.Rotation * new Vector3(-halfSizeX, halfSizeY, -halfSizeZ)),//back left upper corner
             verts[5] + (rbb.Rotation * new Vector3(halfSizeX, halfSizeY, -halfSizeZ)),//back right upper corner
             verts[6] + (rbb.Rotation * new Vector3(halfSizeX, halfSizeY, halfSizeZ)),//forward right upper corner
             verts[7] + (rbb.Rotation * new Vector3(-halfSizeX, halfSizeY, halfSizeZ)),//forward left upper corner
            };
        } 
    }
}
