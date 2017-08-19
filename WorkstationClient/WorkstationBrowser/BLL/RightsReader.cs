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
                {"CanEditUser", rightsArray[8]},
                {"CanReadTask", rightsArray[9]},
                {"CanEditTask", rightsArray[10]},
                {"CanReadMesg", rightsArray[11]},
                {"CanEditMesg", rightsArray[12]},
                {"CanReadNoti", rightsArray[13]},
                {"CanEditNoti", rightsArray[14]},

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

            encoded += (Int32.Parse(rights["CanReadTask"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditTask"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanReadMesg"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditMesg"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanReadNoti"].ToString())).ToString();
            encoded += (Int32.Parse(rights["CanEditNoti"].ToString())).ToString();

            return encoded;
        }
    }
}