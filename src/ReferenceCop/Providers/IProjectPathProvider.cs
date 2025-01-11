namespace ReferenceCop
{
    public interface IProjectPathProvider
    {
        string GetRelativePath(string projectFilePath);
    }
}
