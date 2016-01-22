using ManyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainFuck.Compiler
{
    public class ProcessFileCommand : ConsoleCommand
    {
        public ProcessFileCommand()
        {
            IsCommand("run", "run brainfuck code");

            HasAdditionalArguments(1, "<code>");
        }

        public override int Run(string[] remainingArguments)
        {
            BrainFuckCompiler.Run(remainingArguments[0]);
            return 0;
        }
    }
}
