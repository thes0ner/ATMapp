using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMapp.UI
{
    public static class Validator
    {

        public static T Convert<T>(string prompt)
        {
            bool valid = false;
            string userInput;

            while (!valid)
            {
                // prints 'enter your card number' & needs card number. 
                userInput = Utility.GetUserInput(prompt);

                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));

                    if (converter != null)
                    {
                        //Converts the string to T type.
                        return (T)converter.ConvertFromString(userInput);
                    }
                    else
                    {
                        return default;
                    }
                }
                catch
                {
                    Utility.PrintMessage("Invalid Input. Try again! ", false);
                }
            }

            return default;
        }
    }
}
