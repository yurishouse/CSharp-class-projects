using System;

namespace Twinprime
{
    class Program
    {
        static void Main(string[] args)
        {
            int min = 0;
            int max = 50;
            int input;

            bool error = false;

            if (args.Length > 0) {
                if (int.TryParse(args[0], out input)) min = input;
                else error = true;
            }
            if (args.Length > 1) {
                if (int.TryParse(args[1], out input)) max = input;
                else error = true;
            }

            if (error) { 
                Console.WriteLine("Error");
                return;
            }
            //Console.WriteLine(min + "," + max);  
                    for (int i = min; i <= max; i++) {  
                if (isPrime(i) == true && isPrime(i + 2) == true) {  
                    Console.WriteLine(i + "," + (i + 2));  
                }
                    }
                    
        }
            static bool isPrime(int n)  
    { 
        if (n <= 1) return false; 
        if (n <= 3) return true; 
        if (n % 2 == 0 || n % 3 == 0)  
            return false; 
        for (int i = 5; i * i <= n; i = i + 6) 
            if (n % i == 0 || n % (i + 2) == 0) 
                return false; 
        return true;
    }
    }
}
