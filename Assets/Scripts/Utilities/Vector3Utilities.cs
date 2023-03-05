using UnityEngine;

namespace Utilities
{
    public enum Vector3Dimensions
    {
        X = 1,
        Y = 2,
        Z = 4
    }

    public static class Vector3Utilities
    {
        public static bool AllDimensionsBiggerThan(this Vector3 source, Vector3 target)
            => source.x > target.x &&
            source.y > target.y &&
            source.z > target.z;

        public static bool AllDimensionsLowerThan(this Vector3 source, Vector3 target)
            => source.x < target.x &&
            source.y < target.y &&
            source.z < target.z;

        public static Vector3 ClearDimension(this Vector3 vector, Vector3Dimensions toClear)
        {
            if ((toClear & Vector3Dimensions.X) == Vector3Dimensions.X)
                vector.x = 0;
            if ((toClear & Vector3Dimensions.Y) == Vector3Dimensions.Y)
                vector.y = 0;
            if ((toClear & Vector3Dimensions.Z) == Vector3Dimensions.Z)
                vector.z = 0;

            return vector;
        }
    }
}