using UnityEngine;

namespace RotatedBoundingVolume
{
    /// <summary>
    /// Finds the collective bounds of all mesh renderers in an objects hierarchy.
    /// </summary>
    public class BoundsFinder : MonoBehaviour
    {
        RBB rbb;
        private void AssessBounds()
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            var mergedBounds = new Bounds(transform.position, Vector3.zero);
            var currentRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            foreach (var rend in renderers)
            {
                mergedBounds.Encapsulate(rend.bounds);
            }
            transform.rotation = currentRotation;
            rbb = new RBB(transform, mergedBounds);
        }

        void OnDrawGizmos()
        {
            AssessBounds();
            rbb.DrawBounds();
        } 
    }
}
