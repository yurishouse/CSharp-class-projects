using System;

namespace input_validation {
    class Program {

        public static void Main (string[] args) 
        {
            // Q1
            long sum = 0;
            for (int j = 0; j < 3; j++) 
            {
                Console.WriteLine ("Please enter a int.");
                int input = 0;
                try 
                {
                    input = SafePromptForInt ();
                }
                 catch (Exception e) 
                 {
                    Console.WriteLine (e); // MAX_ATTEMPT existed for stack overflow, so left here just in case.
                    return;
                }
                sum += input;
            }
            Console.WriteLine ("The sum of the 3 int you entered is: " + sum);

            // Q2
            Console.WriteLine ("Please enter a int for the polynomial");
            int secondInput = 0;
            long polySum = 0;
            try 
            {
                secondInput = SafePromptForInt ();
            }
             catch (Exception e) 
             {
                Console.WriteLine (e); // MAX_ATTEMPT existed for stack overflow, so left here just in case.
                return;
            }
            polySum = Polynomial (secondInput);
            Console.WriteLine ("The polynomial of the int you entered is: " + polySum);

            // Q3
            Console.WriteLine ("Please enter the number of seconds to be converted");
            int inputSeconds = 0;
            string result = "";
            try 
            {
                inputSeconds = SafePromptForInt ();
            }
             catch (Exception e) 
             {
                Console.WriteLine (e); // MAX_ATTEMPT existed for stack overflow, so left here just in case.
                return;
            }
            result = ConvertTime (inputSeconds);
            Console.WriteLine ("The time in (hh/mm/ss) is: " + result);

            // Q4
            Console.WriteLine ("enter the intgers, type 'end' to end");
            string s;
            int min = Int32.MaxValue;
            int max = Int32.MinValue;
            int count = 0;
            while (true) 
            {
                s = Console.ReadLine ();
                if (s == "end") break;
                int t;
                try 
                {
                    t = Convert.ToInt32 (s);
                } 
                catch (FormatException) 
                {
                    Console.WriteLine ("[Error.] Input Invalid, Ignored.");
                    continue;
                } 
                catch (OverflowException) 
                {
                    Console.WriteLine ("[Error.] Input Invalid, Ignored.");
                    continue;
                }
                count++; // A number is entered and checked, number count ++
                if (t < min) min = t;
                if (t > max) max = t;
            }
            if (count == 0) 
            {
                Console.WriteLine ("[Error.] You didn't enter any number!");
            }
             else 
             {
                Console.WriteLine ("The Maximum is " + max);
                Console.WriteLine ("The Minimum is " + min);
            }

            // Q5
            int i = 0;
            string outputBuffer = "";
            while (i <= 200) 
            {
                if (i != 0) 
                {
                    outputBuffer += ", ";
                    if ((i % 20) == 0) 
                    {
                        Console.WriteLine (outputBuffer);
                        outputBuffer = "";
                    }
                }
                outputBuffer += i;
                i += 2;
            }
            Console.WriteLine (outputBuffer);

            // Q6
            i = 199;
            outputBuffer = "";
            // reusing var count so no declaration
            count = 0;
            do 
            {
                if (i != 1) 
                {
                    outputBuffer += ", ";
                    if (count == 25) 
                    {
                        Console.WriteLine (outputBuffer);
                        outputBuffer = "";
                        count = 0;
                    }
                }
                outputBuffer += i;
                count++;
                i -= 2;
            }
             while (i > 0); 
            {
                Console.WriteLine (outputBuffer);
            }
            
            // Q7

            while (true) 
            {
                Console.WriteLine ("Enter a score(0-100) to be converted to grade. Type quit to quit");
                s = Console.ReadLine ();
                if (s == "quit") break;
                int t;
                try 
                {
                    t = Convert.ToInt32 (s);
                }
                 catch (Exception) 
                 {
                    Console.WriteLine ("[Error.] Input Invalid, Ignored.");
                    continue;
                }
                switch (GetGrade (t)) 
                {
                    case -1:
                        Console.WriteLine ("[Error.] score entered not valid!");
                        break;
                    case 0:
                        Console.WriteLine ("Grade: F");
                        break;
                    case 1:
                        Console.WriteLine ("Grade: D");
                        break;
                    case 2:
                        Console.WriteLine ("Grade: C");
                        break;
                    case 3:
                        Console.WriteLine ("Grade: B");
                        break;
                    case 4:
                        Console.WriteLine ("Grade: A");
                        break;
                }
            }
        }

        // Helper Functions

        // get grade of score
        // 0=F 1=D 2=C 3=B 4=A
        // (can be used to compute GPA)
        // A:91-100 B:81-90 C:71-80 D:61-70 F:<=60
        // REMARK: return -1 when score >100 or <0
        
        private static int GetGrade (int score) 
        {
            if ((score > 100) || (score < 0)) return -1;
            switch ((score - 1) / 10) 
            { 
                // Map score to A:90-99=>9
                case 9:
                    return 4;
                case 8:
                    return 3;
                case 7:
                    return 2;
                case 6:
                    return 1;
                default:
                    return 0;
            }
        }

        private static int SafePromptForInt () 
        {
            return SafePromptForInt (0);
        }

        private static int SafePromptForInt (int recursionCt) 
        {
            string input = Console.ReadLine ();
            int inputAsInt;
            try 
            {
                inputAsInt = Convert.ToInt32 (input);
            } 
            catch (Exception) 
            {
                Console.WriteLine ("[Error.] The value you entered is invalid, Please try again.");
                inputAsInt = SafePromptForInt (recursionCt + 1);
            }
            return inputAsInt;
        }

        private static long Polynomial (int input) 
        {
            long poly = 4 * (long) Math.Pow (input, 3) + 5 * input - 2;
            return poly;
        }

        private static string ConvertTime (int input) 
        {
            //TimeSpan time = TimeSpan.FromSeconds(input);
            // from https://stackoverflow.com/questions/463642/what-is-the-best-way-to-convert-seconds-into-hourminutessecondsmilliseconds
            //string str = time.ToString(@"hh\:mm\:ss");
            int second = input % 60;
            int minute = (input / 60) % 60;
            int hours = input / 3600;
            string str = "";

            str += hours;
            str += " hours, ";
            str += minute;
            str += " minutes, ";
            str += second;
            str += " seconds";
            return str;
        }
    }
}