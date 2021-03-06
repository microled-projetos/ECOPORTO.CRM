//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace Ecoporto.CRM.Site.WsControleSenha {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="CriptografiaSoap", Namespace="http://tempuri.org/")]
    public partial class Criptografia : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback EncriptarTabelaOperationCompleted;
        
        private System.Threading.SendOrPostCallback DescriptarTabelaOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObterVersaoOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObterTabelaLoginOperationCompleted;
        
        private System.Threading.SendOrPostCallback usuLogOutOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidarUsuarioOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidarUsuarioCPFOperationCompleted;
        
        private System.Threading.SendOrPostCallback alterarSenhaOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Criptografia() {
            this.Url = global::Ecoporto.CRM.Site.Properties.Settings.Default.Ecoporto_CRM_Site_WsControleSenha_Criptografia;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event EncriptarTabelaCompletedEventHandler EncriptarTabelaCompleted;
        
        /// <remarks/>
        public event DescriptarTabelaCompletedEventHandler DescriptarTabelaCompleted;
        
        /// <remarks/>
        public event ObterVersaoCompletedEventHandler ObterVersaoCompleted;
        
        /// <remarks/>
        public event ObterTabelaLoginCompletedEventHandler ObterTabelaLoginCompleted;
        
        /// <remarks/>
        public event usuLogOutCompletedEventHandler usuLogOutCompleted;
        
        /// <remarks/>
        public event ValidarUsuarioCompletedEventHandler ValidarUsuarioCompleted;
        
        /// <remarks/>
        public event ValidarUsuarioCPFCompletedEventHandler ValidarUsuarioCPFCompleted;
        
        /// <remarks/>
        public event alterarSenhaCompletedEventHandler alterarSenhaCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/EncriptarTabela", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string EncriptarTabela(string nomeSistema) {
            object[] results = this.Invoke("EncriptarTabela", new object[] {
                        nomeSistema});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void EncriptarTabelaAsync(string nomeSistema) {
            this.EncriptarTabelaAsync(nomeSistema, null);
        }
        
        /// <remarks/>
        public void EncriptarTabelaAsync(string nomeSistema, object userState) {
            if ((this.EncriptarTabelaOperationCompleted == null)) {
                this.EncriptarTabelaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEncriptarTabelaOperationCompleted);
            }
            this.InvokeAsync("EncriptarTabela", new object[] {
                        nomeSistema}, this.EncriptarTabelaOperationCompleted, userState);
        }
        
        private void OnEncriptarTabelaOperationCompleted(object arg) {
            if ((this.EncriptarTabelaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EncriptarTabelaCompleted(this, new EncriptarTabelaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DescriptarTabela", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string DescriptarTabela(string nomeSistema) {
            object[] results = this.Invoke("DescriptarTabela", new object[] {
                        nomeSistema});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void DescriptarTabelaAsync(string nomeSistema) {
            this.DescriptarTabelaAsync(nomeSistema, null);
        }
        
        /// <remarks/>
        public void DescriptarTabelaAsync(string nomeSistema, object userState) {
            if ((this.DescriptarTabelaOperationCompleted == null)) {
                this.DescriptarTabelaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDescriptarTabelaOperationCompleted);
            }
            this.InvokeAsync("DescriptarTabela", new object[] {
                        nomeSistema}, this.DescriptarTabelaOperationCompleted, userState);
        }
        
        private void OnDescriptarTabelaOperationCompleted(object arg) {
            if ((this.DescriptarTabelaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DescriptarTabelaCompleted(this, new DescriptarTabelaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ObterVersao", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObterVersao(string nomeSistema) {
            object[] results = this.Invoke("ObterVersao", new object[] {
                        nomeSistema});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObterVersaoAsync(string nomeSistema) {
            this.ObterVersaoAsync(nomeSistema, null);
        }
        
        /// <remarks/>
        public void ObterVersaoAsync(string nomeSistema, object userState) {
            if ((this.ObterVersaoOperationCompleted == null)) {
                this.ObterVersaoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObterVersaoOperationCompleted);
            }
            this.InvokeAsync("ObterVersao", new object[] {
                        nomeSistema}, this.ObterVersaoOperationCompleted, userState);
        }
        
        private void OnObterVersaoOperationCompleted(object arg) {
            if ((this.ObterVersaoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObterVersaoCompleted(this, new ObterVersaoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ObterTabelaLogin", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObterTabelaLogin(string nomeSistema) {
            object[] results = this.Invoke("ObterTabelaLogin", new object[] {
                        nomeSistema});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObterTabelaLoginAsync(string nomeSistema) {
            this.ObterTabelaLoginAsync(nomeSistema, null);
        }
        
        /// <remarks/>
        public void ObterTabelaLoginAsync(string nomeSistema, object userState) {
            if ((this.ObterTabelaLoginOperationCompleted == null)) {
                this.ObterTabelaLoginOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObterTabelaLoginOperationCompleted);
            }
            this.InvokeAsync("ObterTabelaLogin", new object[] {
                        nomeSistema}, this.ObterTabelaLoginOperationCompleted, userState);
        }
        
        private void OnObterTabelaLoginOperationCompleted(object arg) {
            if ((this.ObterTabelaLoginCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObterTabelaLoginCompleted(this, new ObterTabelaLoginCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/usuLogOut", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string usuLogOut(string tabela, string usuario) {
            object[] results = this.Invoke("usuLogOut", new object[] {
                        tabela,
                        usuario});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void usuLogOutAsync(string tabela, string usuario) {
            this.usuLogOutAsync(tabela, usuario, null);
        }
        
        /// <remarks/>
        public void usuLogOutAsync(string tabela, string usuario, object userState) {
            if ((this.usuLogOutOperationCompleted == null)) {
                this.usuLogOutOperationCompleted = new System.Threading.SendOrPostCallback(this.OnusuLogOutOperationCompleted);
            }
            this.InvokeAsync("usuLogOut", new object[] {
                        tabela,
                        usuario}, this.usuLogOutOperationCompleted, userState);
        }
        
        private void OnusuLogOutOperationCompleted(object arg) {
            if ((this.usuLogOutCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.usuLogOutCompleted(this, new usuLogOutCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ValidarUsuario", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ValidarUsuario(string tabela, string usuario, string senha) {
            object[] results = this.Invoke("ValidarUsuario", new object[] {
                        tabela,
                        usuario,
                        senha});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ValidarUsuarioAsync(string tabela, string usuario, string senha) {
            this.ValidarUsuarioAsync(tabela, usuario, senha, null);
        }
        
        /// <remarks/>
        public void ValidarUsuarioAsync(string tabela, string usuario, string senha, object userState) {
            if ((this.ValidarUsuarioOperationCompleted == null)) {
                this.ValidarUsuarioOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidarUsuarioOperationCompleted);
            }
            this.InvokeAsync("ValidarUsuario", new object[] {
                        tabela,
                        usuario,
                        senha}, this.ValidarUsuarioOperationCompleted, userState);
        }
        
        private void OnValidarUsuarioOperationCompleted(object arg) {
            if ((this.ValidarUsuarioCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidarUsuarioCompleted(this, new ValidarUsuarioCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ValidarUsuarioCPF", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ValidarUsuarioCPF(string tabela, string usuario, string cpf) {
            object[] results = this.Invoke("ValidarUsuarioCPF", new object[] {
                        tabela,
                        usuario,
                        cpf});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ValidarUsuarioCPFAsync(string tabela, string usuario, string cpf) {
            this.ValidarUsuarioCPFAsync(tabela, usuario, cpf, null);
        }
        
        /// <remarks/>
        public void ValidarUsuarioCPFAsync(string tabela, string usuario, string cpf, object userState) {
            if ((this.ValidarUsuarioCPFOperationCompleted == null)) {
                this.ValidarUsuarioCPFOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidarUsuarioCPFOperationCompleted);
            }
            this.InvokeAsync("ValidarUsuarioCPF", new object[] {
                        tabela,
                        usuario,
                        cpf}, this.ValidarUsuarioCPFOperationCompleted, userState);
        }
        
        private void OnValidarUsuarioCPFOperationCompleted(object arg) {
            if ((this.ValidarUsuarioCPFCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidarUsuarioCPFCompleted(this, new ValidarUsuarioCPFCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/alterarSenha", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string alterarSenha(string novaSenha, string tabela, string login) {
            object[] results = this.Invoke("alterarSenha", new object[] {
                        novaSenha,
                        tabela,
                        login});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void alterarSenhaAsync(string novaSenha, string tabela, string login) {
            this.alterarSenhaAsync(novaSenha, tabela, login, null);
        }
        
        /// <remarks/>
        public void alterarSenhaAsync(string novaSenha, string tabela, string login, object userState) {
            if ((this.alterarSenhaOperationCompleted == null)) {
                this.alterarSenhaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnalterarSenhaOperationCompleted);
            }
            this.InvokeAsync("alterarSenha", new object[] {
                        novaSenha,
                        tabela,
                        login}, this.alterarSenhaOperationCompleted, userState);
        }
        
        private void OnalterarSenhaOperationCompleted(object arg) {
            if ((this.alterarSenhaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.alterarSenhaCompleted(this, new alterarSenhaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void EncriptarTabelaCompletedEventHandler(object sender, EncriptarTabelaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EncriptarTabelaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EncriptarTabelaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void DescriptarTabelaCompletedEventHandler(object sender, DescriptarTabelaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DescriptarTabelaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DescriptarTabelaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void ObterVersaoCompletedEventHandler(object sender, ObterVersaoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObterVersaoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObterVersaoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void ObterTabelaLoginCompletedEventHandler(object sender, ObterTabelaLoginCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObterTabelaLoginCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObterTabelaLoginCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void usuLogOutCompletedEventHandler(object sender, usuLogOutCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class usuLogOutCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal usuLogOutCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void ValidarUsuarioCompletedEventHandler(object sender, ValidarUsuarioCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidarUsuarioCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidarUsuarioCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void ValidarUsuarioCPFCompletedEventHandler(object sender, ValidarUsuarioCPFCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidarUsuarioCPFCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidarUsuarioCPFCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void alterarSenhaCompletedEventHandler(object sender, alterarSenhaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class alterarSenhaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal alterarSenhaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591