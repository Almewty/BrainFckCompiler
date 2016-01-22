using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace BrainFuck.Compiler
{
    public class BrainFuckCompiler
    {
        #region Private Fields

        private static Expression AtIndex = Expression.ArrayAccess(Band, Index);
        private static Expression AtIndexChar = Expression.Convert(AtIndex, typeof(Char));
        private static Expression Band = Expression.Parameter(typeof(int[]), "band");
        private static Expression BandInit = Expression.Assign(Band, Expression.NewArrayBounds(typeof(int), Expression.Constant(5000)));
        private static Expression DecBand = Expression.PreDecrementAssign(AtIndex);
        private static Expression DecIndex = Expression.PreDecrementAssign(Index);
        private static Expression IncBand = Expression.PreIncrementAssign(AtIndex);
        private static Expression IncIndex = Expression.PreIncrementAssign(Index);
        private static Expression Index = Expression.Parameter(typeof(int), "index");
        private static Expression PrintBand = Expression.Call(null, typeof(Console).GetMethod("Write", new[] { typeof(Char) }), AtIndexChar);
        private static Expression ReadChar = Expression.Call(null, typeof(Console).GetMethod("Read"));
        private static Expression ReadInt = Expression.Assign(AtIndex, ReadChar);

        #endregion Private Fields

        #region Public Methods

        public static Action Compile(string code)
        {
            return CompileToLambda(code).Compile();
        }

        public static void Compile(string code, string path)
        {
            Compile(code, path, new SavingOptions());
        }

        public static void Compile(string code, string path, SavingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var action = CompileToLambda(code);
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(options.AssemblyName), AssemblyBuilderAccess.Save);
            var module = assembly.DefineDynamicModule(options.ModuleName);
            var type = module.DefineType(options.TypeName);
            var method = type.DefineMethod(options.MethodName, MethodAttributes.Public | MethodAttributes.Static);
            action.CompileToMethod(method);
            type.CreateType();
            assembly.SetEntryPoint(method, options.ApplicationType);
            assembly.Save(path);
        }

        public static void Run(string code)
        {
            Compile(code)();
        }

        #endregion Public Methods

        #region Private Methods

        private static Expression<Action> CompileToLambda(string code)
        {
            List<Expression> commands = new List<Expression>();
            commands.Add(BandInit);
            Stack<List<Expression>> closures = new Stack<List<Expression>>();
            foreach (char c in code)
            {
                switch (c)
                {
                    case '.':
                        commands.Add(PrintBand);
                        break;

                    case ',':
                        commands.Add(ReadInt);
                        break;

                    case '+':
                        commands.Add(IncBand);
                        break;

                    case '-':
                        commands.Add(DecBand);
                        break;

                    case '>':
                        commands.Add(IncIndex);
                        break;

                    case '<':
                        commands.Add(DecIndex);
                        break;

                    case '[':
                        closures.Push(commands);
                        commands = new List<Expression>();
                        break;

                    case ']':
                        var outer = closures.Pop();
                        var label = Expression.Label();
                        var ifThen = Expression.IfThen(Expression.Equal(AtIndex, Expression.Constant(0)), Expression.Break(label));
                        var loop = Expression.Loop(Expression.Block((new[] { ifThen }).Concat(commands)), label);
                        commands = outer;
                        commands.Add(loop);
                        break;
                }
            }
            var block = Expression.Block(new[] { Band, Index }.Concat(commands));
            return Expression.Lambda<Action>(block);
        }

        #endregion Private Methods

        #region Public Classes

        public class SavingOptions
        {
            #region Public Properties

            public PEFileKinds ApplicationType { get; set; } = PEFileKinds.ConsoleApplication;
            public string AssemblyName { get; set; } = "BrainFuck";
            public string MethodName { get; set; } = "FuckMe";
            public string ModuleName { get; set; } = "BrainFuck";
            public string TypeName { get; set; } = "BrainFuck";

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}