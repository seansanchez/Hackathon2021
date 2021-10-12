using System;

namespace H21.Wellness.Extensions
{
    public static class StringExtensions
    {
        public static void ThrowIfNullOrWhitespace(this string source, string name)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(name) ? nameof(name) : name);
            }
        }

        public static void ThrowIfNullOrEmpty(this string source, string name)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(string.IsNullOrWhiteSpace(name) ? nameof(name) : name);
            }
        }
    }
}