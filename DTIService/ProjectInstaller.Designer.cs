namespace DTIService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.DTIServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DTIService = new System.ServiceProcess.ServiceInstaller();
            // 
            // DTIServiceInstaller
            // 
            this.DTIServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DTIServiceInstaller.Password = null;
            this.DTIServiceInstaller.Username = null;
            this.DTIServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // DTIService
            // 
            this.DTIService.Description = "Serviços da DTI CLN.";
            this.DTIService.DisplayName = "DTIServices";
            this.DTIService.ServiceName = "DTIServices";
            this.DTIService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.DTIService.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DTIServiceInstaller,
            this.DTIService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DTIServiceInstaller;
        private System.ServiceProcess.ServiceInstaller DTIService;
    }
}