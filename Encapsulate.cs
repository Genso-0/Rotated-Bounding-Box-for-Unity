 
using UnityEngine;

namespace RotatedBoundingVolume
{
    /// <summary>
    /// Finds the collective bounds of all mesh renderers in an objects hierarchy.
    /// </summary>
    public static class Encapsulate
    {
        /// <summary>
        /// Returns a bounds that encapsulates mesh renderers in a hierarchy.
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
    }
}
