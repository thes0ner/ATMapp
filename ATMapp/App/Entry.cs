using ATMapp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMapp.App
{
    class Entry
    {
        static void Main(string[] args)
        {
            ATMapp atmApp = new ATMapp();
            atmApp.InitializeData(); 
            atmApp.Run();
        }
    }
}
