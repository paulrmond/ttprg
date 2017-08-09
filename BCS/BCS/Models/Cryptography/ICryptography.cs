using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Models.Cryptography
{
    public interface ICryptography
    {
        string EncryptedValue
        {
            get; set;
        }
    }
}
