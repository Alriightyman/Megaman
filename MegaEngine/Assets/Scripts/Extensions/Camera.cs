using UnityEngine;

namespace Extensions
{
    public static class CameraExtensions
    {

        /// <summary>
        /// Determines if an object is within the cameras view
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="targetPoint">The target objects transform</param>
        /// <param name="offest">Optional value to offest the view to be a specified amount outside the view</param>
        /// <returns></returns>
        public static bool IsObjectOnScreen(this Camera cam, Transform targetPoint)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(targetPoint.position);
            return screenPoint.x > 0 
                && screenPoint.x < 1 
                && screenPoint.y > 0 
                && screenPoint.y < 1;
        }
    }
}
//               y > 1            
        /************************\
        *                        *
x > 0   *                        * x > 1
        *                        *
        *                        *
        \************************/
//               y < 0
