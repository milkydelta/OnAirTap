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

        if (gameDataPath == "")
        {
            System.Console.WriteLine("The provided game path contains no Data folder.");
            return;
        }

        if (!File.Exists(gamePath + "lincomm.dll"))
        {
            
            if (File.Exists(modPath + "/lincomm.dll.so"))
            {
                System.Console.WriteLine("Copying Linux communication library");
                File.Copy(modPath + "/lincomm.dll.so", gamePath + "lincomm.dll");
            }
            else
            {
                System.Console.WriteLine("Writing Linux communication library");
                CopyResourceToPath("lincomm.dll.so", gamePath + "lincomm.dll");
            }
        }

        if (!File.Exists(gameDataPath + "/Plugins/x86_64/OAT_KlakSpout.dll"))
        {
            Directory.CreateDirectory(gameDataPath + "/Plugins/x86_64");

            if (File.Exists(modPath + "/KlakSpout.dll"))
            {
                System.Console.WriteLine("Copying KlakSpout Plugin");
                File.Copy(modPath + "/KlakSpout.dll", gameDataPath + "/Plugins/x86_64/OAT_KlakSpout.dll");
            }
            else
            {
                System.Console.WriteLine("Writing KlakSpout Plugin");
                CopyResourceToPath("KlakSpout.dll", gameDataPath + "/Plugins/x86_64/OAT_KlakSpout.dll");
            }

            if (File.Exists(modPath + "/KLAKSPOUT-DLL-LICENSE.txt")){
                File.Copy(modPath + "/KLAKSPOUT-DLL-LICENSE.txt", gameDataPath + "/Plugins/x86_64/KLAKSPOUT-DLL-LICENSE.txt");
            }
        }
    }

    static void CopyResourceToPath(string resource, string path)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        if (!asm.GetManifestResourceNames().Contains(resource))
        {
            System.Console.WriteLine("No resource "+resource+" exists in assembly.");
            return;
        }

        using (Stream res = asm.GetManifestResourceStream(resource))
        using (FileStream file = File.Open(path, FileMode.Create))
        {
            res.CopyTo(file);
        }
    }
}
