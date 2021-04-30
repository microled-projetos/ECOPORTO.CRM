using Ecoporto.CRM.Infra.Configuracao;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Ecoporto.CRM.Site.App_Start
{
    public static class NLogConfig
    {
        public static LoggingConfiguration Configure()
        {
            var config = new LoggingConfiguration();

            var target = new FileTarget();

            var dbTarget = new DatabaseTarget
            {
                KeepConnection = false,
                DBProvider = "Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess",
                ConnectionString = Config.StringConexao(),
                CommandText = @"INSERT INTO TB_CRM_AUDITORIA (ID, DATA, NIVEL, MENSAGEM, MAQUINA, USUARIO, ORIGEM, EXCEPTION, STACKTRACE, TICKET, ACAO, OBJETO, CHAVE, ACTION, CONTROLLER, METODO, IP, REQUEST, CHAVEPAI) VALUES (SEQ_CRM_AUDITORIA.NEXTVAL, :DATA, :NIVEL, :MENSAGEM, :MAQUINA, :USUARIO, :ORIGEM, :EXCEPTION, :STACKTRACE, :TICKET, :ACAO, :OBJETO, :CHAVE, :ACTION, :CONTROLLER, :METODO, :IP, :REQUEST, :CHAVEPAI)"
            };

            dbTarget.Parameters.Add(new DatabaseParameterInfo("DATA", new NLog.Layouts.SimpleLayout("${longdate}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("NIVEL", new NLog.Layouts.SimpleLayout("${level}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("MENSAGEM", new NLog.Layouts.SimpleLayout("${message}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("MAQUINA", new NLog.Layouts.SimpleLayout("${machinename}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("USUARIO", new NLog.Layouts.SimpleLayout("${aspnet-user-identity}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("ORIGEM", new NLog.Layouts.SimpleLayout("${callsite}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("EXCEPTION", new NLog.Layouts.SimpleLayout("${exception:format=toString}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("STACKTRACE", new NLog.Layouts.SimpleLayout("${stacktrace}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("TICKET", new NLog.Layouts.SimpleLayout("${gdc:ticket}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("ACAO", new NLog.Layouts.SimpleLayout("${gdc:acao}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("OBJETO", new NLog.Layouts.SimpleLayout("${gdc:objeto}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("CHAVE", new NLog.Layouts.SimpleLayout("${gdc:chave}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("ACTION", new NLog.Layouts.SimpleLayout("${aspnet-mvc-action}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("CONTROLLER", new NLog.Layouts.SimpleLayout("${aspnet-mvc-controller}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("METODO", new NLog.Layouts.SimpleLayout("${aspnet-request-method")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("IP", new NLog.Layouts.SimpleLayout("${aspnet-request-ip}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("REQUEST", new NLog.Layouts.SimpleLayout("${aspnet-request:serverVariable=HTTP_URL}${aspnet-request:queryString}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("CHAVEPAI", new NLog.Layouts.SimpleLayout("${gdc:chavePai}")));
            
            config.AddTarget("async", new AsyncTargetWrapper(dbTarget));
            config.AddTarget("database", dbTarget);
            
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, dbTarget));
            
            return config;
        }
    }
}