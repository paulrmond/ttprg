using BCS.Models.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BCS.Controllers
{
    public class CryptoController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCryptography()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Encrypt(string val, string ekey)
        {
            Encrypt en = new Encrypt(val);
            string a = en.EncryptedValue;
            string encrypted = "";
            string pass = val;
            string EncriptionKey = ekey;
            byte[] clearBytes = Encoding.Unicode.GetBytes(pass);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncriptionKey, new byte[]
                {
                    0x49,0x76,0x61,0x6e,0x20,0x4d,0x65,
                    0x64,0x76,0x65,0x64,0x65,0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encrypted = Convert.ToBase64String(ms.ToArray());
                }
            }
            return Json(new { encrypted }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Decrypt(string val, string ekey)
        {
            string encrypted = "";
            string EncriptionKey = ekey;
            byte[] cipherBytes = Convert.FromBase64String(val);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncriptionKey, new byte[]
                {
                    0x49,0x76,0x61,0x6e,0x20,0x4d,0x65,
                    0x64,0x76,0x65,0x64,0x65,0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encrypted = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return Json(new { encrypted }, JsonRequestBehavior.AllowGet);
        }

    }
}