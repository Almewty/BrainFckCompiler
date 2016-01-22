using ManyConsole;
using System;
using System.Collections.Generic;

namespace BrainFuck.Compiler
{
    internal class Program
    {
        #region Private Methods

        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }

        private static int Main(string[] args)
        {
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = GetCommands();
            // then run them.
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        #endregion Private Methods
    }
}