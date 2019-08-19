using System;

namespace YF.Utils
{
    public partial class CommonUtil
    {
        public static string ExecuteCommandLine(string command, string arguments)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName               = command;
            process.StartInfo.UseShellExecute        = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow         = true;
            process.StartInfo.Arguments              = arguments;
            process.Start();

            // Synchronously read the standard output of the spawned process. 
            var reader = process.StandardOutput;
            var output = reader.ReadToEnd();

            // Waiting for the process to exit directly in the UI thread. Similar cases are working that way too.

            // TODO: Is it better to provide a timeout avoid any issues of forever blocking the UI thread? If so, what is
            // a relevant timeout value for soundbank generation?
            process.WaitForExit();
            process.Close();

            return output;
        }


        public static void ProcessEnumNames<T>(Action<string> process)
        {
            var typeT = typeof(T);
            var names = Enum.GetNames(typeT);
            if (names.Length == 0)
                return;
            for (int i = 0; i < names.Length; i++)
            {
                if (process != null)
                    process(names[i]);
            }
        }

        public static void ProcessDerivedTypes(Type baseType, bool includeSelf, Action<Type> process)
        {
            var types = baseType.Assembly.GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].IsClass &&
                    (types[i].IsSubclassOf(baseType) || baseType.IsAssignableFrom(types[i])))
                {
                    if (!includeSelf && baseType == types[i]) continue;
                    //   instance = Activator.CreateInstance(types[i]);
                    process(types[i]);
                }
            }
        }
    }
}