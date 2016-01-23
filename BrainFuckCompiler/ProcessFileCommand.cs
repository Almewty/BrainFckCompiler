using ManyConsole;
using System.IO;

namespace BrainFuck.Compiler
{
    public class ProcessFileCommand : ConsoleCommand
    {
        #region Public Constructors

        public ProcessFileCommand()
        {
            IsCommand("compile", "compile brainfuck to executable");

            HasAdditionalArguments(2, "<code> <filename");
        }

        #endregion Public Constructors

        #region Public Methods

        public override int Run(string[] remainingArguments)
        {
            BrainFuckCompiler.Compile(File.ReadAllText(remainingArguments[0]), remainingArguments[1]);
            return 0;
        }

        #endregion Public Methods
    }
}