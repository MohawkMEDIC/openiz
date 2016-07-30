using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatasetTool
{
    class Program
    {
        static void Main(string[] args)
        {

            var consoleParms = new ParameterParser<ConsoleParameters>().Parse(args);

            var tool = typeof(Program).Assembly.ExportedTypes.FirstOrDefault(o => o.Name == consoleParms.ToolName);
            if (tool == null)
                Console.WriteLine("Could not find tool: {0}", consoleParms.ToolName);
            else
            {
                var operation = tool.GetMethod(consoleParms.OperationName);
                if (operation == null)
                    Console.WriteLine("Tool {0} does not have operation named {1}", consoleParms.ToolName, consoleParms.OperationName);
                else
                    operation.Invoke(null, new Object[] { args });
            }

            Console.ReadKey();
        }
    }
}
