using System.Diagnostics;

namespace LocalServer
{
    public static class BackgroundServer
    {
        private const string SERVER_EXE_LOCATION = "\\MiBand2DLL\\BackgroundServer\\bin\\Release\\BackgroundServer.exe";
        private static Process _process;

        
        public static void StartServer(bool hideWindow = true)
        {
            _process = new Process
            {
                StartInfo =
                {
                    FileName = System.IO.Directory.GetCurrentDirectory() + SERVER_EXE_LOCATION,
                    CreateNoWindow = hideWindow,
                    WindowStyle = hideWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
                }
            };
            _process.Start();
        }

        public static void StopServer() => _process?.Kill();
    }
}
