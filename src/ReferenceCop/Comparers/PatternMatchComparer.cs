namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;

    internal class PatternMatchComparer : IEqualityComparer<string>
    {
        private const string DefaultPattern = "*";
        private const char PrefixDelimiter = '*';

        public bool Equals(string x, string y)
        {
            if (x == DefaultPattern || y == DefaultPattern)
            {
                return true;
            }

            return PrefixEquals(x, y) || PrefixEquals(y, x);
        }

        private static bool PrefixEquals(string first, string second)
        {
            var prefixIndex = first.IndexOf(PrefixDelimiter);
            if (prefixIndex == -1)
            {
                return first.Equals(second, StringComparison.InvariantCulture);
            }
            else
            {
                var prefix = first.Substring(0, prefixIndex);
                return second.StartsWith(prefix, StringComparison.InvariantCulture);
            }
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
