using System;
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
            string code = "[ This program prints \"Hello World!\" and a newline to the screen, its"  +
  "length is 106 active command characters [it is not the shortest.]" + 

  "This loop is a \"comment loop\", it's a simple way of adding a comment" + 
  "to a BF program such that you don't have to worry about any command" + 
  "characters. Any \".\", \",\", \"+\", \"-\", \"<\" and \">\" characters are simply" + 
  "ignored, the \"[\" and \"]\" characters just have to be balanced." + 
"]" + 
"+++++ +++               Set Cell #0 to 8" + 
"[" + 
"    >++++               Add 4 to Cell #1; this will always set Cell #1 to 4" + 
"    [                   as the cell will be cleared by the loop" + 
"        >++             Add 2 to Cell #2" + 
"        >+++            Add 3 to Cell #3" + 
"        >+++            Add 3 to Cell #4" + 
"        >+              Add 1 to Cell #5" + 
"        <<<<-           Decrement the loop counter in Cell #1" + 
"    ]                   Loop till Cell #1 is zero; number of iterations is 4" + 
"    >+                  Add 1 to Cell #2" + 
"    >+                  Add 1 to Cell #3" + 
"    >-                  Subtract 1 from Cell #4" + 
"    >>+                 Add 1 to Cell #6" + 
"    [<]                 Move back to the first zero cell you find; this will" + 
"                        be Cell #1 which was cleared by the previous loop" + 
"    <-                  Decrement the loop Counter in Cell #0" + 
"]                       Loop till Cell #0 is zero; number of iterations is 8" + 

"The result of this is:" + 
"Cell No :   0   1   2   3   4   5   6" + 
"Contents:   0   0  72 104  88  32   8" + 
"Pointer :   ^" + 

">>.                     Cell #2 has value 72 which is 'H'" + 
">---.                   Subtract 3 from Cell #3 to get 101 which is 'e'" + 
"+++++++..+++.           Likewise for 'llo' from Cell #3" + 
">>.                     Cell #5 is 32 for the space" + 
"<-.                     Subtract 1 from Cell #4 for 87 to give a 'W'" + 
"<.                      Cell #3 was set to 'o' from the end of 'Hello'" + 
"+++.------.--------.    Cell #3 for 'rl' and 'd'" + 
">>+.                    Add 1 to Cell #5 gives us an exclamation point" + 
">++.                    And finally a newline from Cell #6";

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
            Expression.Lambda<Action>(block).CompileToMethod(mb);
            tb.CreateType();
            ass.SetEntryPoint(mb, PEFileKinds.ConsoleApplication);
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