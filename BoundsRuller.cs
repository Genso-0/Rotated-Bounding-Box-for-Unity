using System.Collections.Generic;
using UnityEngine;
namespace RotatedBoundingVolume
{
    public class BoundsRuller : MonoBehaviour
    {
        public Vector3 sizeOfEachVolume = Vector3.one;
        void OnDrawGizmos()
        {
            var bounds = Encapsulate.EncapsulateMeshRendererBounds(transform);
            var encapsulatingRBB = new RBB(transform.position, transform.rotation, bounds);
            encapsulatingRBB.DrawBounds(Color.yellow);

            var corners = Encapsulate.GetBoundCorners(encapsulatingRBB, sizeOfEachVolume); 
            for (int i = 0; i < corners.Length; i++) 
                Gizmos.DrawWireSphere(corners[i], 0.1f); 

            DrawEdgeVolumes(corners);
        }

        private void DrawEdgeVolumes(Vector3[] corners)
        {
            var edgeRBB = new RBB(transform.position, transform.rotation, new Bounds(Vector3.zero, sizeOfEachVolume));
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[1], sizeOfEachVolume.x))
            {
                edgeRBB.OffSet = center;
                edgeRBB.DrawBounds(Color.red);
            }
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[3], sizeOfEachVolume.z))
            {
                edgeRBB.OffSet = center;
                edgeRBB.DrawBounds(Color.blue);
            }
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[4], sizeOfEachVolume.y))
            {
                edgeRBB.OffSet = center;
                edgeRBB.DrawBounds(Color.green);
            }
        }
        private IEnumerable<Vector3> GetBoundedEdgePosition(Vector3 start, Vector3 end, float sizeMagnitudeOnAxis)
        {
            float distance = Vector3.Distance(start, end);
            float step = sizeMagnitudeOnAxis / distance;
            for (float i = 0; i < 1; i += step)
            {
                yield return Vector3.Lerp(start, end, i);
            }
        }
    }
}
