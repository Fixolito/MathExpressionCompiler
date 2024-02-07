using System.Numerics;
using System.IO;

namespace CodeGenerator;

internal static class CodeGenerator
{
    internal static void CreateCode(string baseName, string[] children)
    {
        string path = Path.Combine("..", "MathExpressionCompiler", baseName + "s");
        Directory.CreateDirectory(path);
        CreateBaseClass(path, baseName, children);
        foreach(string child in children)
        {
            string[] parts = child.Split(':');
            string className = parts[0].Trim();
            string fieldList = parts[1].Trim();
            CreateChildClass(path, baseName, className, fieldList);
        }
    }

    private static void CreateBaseClass(string path, string baseName, string[] children)
    {
        path = Path.Combine(path, baseName + ".cs");
        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("namespace MEC;");
            writer.WriteLine();
            writer.WriteLine($"internal abstract class {baseName}<Space>");
            writer.WriteLine("{");

            // Interface
            writer.WriteLine("    internal interface IVisitor<T>");
            writer.WriteLine("    {");



            foreach(string child in children)
            {
                string childClassName = child.Split(':')[0].Trim();
                writer.WriteLine($"        T Visit{childClassName}{baseName}({childClassName}<Space> {baseName.ToLower()});");
            }
            writer.WriteLine("    }");

            // The base accept() method.
            writer.WriteLine();
            writer.WriteLine($"    internal abstract T Accept<T>(IVisitor<T> visitor);");
            writer.WriteLine("}");
        }
    }

    private static void CreateChildClass(string path, string baseName, string className, string fieldString)
    {
        path = Path.Combine(path, className + ".cs");
        string[] arguments = fieldString.Split(", ");
        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("namespace MEC;");
            writer.WriteLine();
            writer.WriteLine($"internal class {className}<Space> : {baseName}<Space>");
            writer.WriteLine("{");
            foreach(string field in arguments)
            {
                string type = field.Split(" ")[0];
                if (type[0] == '<' && type[^1] == '>') type = type[1..^1];

                string fieldName = FirstLetterToUpper(field.Split(" ")[1]);
                writer.WriteLine($"    internal readonly {type} {fieldName};");
            }
            writer.WriteLine();

            // remove parenthesis for types like:  <Space>
            List<string> cleanedArguments = [];
            foreach (string t in arguments)
            {
                string type = t.Split(' ')[0];
                string name = t.Split(' ')[1];
                while (type.Length > 2 && type[0] == '<' && type[^1] == '>') type = type[1..^1];
                cleanedArguments.Add($"{type} {name}");
            }
            fieldString = string.Join(", ", cleanedArguments);

            writer.WriteLine($"    internal {className}({fieldString})");
            writer.WriteLine("    {");
            foreach(string field in arguments)
            {
                string name = field.Split(" ")[1];
                writer.WriteLine($"        this.{FirstLetterToUpper(name)} = {name};");
            }
            
            writer.WriteLine("    }");

            // Implementing interface
            writer.WriteLine();
            writer.WriteLine($"    internal override T Accept<T>(IVisitor<T> visitor)");
            writer.WriteLine("    {");
            writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("    }");
           
            writer.WriteLine("}");
        }
    }

    private static string FirstLetterToUpper(string input)
    {
        return char.ToUpper(input[0]) + input[1..];
    }
}