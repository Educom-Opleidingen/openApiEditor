using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiInterpreter
{
    internal class Program
    {
        private const string ExampleOpenAPDefinitionV3Yaml = "https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml";
        private const string ExampleOpenAPDefinitionV3Json = "https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.json";

        static void Main(string[] args)
        {
            var openApiIntf = new OpenApiInterface();

            openApiIntf.ReadAsync(ExampleOpenAPDefinitionV3Json).GetAwaiter().GetResult();

            if (openApiIntf.HasErrors)
            {
                Console.WriteLine("Found errors: ");
                foreach (var error in openApiIntf.Errors)
                {
                    Console.WriteLine(error);
                }
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Paths:");
            foreach (var path in openApiIntf.GetPaths()) {
                Console.WriteLine(path);

                Console.WriteLine("  Methods:");
                foreach(var method in openApiIntf.GetMethods(path))
                {
                    Console.WriteLine($"    { method.Key.ToString().ToUpper() }: {method.Value.Summary} ");
                }
            }

            Console.ReadKey();
        }
    }
}
