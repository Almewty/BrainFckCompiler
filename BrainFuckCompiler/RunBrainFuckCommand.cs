using ManyConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainFuck.Compiler
{
    public class RunBrainFuckCommand : ConsoleCommand
    {
        public RunBrainFuckCommand()
        {
            IsCommand("file", "input file");

            HasAdditionalArguments(1, "<filename>");
        }
        public override int Run(string[] remainingArguments)
        {
            BrainFuckCompiler.Run(File.ReadAllText(remainingArguments[0]));
            return 0;
        }
    }
}
