using System.Diagnostics;
using HYDRANT;

namespace Firehose;
internal class main
{
    public static Boolean StartLLMServer = true;
    public static void Main(string[] args)
    {
        Console.SetWindowSize(800, 300);
        Console.WriteLine("To whom does the true voice speak?");
        if (StartLLMServer)
        {
            Console.WriteLine("Starting LLM Server");
            ProcessStartInfo P = new()
            {
                FileName = "cmd.exe",
                Arguments = "/k python3 -m llama_cpp.server --config_file model_config.cfg",
                UseShellExecute = true
            };
            Process.Start(P);
            Console.WriteLine("To whom does the true form show itself?");
        }

        Console.WriteLine("Attaining blacklist");
        Console.WriteLine($"I ask: Why did humans disappear from the world?\nI answer, {Article.GetURLs().Count} articles are in the DB");

        Console.WriteLine("Starting Firehose.");
        Hose.Init();
        Console.ReadLine();
    }
}