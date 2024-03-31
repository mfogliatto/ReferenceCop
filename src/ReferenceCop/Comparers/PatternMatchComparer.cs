namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;

    internal class PatternMatchComparer : IEqualityComparer<string>
    {
        private const string DefaultPattern = "*";
        private const char Wildcard = '*';

        public bool Equals(string x, string y)
        {
            if (x == DefaultPattern || y == DefaultPattern)
            {
                return true;
            }

            return AreEquals(x, y) || AreEquals(y, x);
        }

        private static bool AreEquals(string first, string second)
        {
            var wildcardIndex = first.IndexOf(Wildcard);
            if (wildcardIndex == -1)
            {
                return first.Equals(second, StringComparison.InvariantCulture);
            }
            else if (wildcardIndex == 0)
            {
                return SuffixEquals(first, second);
            }
            else if (wildcardIndex > 0)
            {
                return PrefixEquals(first, second, wildcardIndex);
            }
            else
            {
                return false;
            }
        }

        private static bool PrefixEquals(string first, string second, int wildcardIndex)
        {
            var prefix = first.Substring(0, wildcardIndex);
            return second.StartsWith(prefix, StringComparison.InvariantCulture);
        }

        private static bool SuffixEquals(string first, string second)
        {
            var suffix = first.Substring(1, first.Length - 1);
            return second.EndsWith(suffix, StringComparison.InvariantCulture);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
