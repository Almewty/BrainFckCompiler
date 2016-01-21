﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace bf
{
    internal class Program
    {
        #region Private Methods

        private static void Main (string[] args)
        {
            string code = @",[.,]";

            var band = Expression.Parameter(typeof(int[]), "band");
            var index = Expression.Parameter(typeof(int), "index");
            var bandInit = Expression.Assign(band, Expression.NewArrayBounds(typeof(int), Expression.Constant(5000)));
            var atIndex = Expression.ArrayAccess(band, index);
            var atIndexChar = Expression.Convert(atIndex, typeof(Char));
            var incBand = Expression.PreIncrementAssign(atIndex);
            var decBand = Expression.PreDecrementAssign(atIndex);
            var incIndex = Expression.PreIncrementAssign(index);
            var decIndex = Expression.PreDecrementAssign(index);
            var printBand = Expression.Call(null, typeof(Console).GetMethod("Write", new[] { typeof(Char) }), atIndexChar);
            var readChar = Expression.Call(null, typeof(Console).GetMethod("Read"));
            var readInt = Expression.Assign(atIndex, readChar);


            List<Expression> commands = new List<Expression>();
            commands.Add(bandInit);

            Stack<List<Expression>> closures = new Stack<List<Expression>>();

            foreach (char c in code)
            {
                switch (c)
                {
                    case '.':
                        commands.Add(printBand);
                        break;
                    case ',':
                        commands.Add(readInt);
                        break;
                    case '+':
                        commands.Add(incBand);
                        break;
                    case '-':
                        commands.Add(decBand);
                        break;
                    case '>':
                        commands.Add(incIndex);
                        break;
                    case '<':
                        commands.Add(decIndex);
                        break;
                    case '[':
                        closures.Push(commands);
                        commands = new List<Expression>();
                        break;
                    case ']':
                        var outer = closures.Pop();
                        var label = Expression.Label();
                        commands.Add(Expression.IfThen(Expression.Equal(atIndex, Expression.Constant(0)), Expression.Break(label)));
                        var loop = Expression.Loop(Expression.Block(commands), label);
                        commands = outer;
                        commands.Add(loop);
                        break;
                }
            }


            var block = Expression.Block(new[] { band, index }, commands);

            //Console.WriteLine(block.ToString());

            var ass = AssemblyBuilder.DefineDynamicAssembly(new System.Reflection.AssemblyName("CompiledBf"), AssemblyBuilderAccess.Save);
            var mod = ass.DefineDynamicModule("test", "test.exe");
            var tb = mod.DefineType("BF");
            var mb = tb.DefineMethod("Exec", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static);
            /*var action = */Expression.Lambda<Action>(block).CompileToMethod(mb);
            ass.Save("test.exe");
            

            //action();

            Console.ReadKey();

            //// Creating a parameter expression.
            //ParameterExpression value = Expression.Parameter(typeof(int), "value");

            //// Creating an expression to hold a local variable.
            //ParameterExpression result = Expression.Parameter(typeof(int), "result");

            //// Creating a label to jump to from a loop.
            //LabelTarget label = Expression.Label(typeof(int));

            //// Creating a method body.
            //BlockExpression block = Expression.Block(
            //    new[] { result },
            //    Expression.Assign(result, Expression.Constant(1)),
            //        Expression.Loop(
            //           Expression.IfThenElse(
            //               Expression.GreaterThan(value, Expression.Constant(1)),
            //               Expression.MultiplyAssign(result,
            //                   Expression.PostDecrementAssign(value)),
            //               Expression.Break(label, result)
            //           ),
            //       label
            //    )
            //);

            //// Compile and run an expression tree.
            //int factorial = Expression.Lambda<Func<int, int>>(block, value).Compile()(5);

            //Console.WriteLine(factorial);
        }

        #endregion Private Methods
    }
}