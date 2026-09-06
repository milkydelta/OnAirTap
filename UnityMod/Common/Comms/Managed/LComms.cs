using System.IO.MemoryMappedFiles;
using System.IO;

namespace OnAirTap;

public class LComms : WComms {
    public override bool Open(string targetName, ushort protocolMinorVersion){
        Plugin.logger.Info("Managed Wine Comms!");
        if (isOpen){return false;}
        string name = targetName + ".v1." + protocolMinorVersion.ToString();
        pMV = protocolMinorVersion;

        int size = (sizeof(float) * 8)+sizeof(int);
        if (protocolMinorVersion >= 1) {
            size += sizeof(int) * 2;
            size += sizeof(float) * 3;
        }

        string pathName = "Z:\\dev\\shm\\" + name;
        Plugin.logger.Info(pathName);


        if (!File.Exists(pathName))
        {
            Plugin.logger.Info("MAKING");
            using (var f = File.Create(pathName))
            {
                byte[] b = new byte[size];
                f.Write(b, 0, size);
            }
        }

        mmf = MemoryMappedFile.CreateFromFile(pathName, System.IO.FileMode.Open, targetName, size, MemoryMappedFileAccess.ReadWrite);

        mmfView = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        isOpen = true;
        return true;
    }
}