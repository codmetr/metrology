using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = File.ReadAllText("data.txt");

            var sr = new StringReader(file);
            var commands = new List<KeyValuePair<string, string>> ();
            var itemStr = sr.ReadLine();
            while (!string.IsNullOrEmpty(itemStr))
            {
                var lines = itemStr.Split(' ');
                var cmd = string.Empty;
                var cmdDescr = "";
                int indexColumn = 0;
                foreach (var line in lines)
                {
                    if(string.IsNullOrEmpty(line))
                        continue;
                    if (line.All(char.IsUpper) && indexColumn<2)
                    {
                        indexColumn++;
                        if (!string.IsNullOrEmpty(cmd))
                        {
                            commands.Add(new KeyValuePair<string, string>(cmd, cmdDescr));
                        }
                        cmd = line;
                        cmdDescr = "";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(cmdDescr))
                            cmdDescr = cmdDescr + " " + line;
                        else
                            cmdDescr = line;
                    }

                }
                if (!string.IsNullOrEmpty(cmd))
                {
                    commands.Add(new KeyValuePair<string, string>(cmd, cmdDescr));
                }
                itemStr = sr.ReadLine();
            }
            StringBuilder sb = new StringBuilder();

            commands = commands.OrderBy(el => el.Key).ToList();
            foreach (var command in commands)
            {
                sb.AppendLine($"[CodeDescriptorAttribue(\"{command.Key}\", \"{command.Value}\")]");
                sb.AppendLine($"{command.Key},");
                Console.WriteLine("[CodeDescriptorAttribue(\"{0}\", \"{1}\")]", command.Key, command.Value);
                Console.WriteLine("{0},", command.Key, command.Value);
            }
            sb.ToString();
            Console.Read();
        }
    }
}
