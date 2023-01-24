using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

class Helper
{
    public static bool IsDebugBuild
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }

    public static Stream GetResourcesStream(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().First(n => n.EndsWith(fileName));
        return assembly.GetManifestResourceStream(resourceName);
    }

    public static string LoadTextFromResources(string fileName)
    {
        using (var stream = GetResourcesStream(fileName))
        using (var streamReader = new StreamReader(stream))
        {
            return streamReader.ReadToEnd();
        }
    }

    public static void WatchConfigFileChanges(ConfigFile config, Action onChanged = null)
    {
        var path = config.ConfigFilePath;
        WatchFileChanges(path, config.Reload);

        config.SettingChanged += (a, b) => onChanged?.Invoke();
    }

    public static void WatchFileChanges(string path, Action onChanged)
    {
        var watcher = new FileSystemWatcher();
        var directory = Path.GetDirectoryName(path);
        var fileName = Path.GetFileName(path);
        watcher.Path = directory;
        watcher.Filter = fileName;
        watcher.NotifyFilter = NotifyFilters.LastWrite
            | NotifyFilters.FileName 
            | NotifyFilters.DirectoryName;

        watcher.Changed += (obj, arg) => onChanged?.Invoke();
        watcher.Deleted += (obj, arg) => onChanged?.Invoke();
        watcher.Created += (obj, arg) => onChanged?.Invoke();
        watcher.Renamed += (obj, arg) => onChanged?.Invoke();

        watcher.EnableRaisingEvents = true;
    }

    public static void WatchFolderChanges(string path, Action onChanged)
    {
        WatchFileChanges(Path.Combine(path, "*.*"), onChanged);
    }
}
