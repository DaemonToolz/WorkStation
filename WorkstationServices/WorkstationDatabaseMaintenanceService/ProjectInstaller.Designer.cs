namespace WorkstationDatabaseMaintenanceService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.WorkstationProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.WorkstationTokenService = new System.ServiceProcess.ServiceInstaller();
            // 
            // WorkstationProcessInstaller
            // 
            this.WorkstationProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.WorkstationProcessInstaller.Password = null;
            this.WorkstationProcessInstaller.Username = null;
            // 
            // WorkstationTokenService
            // 
            this.WorkstationTokenService.DisplayName = "WTCS";
            this.WorkstationTokenService.ServiceName = "TokenCleanUpService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WorkstationProcessInstaller,
            this.WorkstationTokenService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller WorkstationProcessInstaller;
        private System.ServiceProcess.ServiceInstaller WorkstationTokenService;
    }
}