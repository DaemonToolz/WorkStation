using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using WorkstationDatabaseMaintenanceService.DAL.Token;

namespace WorkstationDatabaseMaintenanceService
{
    partial class TokenCleanUpService : ServiceBase
    {
        private TokensEntities _TokenEF;
        private Timer TokenCleanUpTrigger;
        public TokenCleanUpService(){
            InitializeComponent();

            TokenCleanUpLogger.WriteEntry($@"[{DateTime.Now}] - Initializing Service");
            _TokenEF = new TokensEntities();

            TokenCleanUpTrigger = new Timer(60000);
            TokenCleanUpTrigger.Elapsed += new ElapsedEventHandler((sender, eventargs)=>CheckTokenStates());
        }

        protected override void OnStart(string[] args)
        {
            TokenCleanUpLogger.WriteEntry($@"[{DateTime.Now}] - Starting Service");

            TokenCleanUpTrigger.Enabled = true;
            TokenCleanUpTrigger.Start();
        }

        protected override void OnStop()
        {
            TokenCleanUpLogger.WriteEntry($@"[{DateTime.Now}] - Stopping Service");

            // Explicity closing the connection
            TokenCleanUpTrigger.Stop();
            _TokenEF.Database.Connection.Close();
        }

        private void CheckTokenStates()
        {
            try
            {
                int changes = (_TokenEF.Token.RemoveRange(
                    _TokenEF.Token.Where(token => token.exp != null && token.exp < DateTime.Now))).Count();
                _TokenEF.SaveChanges();

                TokenCleanUpLogger.WriteEntry($@"[{DateTime.Now}] - Cleaning operation terminated with {changes} modifications.");

            }
            catch (Exception ex)
            {
                TokenCleanUpLogger.WriteEntry(ex.Message);   
                OnStop();
            }

        }
    }
}
