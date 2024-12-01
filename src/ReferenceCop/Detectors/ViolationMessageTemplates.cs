namespace ReferenceCop
{
    public static class ViolationMessageTemplates
    {
        public const string IllegalReference = 
            "Illegal reference '{0}' must be removed, as its usage is forbidden by rule '{1}': {2}";

        public const string DiscouragedReference =
            "Consider removing reference '{0}' as its usage is discouraged by rule '{1}': {2}";

        public static string GetIllegalReferenceMessage(string reference, string ruleName, string ruleDescription)
        {
            return string.Format(IllegalReference, reference, ruleName, ruleDescription);
        }

        public static string GetDiscouragedReferenceMessage(string reference, string ruleName, string ruleDescription)
        {
            return string.Format(DiscouragedReference, reference, ruleName, ruleDescription);
        }
    }
}
