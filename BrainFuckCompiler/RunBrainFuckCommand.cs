using ManyConsole;

namespace BrainFuck.Compiler
{
    public class RunBrainFuckCommand : ConsoleCommand
    {
        #region Public Constructors

        public RunBrainFuckCommand()
        {
            IsCommand("run", "run brainfuck");

            HasAdditionalArguments(1, "<code>");
        }

        #endregion Public Constructors

        #region Public Methods

        public override int Run(string[] remainingArguments)
        {
            BrainFuckCompiler.Run(remainingArguments[0]);
            return 0;
        }

        #endregion Public Methods
    }
}