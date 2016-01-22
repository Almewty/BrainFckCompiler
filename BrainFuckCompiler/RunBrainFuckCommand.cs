using ManyConsole;
using System.IO;

namespace BrainFuck.Compiler
{
    public class RunBrainFuckCommand : ConsoleCommand
    {
        #region Public Constructors

        public RunBrainFuckCommand()
        {
            IsCommand("file", "input file");

            HasAdditionalArguments(1, "<filename>");
        }

        #endregion Public Constructors

        #region Public Methods

        public override int Run(string[] remainingArguments)
        {
            BrainFuckCompiler.Run(File.ReadAllText(remainingArguments[0]));
            return 0;
        }

        #endregion Public Methods
    }
}