using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace DomainHackGen
{
    class Program
    {
        //static string[] tlds = new string[] { "AC", "AD", "AE", "AERO", "AF", "AG", "AI", "AL", "AM", "AN", "AO", "AQ", "AR", "ARPA", "AS", "ASIA", "AT", "AU", "AW", "AX", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BIZ", "BJ", "BM", "BN", "BO", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CAT", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "COM", "COOP", "CR", "CU", "CV", "CW", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EDU", "EE", "EG", "ER", "ES", "ET", "EU", "FI", "FJ", "FK", "FM", "FO", "FR", "GA", "GB", "GD", "GE", "GF", "GG", "GH", "GI", "GL", "GM", "GN", "GOV", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID", "IE", "IL", "IM", "IN", "INFO", "INT", "IO", "IQ", "IR", "IS", "IT", "JE", "JM", "JO", "JOBS", "JP", "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "ME", "MG", "MH", "MIL", "MK", "ML", "MM", "MN", "MO", "MOBI", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NAME", "NC", "NE", "NET", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "ORG", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PRO", "PS", "PT", "PW", "PY", "QA", "RE", "RO", "RS", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SU", "SV", "SX", "SY", "SZ", "TC", "TD", "TEL", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TO", "TP", "TR", "TT", "TV", "TW", "TZ", "UA", "UG", "UK", "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "XXX", "YE", "YT", "ZA", "ZM", "ZW" };
        //static string[] tlds = new string[] {"af", "netaf", "comaf", "am", "coat", "orat", "cc","cocc", "cn", "cx", "ie", "coid", "mk", "mo", "mobi", "nc", "comng", "no", "pl", "tv", "tw", "ua", "couk", "meuk" };
        static string[] tlds = new string[] { "be", "de", "ee", "eg", "fi", "cl", "ie", "it", "pe", "ru" };
        static void Main(string[] args)
        {
            int AttemptCount = 0;
            string word;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader("../wordlist.txt");
            while ((word = file.ReadLine()) != null)
            {
                foreach (string prefix in tlds)
                {
                    if(word.EndsWith(prefix.ToLower()))
                    {
                        Console.Title = "" + AttemptCount + " Attempting: " + word.Substring(0, word.Length - prefix.Length) + "." + prefix.ToLower();
                        if ((word.Substring(0, word.Length - prefix.Length)).Length > 2)
                        {
                            if (!IsValidDomainName(word.Substring(0, word.Length - prefix.Length) + "." + prefix.ToLower()))
                            {
                                if (Whois_lookup(word.Substring(0, word.Length - prefix.Length) + "." + prefix.ToLower(), RecordType.domain).Contains("No match for domain"))
                                {
                                    Console.WriteLine(word.Substring(0, word.Length - prefix.Length) + "." + prefix.ToLower());
                                }
                            }
                        }
                    }
                }
                AttemptCount++;
            }
            file.Close();

            //Console.WriteLine(IsValidDomainName("benjojo12.co.uk"));
            Console.Read();
        }
        public enum RecordType { domain, nameserver, registrar };
        public static string Whois_lookup(string domainname, RecordType recordType)
        {
            string whois_server_address = "whois.internic.net";
            TcpClient tcp = new TcpClient();
            tcp.Connect(whois_server_address, 43);
            string strDomain = recordType.ToString() + " " + domainname + "\r\n";
            byte[] bytDomain = Encoding.ASCII.GetBytes(strDomain.ToCharArray());
            Stream s = tcp.GetStream();
            s.Write(bytDomain, 0, strDomain.Length);
            StreamReader sr = new StreamReader(tcp.GetStream(), Encoding.ASCII);
            string final = "";
            string strLine = "";
            List<string> result = new List<string>();
            while (null != (strLine = sr.ReadLine()))
            {
                final = final + strLine + "\n";
            }
            tcp.Close();
            return final;
        }




        private static bool IsValidDomainName(string name)
        {
            try
            {
                IPAddress[] addresslist = Dns.GetHostAddresses(name);
                return true;
            }
            catch
            {
                return false;
            }
            /*foreach (IPAddress theaddress in addresslist)
            {
                Console.WriteLine(theaddress.ToString());
            }*/

        }
    }
}
