# MathExpressionCompiler
Parse math strings into Delegates for real and complex numbers. 
## Getting started
There are only two methods and two number spaces to choose from, but more to this later. Let's run the most basic form.
````csharp
var del = MathExpressionCompiler.CreateDelegate<double>("length + 3 * height", out string[] variableNameList);
Console.WriteLine($"[{string.Join(", ", variableNameList)}]");
//> [length, height]
Console.WriteLine(del.DynamicInvoke(4,7));
//>  25
````

If you want to change number space to complex 

````csharp
var del = MathExpressionCompiler.CreateDelegate<Complex>("length + 3 * height", out string[] variableNameList);
Console.WriteLine($"[{string.Join(", ", variableNameList)}]");
//> [length, height]
Console.WriteLine(del.DynamicInvoke(new Complex(4,2),new Complex(5,7)));
//>  <19; 23>
````

## Forcing input to double
When using the complex numberspace, you can also create a delegate that takes doubles and just sets the imaginry part to 0.

````csharp
var del = MathExpressionCompiler.CreateDelegateWithDoubleParameters<Complex>("length + 3 * height", out string[] variableNameList);
Console.WriteLine(del.DynamicInvoke(4,7));
//> <25; 0>
````
Since ``i`` will be parsed as the complex unit you can still implement complex functions this way.
````csharp
var del = MathExpressionCompiler.CreateDelegateWithDoubleParameters<Complex>("a + b * i", out string[] variableNameList);
Console.WriteLine(del.DynamicInvoke(4,7));
//> <4; 7>
````
Another usecase for this is entering a value using polar coordinates
````csharp
var del = MathExpressionCompiler.CreateDelegateWithDoubleParameters<Complex>("polar(a,b)", out string[] variableNameList);
Console.WriteLine(del.DynamicInvoke(4,7));
//> <3,0156090173732184; 2,6279463948751562>
````
## General Rules
1. All strings get transformed to lowercase.
2. Use ``*`` to connect numbers and variables. For now sloppy notation will not get fixed.
3. To prevent scoping confusion all methods like ``sqrt``, ``sin``, ``cos``... need to be followed by ``(<term>)``.
4. Methods taking multiple parameters like ``sum`` have their parameters seperated by ``,`` : ``sum(x,1,4,x/2)``.
5. Spaces can be used to make you equations easier to read.