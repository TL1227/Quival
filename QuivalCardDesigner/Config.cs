using System.IO;

namespace QuivalCardDesigner;

public class Config
{
    public DirectoryInfo CardDirectory { get; set; }

    public Config(string cardDirectoryPath)
    {
        CardDirectory = new(cardDirectoryPath);

        if (!CardDirectory.Exists)
            CardDirectory.Create();
    }
}
