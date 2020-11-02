using System;

namespace HellowWorld {
    class Program {
        static void Main (string[] args) {
            Console.WriteLine ("Howdy World!!!!");
            Console.WriteLine(MyMethod(name: "Fred"));
        }
        static string MyMethod(int age = 4, string name = "no name")
        {
            return $"{name} is {age} years old.";
        }
        }
    }
}