/*
 * trcanal -- TruConf log analyzer, 2022
 *
 * The main program module
 *
 * written by Steinkrauz <steinkrauz@yahoo.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3 of the License ONLY.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
