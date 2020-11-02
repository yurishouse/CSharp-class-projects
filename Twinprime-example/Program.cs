using System;

namespace Twinprime_example
{
    class Program
    {
        public static void Main(string[] args) {  
              
            for (int i = 2; i <= 200; i++) {  
                if (checkprime(i) == true && checkprime(i + 2) == true) {  
                    Console.WriteLine(i + "," + (i + 2));  
                }  
            }  
            Console.ReadKey();  
        }  
        public static bool checkprime(int n) {  
            int flag = 0;  
            for (int i = 2; i <= n / 2; i++) {  
                if (n % i == 0) {  
                    flag = 1;  
                    break;  
                }  
            }  
            if (flag == 0) {  
                return true;  
            } else {  
                return false;  
            }  
        }  
    }  
}  
    }
}
