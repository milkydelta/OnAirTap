using System.Reflection;

namespace OAT_OWML_Patcher;

class Program
{
    static void Main(string[] args)
    {
        string modPath = args.Length > 0 ? args[0] : ".";
        string gamePath = AppDomain.CurrentDomain.BaseDirectory;

        string gameDataPath = "";

        Console.WriteLine($"Game Path: {gamePath}");

        foreach (string folder in Directory.GetDirectories(gamePath, "Outer*", SearchOption.TopDirectoryOnly))
        {
            if (folder.EndsWith("_Data"))
            {
                gameDataPath = folder;
            }
        }

        if (!File.Exists(gamePath + "lincomm.dll"))
        {
            System.Console.WriteLine("Writing Linux communication library");
            CopyResourceToPath("lincomm.dll.so", gamePath + "lincomm.dll");
        }

        if (!File.Exists(gameDataPath + "/Plugins/x86_64/OAT_KlakSpout.dll"))
        {
            System.Console.WriteLine("Writing KlakSpout Plugin");
            Directory.CreateDirectory(gameDataPath + "/Plugins/x86_64");
            CopyResourceToPath("KlakSpout.dll", gameDataPath + "/Plugins/x86_64/OAT_KlakSpout.dll");
            File.Copy(modPath + "/KLAKSPOUT-DLL-LICENSE.txt", gameDataPath + "/Plugins/x86_64/KLAKSPOUT-DLL-LICENSE.txt");
        }
    }

    static void CopyResourceToPath(string resource, string path)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        using (Stream res = asm.GetManifestResourceStream(resource))
        using (FileStream file = File.Open(path, FileMode.Create))
        {
            res.CopyTo(file);
        }
    }
}
