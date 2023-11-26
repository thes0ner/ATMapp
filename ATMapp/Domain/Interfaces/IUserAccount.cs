using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMapp.Domain.Interfaces
{
    public interface IUserAccount
    {
        void CheckBalance();
        void PlaceDeposit();
        void MakeWithDrawal();
    }
}
