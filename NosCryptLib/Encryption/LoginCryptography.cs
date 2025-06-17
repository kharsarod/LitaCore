using System.Text;

namespace NosCryptLib.Encryption
{
    public class LoginCryptography
    {
        #region Methods

        public static string GetPassword(string password)
        {
            bool equal = password.Length % 2 == 0;
            string str = equal ? password.Remove(0, 3) : password.Remove(0, 4);
            StringBuilder decryptpass = new StringBuilder();

            for (int i = 0; i < str.Length; i += 2)
            {
                decryptpass.Append(str[i]);
            }
            if (decryptpass.Length % 2 != 0)
            {
                str = password.Remove(0, 2);
                decryptpass = decryptpass.Clear();
                for (int i = 0; i < str.Length; i += 2)
                {
                    decryptpass.Append(str[i]);
                }
            }

            StringBuilder passwd = new StringBuilder();
            for (int i = 0; i < decryptpass.Length; i += 2)
            {
                passwd.Append(Convert.ToChar(Convert.ToUInt32(decryptpass.ToString().Substring(i, 2), 16)));
            }
            return passwd.ToString();
        }

        public string Decrypt(byte[] data)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                foreach (byte character in data)
                {
                    if (character > 14)
                    {
                        builder.Append(Convert.ToChar((character - 15) ^ 195));
                    }
                    else
                    {
                        builder.Append(Convert.ToChar((256 - (15 - character)) ^ 195));
                    }
                }
                return builder.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string DecryptCustomParameter(byte[] data) => throw new NotImplementedException();

        public byte[] Encrypt(string data)
        {
            try
            {
                data += " ";
                byte[] tmp = Encoding.Default.GetBytes(data);
                for (int i = 0; i < data.Length; i++)
                {
                    tmp[i] = Convert.ToByte(tmp[i] + 15);
                }
                tmp[tmp.Length - 1] = 25;
                return tmp;
            }
            catch
            {
                return new byte[0];
            }
        }

        #endregion Methods
    }
}