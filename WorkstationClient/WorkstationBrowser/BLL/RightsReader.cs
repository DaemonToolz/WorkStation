using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.BLL {
    public static class RightsReader {
        public static IDictionary<String, bool> Decode(String rights)
        {
            var rightsArray = rights.Select(chr => chr == '1').ToArray();
            return new Dictionary<String, bool>()
            {
                {"IsAdmin", rightsArray[0]},
                {"CanReadDept", rightsArray[1]},
                {"CanEditDept", rightsArray[2]},
                {"CanReadProj", rightsArray[3]},
                {"CanEditProj", rightsArray[4]},
                {"CanReadTeam", rightsArray[5]},
                {"CanEditTeam", rightsArray[6]},
                {"CanReadUser", rightsArray[7]},
                {"CanEditUser", rightsArray[8]}

            };
        }

        public static String Encode(IDictionary<String, bool> rights){
            String encoded = (Int32.Parse(rights["IsAdmin"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanReadDept"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditDept"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanReadProj"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditProj"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanReadTeam"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditTeam"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanReadUser"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditUser"].ToString())).ToString();
            return encoded;
        }
    }
}