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

namespace Ecoporto.CRM.Site.WsIntegraChronos {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="IntegraChronosSoap", Namespace="http://tempuri.org/")]
    public partial class IntegraChronos : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ExportarTabelasOperationCompleted;
        
        private System.Threading.SendOrPostCallback CancelarTabelaOperationCompleted;
        
        private System.Threading.SendOrPostCallback IntregrarFichasChronosOperationCompleted;
        
        private System.Threading.SendOrPostCallback IntregrarAdendosChronosOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public IntegraChronos() {
            this.Url = global::Ecoporto.CRM.Site.Properties.Settings.Default.Ecoporto_CRM_Site_WsIntegraChronos_IntegraChronos;
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
        public event ExportarTabelasCompletedEventHandler ExportarTabelasCompleted;
        
        /// <remarks/>
        public event CancelarTabelaCompletedEventHandler CancelarTabelaCompleted;
        
        /// <remarks/>
        public event IntregrarFichasChronosCompletedEventHandler IntregrarFichasChronosCompleted;
        
        /// <remarks/>
        public event IntregrarAdendosChronosCompletedEventHandler IntregrarAdendosChronosCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ExportarTabelas", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response ExportarTabelas(int oportunidadeId, int usuarioId) {
            object[] results = this.Invoke("ExportarTabelas", new object[] {
                        oportunidadeId,
                        usuarioId});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void ExportarTabelasAsync(int oportunidadeId, int usuarioId) {
            this.ExportarTabelasAsync(oportunidadeId, usuarioId, null);
        }
        
        /// <remarks/>
        public void ExportarTabelasAsync(int oportunidadeId, int usuarioId, object userState) {
            if ((this.ExportarTabelasOperationCompleted == null)) {
                this.ExportarTabelasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnExportarTabelasOperationCompleted);
            }
            this.InvokeAsync("ExportarTabelas", new object[] {
                        oportunidadeId,
                        usuarioId}, this.ExportarTabelasOperationCompleted, userState);
        }
        
        private void OnExportarTabelasOperationCompleted(object arg) {
            if ((this.ExportarTabelasCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ExportarTabelasCompleted(this, new ExportarTabelasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CancelarTabela", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response CancelarTabela(int oportunidadeId, int usuarioId) {
            object[] results = this.Invoke("CancelarTabela", new object[] {
                        oportunidadeId,
                        usuarioId});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void CancelarTabelaAsync(int oportunidadeId, int usuarioId) {
            this.CancelarTabelaAsync(oportunidadeId, usuarioId, null);
        }
        
        /// <remarks/>
        public void CancelarTabelaAsync(int oportunidadeId, int usuarioId, object userState) {
            if ((this.CancelarTabelaOperationCompleted == null)) {
                this.CancelarTabelaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelarTabelaOperationCompleted);
            }
            this.InvokeAsync("CancelarTabela", new object[] {
                        oportunidadeId,
                        usuarioId}, this.CancelarTabelaOperationCompleted, userState);
        }
        
        private void OnCancelarTabelaOperationCompleted(object arg) {
            if ((this.CancelarTabelaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelarTabelaCompleted(this, new CancelarTabelaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IntregrarFichasChronos", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response IntregrarFichasChronos(int oportunidadeId, int fichaId) {
            object[] results = this.Invoke("IntregrarFichasChronos", new object[] {
                        oportunidadeId,
                        fichaId});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void IntregrarFichasChronosAsync(int oportunidadeId, int fichaId) {
            this.IntregrarFichasChronosAsync(oportunidadeId, fichaId, null);
        }
        
        /// <remarks/>
        public void IntregrarFichasChronosAsync(int oportunidadeId, int fichaId, object userState) {
            if ((this.IntregrarFichasChronosOperationCompleted == null)) {
                this.IntregrarFichasChronosOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIntregrarFichasChronosOperationCompleted);
            }
            this.InvokeAsync("IntregrarFichasChronos", new object[] {
                        oportunidadeId,
                        fichaId}, this.IntregrarFichasChronosOperationCompleted, userState);
        }
        
        private void OnIntregrarFichasChronosOperationCompleted(object arg) {
            if ((this.IntregrarFichasChronosCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IntregrarFichasChronosCompleted(this, new IntregrarFichasChronosCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IntregrarAdendosChronos", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response IntregrarAdendosChronos(int oportunidadeId, int adendoId) {
            object[] results = this.Invoke("IntregrarAdendosChronos", new object[] {
                        oportunidadeId,
                        adendoId});
            return ((Response)(results[0]));
        }
        
        /// <remarks/>
        public void IntregrarAdendosChronosAsync(int oportunidadeId, int adendoId) {
            this.IntregrarAdendosChronosAsync(oportunidadeId, adendoId, null);
        }
        
        /// <remarks/>
        public void IntregrarAdendosChronosAsync(int oportunidadeId, int adendoId, object userState) {
            if ((this.IntregrarAdendosChronosOperationCompleted == null)) {
                this.IntregrarAdendosChronosOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIntregrarAdendosChronosOperationCompleted);
            }
            this.InvokeAsync("IntregrarAdendosChronos", new object[] {
                        oportunidadeId,
                        adendoId}, this.IntregrarAdendosChronosOperationCompleted, userState);
        }
        
        private void OnIntregrarAdendosChronosOperationCompleted(object arg) {
            if ((this.IntregrarAdendosChronosCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IntregrarAdendosChronosCompleted(this, new IntregrarAdendosChronosCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Response {
        
        private bool sucessoField;
        
        private string mensagemField;
        
        private Tabela[] listaField;
        
        private int tabelaIdField;
        
        private int arquivoIdField;
        
        private string hashField;
        
        private string nomeArquivoField;
        
        private int tamanhoArquivoField;
        
        private string base64Field;
        
        private int simuladorIdField;
        
        /// <remarks/>
        public bool Sucesso {
            get {
                return this.sucessoField;
            }
            set {
                this.sucessoField = value;
            }
        }
        
        /// <remarks/>
        public string Mensagem {
            get {
                return this.mensagemField;
            }
            set {
                this.mensagemField = value;
            }
        }
        
        /// <remarks/>
        public Tabela[] Lista {
            get {
                return this.listaField;
            }
            set {
                this.listaField = value;
            }
        }
        
        /// <remarks/>
        public int TabelaId {
            get {
                return this.tabelaIdField;
            }
            set {
                this.tabelaIdField = value;
            }
        }
        
        /// <remarks/>
        public int ArquivoId {
            get {
                return this.arquivoIdField;
            }
            set {
                this.arquivoIdField = value;
            }
        }
        
        /// <remarks/>
        public string Hash {
            get {
                return this.hashField;
            }
            set {
                this.hashField = value;
            }
        }
        
        /// <remarks/>
        public string NomeArquivo {
            get {
                return this.nomeArquivoField;
            }
            set {
                this.nomeArquivoField = value;
            }
        }
        
        /// <remarks/>
        public int TamanhoArquivo {
            get {
                return this.tamanhoArquivoField;
            }
            set {
                this.tamanhoArquivoField = value;
            }
        }
        
        /// <remarks/>
        public string Base64 {
            get {
                return this.base64Field;
            }
            set {
                this.base64Field = value;
            }
        }
        
        /// <remarks/>
        public int SimuladorId {
            get {
                return this.simuladorIdField;
            }
            set {
                this.simuladorIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Tabela {
        
        private int simuladorIdField;
        
        private int tabelaIdField;
        
        private string descricaoField;
        
        private string importadorField;
        
        private string despachanteField;
        
        private string nVOCCField;
        
        private string coloaderField;
        
        private string coColoaderField;
        
        private string coColoader2Field;
        
        private string propostaField;
        
        private System.Nullable<System.DateTime> dataInicioField;
        
        private System.Nullable<System.DateTime> dataFinalValidadeField;
        
        /// <remarks/>
        public int SimuladorId {
            get {
                return this.simuladorIdField;
            }
            set {
                this.simuladorIdField = value;
            }
        }
        
        /// <remarks/>
        public int TabelaId {
            get {
                return this.tabelaIdField;
            }
            set {
                this.tabelaIdField = value;
            }
        }
        
        /// <remarks/>
        public string Descricao {
            get {
                return this.descricaoField;
            }
            set {
                this.descricaoField = value;
            }
        }
        
        /// <remarks/>
        public string Importador {
            get {
                return this.importadorField;
            }
            set {
                this.importadorField = value;
            }
        }
        
        /// <remarks/>
        public string Despachante {
            get {
                return this.despachanteField;
            }
            set {
                this.despachanteField = value;
            }
        }
        
        /// <remarks/>
        public string NVOCC {
            get {
                return this.nVOCCField;
            }
            set {
                this.nVOCCField = value;
            }
        }
        
        /// <remarks/>
        public string Coloader {
            get {
                return this.coloaderField;
            }
            set {
                this.coloaderField = value;
            }
        }
        
        /// <remarks/>
        public string CoColoader {
            get {
                return this.coColoaderField;
            }
            set {
                this.coColoaderField = value;
            }
        }
        
        /// <remarks/>
        public string CoColoader2 {
            get {
                return this.coColoader2Field;
            }
            set {
                this.coColoader2Field = value;
            }
        }
        
        /// <remarks/>
        public string Proposta {
            get {
                return this.propostaField;
            }
            set {
                this.propostaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> DataInicio {
            get {
                return this.dataInicioField;
            }
            set {
                this.dataInicioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> DataFinalValidade {
            get {
                return this.dataFinalValidadeField;
            }
            set {
                this.dataFinalValidadeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void ExportarTabelasCompletedEventHandler(object sender, ExportarTabelasCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExportarTabelasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ExportarTabelasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void CancelarTabelaCompletedEventHandler(object sender, CancelarTabelaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelarTabelaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CancelarTabelaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void IntregrarFichasChronosCompletedEventHandler(object sender, IntregrarFichasChronosCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IntregrarFichasChronosCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal IntregrarFichasChronosCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void IntregrarAdendosChronosCompletedEventHandler(object sender, IntregrarAdendosChronosCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IntregrarAdendosChronosCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal IntregrarAdendosChronosCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591