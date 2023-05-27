using Dioinecail.ServiceLocator;

namespace LinkedSquad.FileSystem
{
    public interface IFileManager : IService
    {
        bool Exists(string path);
        string ReadAllText(string path);
        bool WriteAllText(string path, string content);

        bool SaveFile(string path, object data);
        object LoadFile(string path);

        string Combine(params string[] paths);
    }
}