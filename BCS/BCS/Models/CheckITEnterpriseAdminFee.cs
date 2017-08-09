using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class CheckITEnterpriseAdminFee
    {
        string[] ZoneType;
        public CheckITEnterpriseAdminFee(string[] ZoneType)
        {
            this.ZoneType = ZoneType;
        }

        public bool HasITWord()
        {
            bool HasITWord = false;
            string[] it = new string[] { "I.T", "i.t", "I.t", "i.T", "IT", "it", "iT", "It" };

            foreach (var ch in ZoneType)
            {
                if (it.Any(m => m.ToString() == ch.ToString()))
                {
                    HasITWord = true;
                    break;
                }
            }

            return HasITWord;
        }

        public bool HasCEZWord()
        {
            bool HasCEZWord = false;

            foreach (var ch in ZoneType)
            {
                if (ch.ToUpper() == "SEZ")
                {
                    HasCEZWord = true;
                    break;
                }
            }

            return HasCEZWord;
        }

        public bool isOther()
        {
            bool isOther = true;

            foreach (var ch in ZoneType)
            {
                if ((ch.ToUpper() == "CEZ") || HasITWordPrivate(ch))
                {
                    isOther = false;
                    break;
                }
            }

            return isOther;
        }

        private bool HasITWordPrivate(string checkIt)
        {
            bool HasITWord = false;
            string[] it = new string[] { "I.T", "i.t", "I.t", "i.T", "IT", "it", "iT", "It" };

            foreach (var ch in ZoneType)
            {
                if (it.Any(m => m.ToString() == checkIt))
                {
                    HasITWord = true;
                    break;
                }
            }

            return HasITWord;
        }
    }
}