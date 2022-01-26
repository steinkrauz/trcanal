using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vksanal
{
    class Program
    {
        static string GetArg(string[] args, string argName)
        {
            foreach (string arg in args)
            {
                int idx = arg.IndexOf(argName);
                if (idx >= 0)
                {
                    return arg.Substring(idx + argName.Length);
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            string filename = GetArg(args, "-file=");
            if (filename == null)
            {
                Console.WriteLine("Usage: vksanal -file=<log file name>");
                return;
            }

            Dictionary<string, VksInfo> storage = new Dictionary<string, VksInfo>();
            List<IParser> parsers = new List<IParser>
            {
                new CreateParser(storage),
                new TopicParser(storage),
                new ParticipantParser(storage),
                new EndParser(storage)
            };
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (StreamReader file = new StreamReader(fs))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    foreach (var p in parsers)
                    {
                        if (p.IsApplicable(line))
                        {
                            p.Parse(line);
                            continue;
                        }
                    }
                }

            }


            using (DBAdapter dba = new DBAdapter())
            {
                foreach (var info in storage.Values)
                {
                    if (info.Duration < 5) continue;
                    Console.WriteLine($"{info.StartTime}\t {info.Topic} for {info.Duration} min");
                    foreach (var name in info.Participants)
                    {
                        Console.WriteLine($"\t{name}");
                    }
                    dba.Store(info);

                }
            }

        }
    }
}
