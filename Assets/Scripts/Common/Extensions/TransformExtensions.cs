using UnityEngine;

namespace Common.Extensions {

    public static class TransformExtensions {

        /// <summary>
        /// Удаляет всех дочерних объектов Transform
        /// </summary>
        /// <param name="transform">Transform, у которого нужно удалить детей</param>
        public static void RemoveAllChildren(this Transform transform) {
            for (int i = transform.childCount - 1; i >= 0; i--) {
                if (Application.isPlaying) {
                    Object.Destroy(transform.GetChild(i).gameObject);
                }
                else {
                    Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }

    }

}