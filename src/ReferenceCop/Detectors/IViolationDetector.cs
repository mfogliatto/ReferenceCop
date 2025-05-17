namespace ReferenceCop
{
    using System.Collections.Generic;

    public interface IViolationDetector<TAssemblyIdentity>
    {
        IEnumerable<Violation> GetViolationsFrom(IEnumerable<TAssemblyIdentity> references);
    }
}
