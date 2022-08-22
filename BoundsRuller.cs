using System.Collections.Generic;
using UnityEngine;
namespace RBB_Utilities
{
    public class BoundsRuller : MonoBehaviour
    {
        public Vector3 sizeOfEachVolume = Vector3.one;
        Vector3[] corners = new Vector3[8];
        void OnDrawGizmos()
        {
            corners = new Vector3[8];
            var bounds = Encapsulate.EncapsulateMeshRendererBounds(transform); 
            RBB.Gizmos_DrawBounds(bounds, transform.position, transform.rotation, Color.yellow);

            RBB.GetBoundCorners(ref corners, bounds, bounds.center+transform.position, transform.rotation); 
            for (int i = 0; i < corners.Length; i++) 
                Gizmos.DrawWireSphere(corners[i], 0.1f); 

            DrawEdgeVolumes(corners);
        }

        private void DrawEdgeVolumes(Vector3[] corners)
        {
            var boxBounds = new Bounds(Vector3.zero, sizeOfEachVolume);
          //  var edgeRBB = new RBB(transform.position, transform.rotation, new Bounds(Vector3.zero, sizeOfEachVolume));
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[1], sizeOfEachVolume.x))
            { 
                RBB.Gizmos_DrawBounds(boxBounds, center, transform.rotation, Color.red);
            }
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[3], sizeOfEachVolume.z))
            { 
                RBB.Gizmos_DrawBounds(boxBounds, center, transform.rotation, Color.blue);
            }
            foreach (var center in GetBoundedEdgePosition(corners[0], corners[4], sizeOfEachVolume.y))
            { 
                RBB.Gizmos_DrawBounds(boxBounds, center, transform.rotation, Color.green);
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
