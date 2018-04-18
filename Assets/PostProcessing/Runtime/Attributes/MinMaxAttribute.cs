using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MinMaxAttribute : Attribute
    {
        public readonly float min;
        public readonly float max;

        public MinMaxAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
