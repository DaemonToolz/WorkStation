using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Net.Http;
using WorkstationServices.Data;

namespace WorkstationServices.Security
{

    public class AttributeInspector : Attribute, IParameterInspector, IOperationBehavior
    {
        /// <summary>
        /// Référence vers le service des tokens
        /// </summary>

        static AttributeInspector()
        {
        }
        public AttributeInspector()
        {

        }

        /// <summary>
        /// Non utilisé
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {

        }

        /// <summary>
        /// Non utilisé
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="clientOperation"></param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {

        }

        /// <summary>
        /// Permet d'ajouter au dispatcher des opérations (e.g. géré par le serveur) uun vérificateur
        /// 
        /// </summary>
        /// <param name="operationDescription"> Description de l'opération appelée </param>
        /// <param name="dispatchOperation"> Opération à invoquer </param>
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(this);
        }

        /// <summary>
        /// Validation 
        /// </summary>
        /// <param name="operationDescription"></param>
        public void Validate(OperationDescription operationDescription)
        {

        }


        /// <summary>
        /// Appelé après l'appel de la fonctipn
        /// </summary>
        /// <param name="operationName"> Nom de l'opération invoquée </param>
        /// <param name="outputs"> Sorties </param>
        /// <param name="returnValue"> Variables de retour</param>
        /// <param name="correlationState"> </param>
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {

        }

        /// <summary>
        /// Appelé avant l'invocation
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public object BeforeCall(string operationName, object[] inputs)
        {
            if (inputs == null || inputs.Length == 0)
                throw new FaultException<InputValidationFaultContract>(new InputValidationFaultContract() { Error = "Inputs are required" }, "No inputs");

            String name = inputs[0].ToString(), token = inputs[1].ToString();

            if (name == null || name.Count() == 0 || token == null || token.Count() == 0)
                throw new FaultException<InputValidationFaultContract>(new InputValidationFaultContract() { Error = "No valid data provided" }, "Invalid data");

          
            if (!UserAccessModel.UserExists(name))
                throw new FaultException<InputValidationFaultContract>(new InputValidationFaultContract() { Error = "User not registered in database" }, "User not registered");
        
            if(!Boolean.Parse(ValidateToken("SystemAdministrator", "defaultpassword", token).Result))
                throw new FaultException<InputValidationFaultContract>(new InputValidationFaultContract() { Error = "Invalid Token" }, "Invalid Token");

            return null;
        }


        private async Task<String> ValidateToken(String Username, String Password, String Token)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Workstation Probing Agent");
            client.DefaultRequestHeaders.Add("Username", Username);
            client.DefaultRequestHeaders.Add("Password", Password);
            client.DefaultRequestHeaders.Add("CustomAuthorizers", Token);
            client.DefaultRequestHeaders.Add("CheckExistence", "true");

            Console.WriteLine("Check the Token at http://localhost:15572/api/Tokens/Check");
            var stringTask = client.GetStringAsync("http://localhost:15572/api/Tokens/Check");

            return await stringTask;// (serializer.ReadObject(await streamTask) as MessageContract);

        }

    }

}