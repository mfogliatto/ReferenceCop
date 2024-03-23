namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;

    internal class ExactMatchComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.InvariantCulture);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
