// See https://aka.ms/new-console-template for more information

namespace Cromatix.MP4Reader
{

    public static class Program
    {
        static string? InputFile;
        static string? OutputFile;

        static Dictionary<string, (string help, bool required)> ArgumentHelper { get; } =
            new Dictionary<string, (string, bool)>
            {
                {"-i", ("Specify the path of the input file", true)},
                {"-o", ("Specify the path of the output file", true)},
                {"-h", ("Show the application help information", false)},
            };

        public static void Main(string[] args)
        {
            var success = ReadArgs(args);
            if (!success.HasValue)
                return;

            if (!success.Value)
            {
                Console.WriteLine("Exiting...");
                return;
            }

            if (InputFile == null)
            {
                return;
            }

            if (OutputFile == null)
            {
                return;
            }

            if (!File.Exists(InputFile))
            {
                Console.WriteLine($"File {InputFile} was not found.");
                return;
            }
            else if (File.Exists(OutputFile))
            {
                Console.WriteLine($"File {OutputFile} already exists.");
                return;
            }

            using (var fs = new FileStream(InputFile, FileMode.Open))
            {
                try
                {
                    MP4MetadataReader reader = new MP4MetadataReader(fs);
                    reader.ProcessGPMFTelemetry();
                    reader.ExportToFile(OutputFile, ExportFormat.GPX);
                    Console.WriteLine($"Output file successfully written at {OutputFile}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Output file creation failed.");
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }

        private static bool? ReadArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                PrintHelp();
                return null;
            }

            List<string> readArgs = new List<string>();
            string? inputFile = null, outputFile = null;

            var argIndex = 0;
            while (argIndex < args.Length)
            {
                if (readArgs.Any(it => it == args[argIndex]))
                {
                    Console.WriteLine($"Argument {args[argIndex]} already provided");
                }

                switch (args[argIndex])
                {
                    case "-i":
                        if ((argIndex + 1) < args.Length)
                        {
                            readArgs.Add(args[argIndex]);
                            inputFile = args[++argIndex];
                        }
                        else
                        {
                            Console.WriteLine($"Argument {args[argIndex]} requires the path an input file to be provided");
                            return false;
                        }
                        break;
                    case "-o":
                        if ((argIndex + 1) < args.Length)
                        {
                            readArgs.Add(args[argIndex]);
                            outputFile = args[++argIndex];
                        }
                        else
                        {
                            Console.WriteLine($"Argument {args[argIndex]} requires the path for an output file to be provided");
                            return false;
                        }
                        break;
                    case "-h":
                        PrintHelp();
                        return null;
                    default:
                        Console.WriteLine($"Argument {args[argIndex]} not recognized");
                        return false;
                }

                argIndex++;
            }

            if (inputFile == null)
            {
                Console.WriteLine($"No file was given for processing.");
                return false;
            }
            else if (outputFile == null)
            {
                Console.WriteLine($"No file was given for output.");
                return false;
            }

            Program.InputFile = inputFile;
            Program.OutputFile = outputFile;
            return true;
        }
        static void PrintHelp()
        {
            string name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            Console.WriteLine($"Usage: {name}.exe [args]");
            foreach (var kvp in ArgumentHelper)
                Console.WriteLine($"{kvp.Key}\t{kvp.Value.help}{(kvp.Value.required ? " [Required]" : "")}");
        }
    }
}


