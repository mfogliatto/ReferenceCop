namespace ReferenceCop
{
    public interface IAssemblyTagProvider
    {
        string GetAssemblyTag(string projectFilePath);
    }
}