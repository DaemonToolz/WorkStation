using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IO;
using System.ServiceModel;

namespace WorkstationServices.Security
{
    internal class ConnectionValidationProcess : UserNamePasswordValidator
    {

        static ConnectionValidationProcess()
        {

        }

        /// <summary>
        /// Valide ou non la connection
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public override void Validate(string userName, string password)
        {
            try
            {
                if (!UserAccessModel.UserValid(userName, password))
                    throw new FaultException("Either the user or the password is invalid");
            }
            catch (FaultException fe)
            {
                throw fe;
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }
}