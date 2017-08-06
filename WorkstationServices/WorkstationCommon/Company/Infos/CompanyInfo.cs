using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Workstation.Company.Infos
{
    public static class CompanyInfoUtil
    {
        public static CompanyInfo CompanyClaims;

        static CompanyInfoUtil()
        {
            //LoadCompanyInfos("./CompanyInfo.json");
        }

        public static void LoadCompanyInfos(String filename)
        {
            using (StreamReader sr = new StreamReader(filename))
                CompanyClaims = JsonConvert.DeserializeObject<CompanyInfo>(sr.ReadToEnd());
        }
    }

    public class CompanyInfo
    {
        public String[] ValidAudiences { get; set; }
        public String[] ValidIssuers { get; set; }
        public Dictionary<String, String> Claims { get; set; }
    }
}
