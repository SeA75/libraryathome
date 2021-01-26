using LibraryAtHomeProvider;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BooksParser
{
    public static class PluginLoader
    {
        public static IBooksProvider GetPluginFromFolder(string pluginName, string pluginFolder, IRestRequestManager reqMngr)
        {
            var assembly = LoadPlugInAssembly(pluginName, pluginFolder);
            return GetPlugin(assembly, reqMngr);
        }

        private static Assembly LoadPlugInAssembly(string plugInName, string pluginFolder)
        {
            Assembly assembly = null;

            DirectoryInfo pluginsDirectory = new DirectoryInfo(pluginFolder);

            if (!plugInName.Contains(".dll"))
                plugInName += ".dll";


            if (pluginsDirectory.Exists)
            {
                FileInfo plugInFile = pluginsDirectory.GetFiles(plugInName).FirstOrDefault();

                if (plugInFile != null)
                {
                    try
                    {
                        assembly = LoadAssembly(plugInFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return assembly;
        }

        private static Assembly LoadAssembly(FileInfo file)
        {
            Assembly loadedAssembly = null;
            
            loadedAssembly = Assembly.LoadFile(file.FullName); // Double loading for get full assembly name

            Assembly toreturn = Assembly.Load(loadedAssembly.FullName);

            return toreturn;
        }

        private static IBooksProvider GetPlugin(Assembly assembly, IRestRequestManager reqMngr)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "assembly cannot be null.");
            }

            Type availableTypes2 =
                assembly.ExportedTypes.FirstOrDefault(type => type.GetInterface("IBooksProvider") != null);


          //  Type availableTypes = assembly.DefinedTypes.FirstOrDefault(info => info.Name == "IBooksProvider");//. .GetInterface("IBooksProvider");


//.FirstOrDefault(info => info.ImplementedInterfaces.FirstOrDefault(type => type.Name == "IBooksProvider") != null );

            if (availableTypes2 != null)
            {
                return (IBooksProvider) Activator.CreateInstance(availableTypes2, reqMngr);
  //              var res = Activator.CreateInstance(availableTypes);
                
            }
                //return Activator.CreateInstance(availableTypes) as IBooksProvider;
            return null;
        }
    }
}
