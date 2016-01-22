using ManyConsole;

namespace BrainFuck.Compiler
{
    public class ProcessFileCommand : ConsoleCommand
    {
        #region Public Constructors

        public ProcessFileCommand()
        {
            IsCommand("run", "run brainfuck code");

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