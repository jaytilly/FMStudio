﻿using System.Reflection;
using System.Threading.Tasks;

namespace FMStudio.Lib.Repositories
{
    public interface IAssemblyRepository
    {
        Task<AssemblyName> GetReferenceByName(Assembly assembly, string referenceName);

        Task<Assembly> LoadFromArchive(string path);

        Task<Assembly> LoadFromBytes(byte[] bytes);

        Task<Assembly> LoadFromFile(string path);
    }
}