using System;
using System.IO;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            throw new Exception("Assembly path required");
        }

        var path = Path.GetFullPath(args[0]);

        if(!File.Exists(path))
        {
            throw new Exception("File '" + path + "' not found");
        }

        var asm = Assembly.LoadFile(path);

        var vers = (AssemblyFileVersionAttribute)asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0];

        Console.WriteLine(vers.Version);
    }
}