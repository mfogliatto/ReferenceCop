namespace ReferenceCop
{
    using System.Collections.Generic;

    public interface IViolationDetector<TAssemblyIdentity>
    {
        IEnumerable<Violation> GetViolationsFrom(IEnumerable<ReferenceEvaluationContext<TAssemblyIdentity>> references);

        IEnumerable<Violation> GetViolationsFromExperimental(IEnumerable<ReferenceEvaluationContext<TAssemblyIdentity>> references);
    }
}
