namespace ReferenceCop
{
    public class Violation
    {
        public ReferenceCopConfig.Rule Rule { get; }
        public string ReferenceName { get; }
        
        public Violation(ReferenceCopConfig.Rule rule, string referenceName)
        {
            this.Rule = rule;
            this.ReferenceName = referenceName;
        }
    }
}
