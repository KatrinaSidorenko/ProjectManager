using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class InputValidator
    {
        public static int IntegerValidator()
        {
            Console.Write("Enter option number: ");

            string input = Console.ReadLine();
            int operationNumber;

            if (int.TryParse(input, out operationNumber))
            {
                return operationNumber;
            }
            else
            {
                Console.WriteLine("Invalid option number, try again");
                return IntegerValidator();
            }

        }

        public static DateTime DateTimeValidator(string typeOfDate)
        {
            Console.Write(typeOfDate);

            string input = Console.ReadLine();
            DateTime date;

            if(DateTime.TryParse(input, out date) && date >= DateTime.Now.Date)
            {
                return date;
            }
            else
            {
                Console.WriteLine("Invalid date input / You entered a date smaller than today's");
                return DateTimeValidator(typeOfDate);
            }
        }

        public static int IndexValidator(int arrLength)
        {
            Console.Write("Enter number of item: ");
            string input = Console.ReadLine();
            int operationNumber;

            if (int.TryParse(input, out operationNumber) && operationNumber > 0 && operationNumber <= arrLength)
            {
                return operationNumber;
            }
            else
            {
                Console.WriteLine("Invalid option number, try again");
                return IntegerValidator();
            }
        }
    }
}
