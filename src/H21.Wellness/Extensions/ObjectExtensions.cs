using System;

namespace H21.Wellness.Extensions
{
    public static class ObjectExtensions
    {
        public static void ThrowIfNull(this object source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(name) ? nameof(name) : name);
            }
        }
    }
}