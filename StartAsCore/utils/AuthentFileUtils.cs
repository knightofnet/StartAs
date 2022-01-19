using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using StartAsCore.dto;

namespace StartAsCore.utils
{
    public static class AuthentFileUtils
    {

        public static bool CreateFile(AuthentFile authentFile, string authentFilepath)
        {
            FileInfo fi = new FileInfo(authentFile.Filepath);
            authentFile.FilepathLength = fi.Length;

            if (authentFile.IsDoSha1VerifAtStart)
            {
                String[] strIntegrity = FileIntegrityUtils.CalculateFileIntegrity(fi);
               
                authentFile.ChecksumSha1 = strIntegrity[0];
                authentFile.ChecksumCrc32 = strIntegrity[1];
            }

            AuthentFileUtils.CryptAuthenDtoToFile(authentFile, authentFilepath);

            return true;

        }

       

        public static string CryptAuthenDtoToString(AuthentFile authentFile)
        {
            XmlSerializer serializer = new XmlSerializer(authentFile.GetType());
            String xml;
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, new XmlWriterSettings() { Indent = false }))
                {
                    serializer.Serialize(writer, authentFile);
                    xml = sww.ToString();
                }
            }

            return StringCipher.Encrypt(xml, MiscAppUtils.GetComputerSid().Value);
        }

        public static void CryptAuthenDtoToFile(AuthentFile authentFile, String cryptFilePath)
        {
            try
            {
                String content = CryptAuthenDtoToString(authentFile);
                File.WriteAllText(cryptFilePath, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static AuthentFile CryptAuthenDtoFromFile(string authentFilepath)
        {
            String content = File.ReadAllText(authentFilepath, Encoding.UTF8);
            return CryptAuthenDtoFromString(content);
        }

        public static AuthentFile CryptAuthenDtoFromString(string content)
        {
            try
            {

                string locContent = StringCipher.Decrypt(content, MiscAppUtils.GetComputerSid().Value);

                AuthentFile authentFile;
                using (TextReader reader = new StringReader(locContent))
                {
                    authentFile = (AuthentFile)new XmlSerializer(typeof(AuthentFile)).Deserialize(reader);
                }

                return authentFile;
            }
            catch (CryptographicException ex)
            {

                throw new Exception("Error when decrypting file");
            }
        }
    }
}
