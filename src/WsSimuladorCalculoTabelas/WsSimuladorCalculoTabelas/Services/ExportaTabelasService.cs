using Ecoporto.CRM.Business.Enums;
using System.Collections.Generic;
using System.Linq;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Enums;
using WsSimuladorCalculoTabelas.Extensions;
using WsSimuladorCalculoTabelas.Helpers;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.Services
{
    public class ExportaTabelasService
    {
        private readonly OportunidadeDAO _oportunidadeDAO;
        private readonly TabelasDAO _tabelaDAO;
        private readonly int _oportunidadeId;
        private readonly List<string> tiposCargas = new List<string>() { "SVAR20", "SVAR40" };
        private readonly List<string> variantes = new List<string>() { "MDIR", "MESQ", "ENTR" };
        private readonly bool _crm;
       


        public ExportaTabelasService(int oportunidadeId, bool crm)
        {
            _oportunidadeDAO = new OportunidadeDAO();
            _tabelaDAO = new TabelasDAO(crm);
            _oportunidadeId = oportunidadeId;

            _crm = crm;
        }

        public void ExportarServicos(int tabelaCobranca)
        { 
          bool _temgrc;
          bool _temminimo;
            int _qtdedias;

            _tabelaDAO.DeletarServicosFixosVariaveis(_oportunidadeId);
            
            var linhas = _oportunidadeDAO.ObterLayoutProposta(_oportunidadeId);
               _temgrc = false;
            _temminimo = false;
            _qtdedias = 0;

            foreach (var layout in linhas)
            {
                var servicosIPA = _oportunidadeDAO.ObterServicosIPA(layout.ServicoId);

                switch (layout.TipoRegistro)
                {
                    case TipoRegistro.ARMAZENAGEM:
                    case TipoRegistro.ARMAZENAGEM_CIF:


                        var objArmazenagem = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            Periodo = layout.Periodo,
                            QtdeDias = layout.QtdeDias,
                            VarianteLocal = ConverteMargemIPA.MargemIPA(layout.Margem),
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            ValorExcesso = layout.ValorExcesso,
                            ProRata = layout.ProRata,
                            GrupoAtracacaoId = layout.GrupoAtracacaoId,
                            ValorAcrescimoPeso = layout.AdicionalPeso,
                            ValorAcrescimo = layout.AdicionalIMO,
                            Exercito=layout.Exercito,
                            PesoLimite = layout.PesoLimite,
                            TipoDocumentoId = layout.TipoDocumentoId,
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                            BaseExcesso = layout.BaseExcesso.ToName().ToUpper(),
                            PrecoMaximo = 0,
                            CobrarNVOCC = false,
                            AdicionalGRC = layout.AdicionalGRC,
                            MinimoAnvisaGRC = layout.ANVISAGRC,
                            ValorCif = layout.ValorCif
                        };


                        if (layout.Exercito > 0)
                        {
                            objArmazenagem.Exercito = layout.Exercito;
                        }
                        if (layout.AdicionalIMO > 0)
                        {
                            objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                        }
                        if (layout.Exercito > 0)
                        {
                            objArmazenagem.Exercito = layout.Exercito;
                        }

                        if (layout.ValorANVISA > 0)
                        {
                            objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                        }

                        if (layout.TipoCarga == TipoCarga.CONTEINER)
                        {
                            foreach (var tipoCarga in tiposCargas)
                            {
                                objArmazenagem.TipoCarga = tipoCarga;
                                objArmazenagem.Linha = layout.Linha;

                                foreach (var servico in servicosIPA)
                                {
                                    objArmazenagem.ServicoId = servico.Id;

                                    if (tipoCarga == "SVAR20")
                                    {
                                        objArmazenagem.PrecoUnitario = layout.Valor20;
                                    }
                                    else
                                    {
                                        objArmazenagem.PrecoUnitario = layout.Valor40;
                                    }

                                    if (layout.TipoRegistro == TipoRegistro.ARMAZENAGEM)
                                    {
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                    }

                                    if (layout.TipoRegistro == TipoRegistro.ARMAZENAGEM_CIF)
                                    {
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 1);
                                    }

                                    if (layout.AdicionalArmazenagem > 0)
                                    {
                                        objArmazenagem.Linha = 0;
                                        objArmazenagem.ServicoId = 295;
                                        objArmazenagem.PrecoUnitario = layout.AdicionalArmazenagem;
                                        objArmazenagem.PrecoMinimo = 0;
                                        objArmazenagem.ValorAnvisa = 0;
                                        objArmazenagem.ValorAcrescimo = 0;
                                        objArmazenagem.BaseCalculo = "CIF0";
                                        objArmazenagem.Exercito = 0;
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                        objArmazenagem.Periodo = layout.Periodo;
                                        objArmazenagem.QtdeDias = layout.QtdeDias;
                                        objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                        objArmazenagem.ProRata = layout.ProRata;
                                        objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                        objArmazenagem.PesoLimite = layout.PesoLimite;
                                        objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                        objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                        objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                        objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                                        objArmazenagem.Exercito = layout.Exercito;

                                    }
                                    else
                                    {
                                        //verifica pelo servico, tabela, periodo
                                        objArmazenagem.Linha = 0;
                                        objArmazenagem.ServicoId = 295;
                                        objArmazenagem.PrecoUnitario = 0;
                                        objArmazenagem.PrecoMinimo = 0;
                                        objArmazenagem.ValorAnvisa = 0;
                                        objArmazenagem.ValorAcrescimo = 0;
                                        objArmazenagem.Exercito = 0;
                                        objArmazenagem.BaseCalculo = "CIF0";
                                        objArmazenagem.Periodo = 1;
                                        objArmazenagem.QtdeDias = layout.QtdeDias;
                                        objArmazenagem.GrupoAtracacaoId = layout.GrupoAtracacaoId;


                                        if (!_tabelaDAO.ExisteServicoAdicional(objArmazenagem))
                                        {
                                            objArmazenagem.Periodo = layout.Periodo;
                                            _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                        }

                                        objArmazenagem.Periodo = layout.Periodo;
                                        objArmazenagem.QtdeDias = layout.QtdeDias;
                                        objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                        objArmazenagem.ProRata = layout.ProRata;
                                        objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                        objArmazenagem.PesoLimite = layout.PesoLimite;
                                        objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                        objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                        objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                        objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                                        objArmazenagem.Exercito = layout.Exercito;
                                    }

                                    if ((layout.AdicionalGRC > 0) || (layout.MinimoGRC > 0))
                                    {
                                        _temgrc = true;
                                        objArmazenagem.Linha = 0;
                                        objArmazenagem.ServicoId = 45;
                                        objArmazenagem.PrecoUnitario = layout.AdicionalGRC;
                                        objArmazenagem.ValorAnvisa = layout.ANVISAGRC;
                                        objArmazenagem.ValorAcrescimo = layout.AdicionalIMOGRC;
                                        objArmazenagem.PrecoMinimo = layout.MinimoGRC;
                                        objArmazenagem.QtdeDias = layout.QtdeDias;
                                        objArmazenagem.BaseCalculo = "CIF";
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                        objArmazenagem.PrecoMinimo = 0;
                                        objArmazenagem.Periodo = layout.Periodo;
                                        objArmazenagem.QtdeDias = layout.QtdeDias;
                                        objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                        objArmazenagem.ProRata = layout.ProRata;
                                        objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                        objArmazenagem.PesoLimite = layout.PesoLimite;
                                        objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                        objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                        objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                        objArmazenagem.Exercito = layout.Exercito;
                                        objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                                        _qtdedias = layout.QtdeDias;
                                    }
                                    else
                                    {
                                        if ((_temgrc == true) || (layout.Periodo == 1) || (_qtdedias != layout.QtdeDias) )
                                        {
                                            //verifica pelo servico, tabela, periodo
                                            objArmazenagem.Linha = 0;
                                            
                                            
                                            objArmazenagem.ServicoId = 45;
                                            objArmazenagem.PrecoUnitario = 0;
                                            objArmazenagem.PrecoMinimo = layout.MinimoGRC;
                                            objArmazenagem.ValorAnvisa = 0;
                                            objArmazenagem.ValorAcrescimo = 0;
                                            objArmazenagem.Exercito = layout.Exercito;
                                            objArmazenagem.BaseCalculo = "CIF";
                                            objArmazenagem.Periodo = layout.Periodo;
                                            objArmazenagem.ValorAnvisa = layout.ANVISAGRC;
                                            objArmazenagem.ValorAcrescimo = layout.AdicionalIMOGRC;
                                            objArmazenagem.QtdeDias = layout.QtdeDias;
                                            _qtdedias = layout.QtdeDias;


                                            if (!_tabelaDAO.ExisteServicoAdicional(objArmazenagem))
                                            {
                                                _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                            }
                                            objArmazenagem.Periodo = layout.Periodo;
                                            objArmazenagem.QtdeDias = layout.QtdeDias;
                                            objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                            objArmazenagem.ProRata = layout.ProRata;
                                            objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                            objArmazenagem.PesoLimite = layout.PesoLimite;
                                            objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                            objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                            objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                            objArmazenagem.Exercito = layout.Exercito;
                                            objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

                            if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                            {
                                objArmazenagem.PrecoUnitario = layout.Valor;
                                if (layout.TipoCarga == TipoCarga.CARGA_SOLTA )
                                {
                                    objArmazenagem.TipoCarga = "CRGST";
                                }
                                if (layout.TipoCarga == TipoCarga.CARGA_BBK )
                                {
                                    objArmazenagem.TipoCarga = "BBK";
                                }
                                if (layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                {
                                    objArmazenagem.TipoCarga = "VEIC";
                                }
                                foreach (var servico in servicosIPA)
                                {
                                    objArmazenagem.ServicoId = servico.Id;

                                    if (layout.TipoRegistro == TipoRegistro.ARMAZENAGEM)
                                    {
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                    }

                                    if (layout.TipoRegistro == TipoRegistro.ARMAZENAGEM_CIF)
                                    {
                                         _tabelaDAO.GravarServicoVariavel(objArmazenagem, 1);
                                    }
                                }
                            }

                            if (layout.TipoCarga == TipoCarga.MUDANCA_REGIME)
                            {
                                objArmazenagem.TipoCarga = "CPIER";
                                objArmazenagem.PrecoUnitario = layout.Valor40;
                                if (layout.Valor40 == 0)
                                {
                                    objArmazenagem.PrecoUnitario = layout.Valor20;
                                }
                                objArmazenagem.PrecoUnitario = layout.Valor40;
                                foreach (var servico in servicosIPA)
                                {
                                    objArmazenagem.ServicoId = servico.Id;

                                    if (layout.TipoRegistro == TipoRegistro.ARMAZENAGEM)
                                    {
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                    }

                                    if (layout.TipoRegistro == TipoRegistro.ARMAZENAGEM_CIF)
                                    {
                                        _tabelaDAO.GravarServicoVariavel(objArmazenagem, 1);
                                    }
                                }

                                objArmazenagem.Periodo = layout.Periodo;
                                objArmazenagem.QtdeDias = layout.QtdeDias;
                                objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                objArmazenagem.ProRata = layout.ProRata;
                                objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                objArmazenagem.PesoLimite = layout.PesoLimite;
                                objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                objArmazenagem.Exercito = layout.Exercito;
                                objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                            }

                            if (layout.AdicionalArmazenagem > 0)
                            {
                                objArmazenagem.Linha = 0;
                                objArmazenagem.ServicoId = 295;
                                objArmazenagem.PrecoUnitario = layout.AdicionalArmazenagem;
                                objArmazenagem.PrecoMinimo = 0;
                                objArmazenagem.ValorAnvisa = 0;
                                objArmazenagem.ValorAcrescimo = 0;
                                objArmazenagem.Exercito = 0;
                                objArmazenagem.BaseCalculo = "CIF0";

                                _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);

                                objArmazenagem.Periodo = layout.Periodo;
                                objArmazenagem.QtdeDias = layout.QtdeDias;
                                objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                objArmazenagem.ProRata = layout.ProRata;
                                objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                objArmazenagem.PesoLimite = layout.PesoLimite;
                                objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                objArmazenagem.Exercito = layout.Exercito;
                                objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                            }
                            else
                            {
                                //verifica pelo servico, tabela, periodo
                                objArmazenagem.Linha = 0;
                                objArmazenagem.ServicoId = 295;
                                objArmazenagem.PrecoUnitario = 0;
                                objArmazenagem.PrecoMinimo = 0;
                                objArmazenagem.ValorAnvisa = 0;
                                objArmazenagem.ValorAcrescimo = 0;
                                objArmazenagem.Exercito = 0;
                                objArmazenagem.BaseCalculo = "CIF0";
                                objArmazenagem.Periodo = 1;

                                if (!_tabelaDAO.ExisteServicoAdicional(objArmazenagem))
                                {
                                    objArmazenagem.Periodo = layout.Periodo;
                                    objArmazenagem.QtdeDias = layout.QtdeDias;
                                    _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                }

                                objArmazenagem.Periodo = layout.Periodo;
                                objArmazenagem.QtdeDias = layout.QtdeDias;
                                objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                objArmazenagem.ProRata = layout.ProRata;
                                objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                objArmazenagem.PesoLimite = layout.PesoLimite;
                                objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                objArmazenagem.Exercito = layout.Exercito;
                                objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                            }

                            if (layout.AdicionalGRC > 0)
                            {
                                _temgrc = true;
                                objArmazenagem.Linha = 0;
                                objArmazenagem.ServicoId = 45;
                                objArmazenagem.PrecoUnitario = layout.AdicionalGRC;
                                objArmazenagem.ValorAnvisa = layout.ANVISAGRC;
                                objArmazenagem.ValorAcrescimo = layout.AdicionalIMOGRC;
                                objArmazenagem.Exercito = layout.Exercito;
                                objArmazenagem.BaseCalculo = "CIF";
                                objArmazenagem.PrecoMinimo = layout.MinimoGRC;
                                objArmazenagem.QtdeDias = layout.QtdeDias;

                                _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                objArmazenagem.PrecoMinimo = 0;

                                objArmazenagem.Periodo = layout.Periodo;
                                objArmazenagem.QtdeDias = layout.QtdeDias;
                                objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                objArmazenagem.ProRata = layout.ProRata;
                                objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                objArmazenagem.PesoLimite = layout.PesoLimite;
                                objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                objArmazenagem.Exercito = layout.Exercito;
                                objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                            }
                            else
                            {
                                if ((layout.Periodo == 1) || (_temgrc == true))
                                {
                                    objArmazenagem.Linha = 0;
                                    objArmazenagem.ServicoId = 45;
                                    objArmazenagem.PrecoUnitario = layout.AdicionalGRC;
                                    objArmazenagem.ValorAnvisa = layout.ANVISAGRC;
                                    objArmazenagem.ValorAcrescimo = layout.AdicionalIMOGRC;
                                    objArmazenagem.Exercito = layout.Exercito;
                                    objArmazenagem.BaseCalculo = "CIF";
                                    objArmazenagem.PrecoMinimo = layout.MinimoGRC;
                                    objArmazenagem.QtdeDias = layout.QtdeDias;                                 
                                    _tabelaDAO.GravarServicoVariavel(objArmazenagem, 0);
                                    objArmazenagem.PrecoMinimo = 0;

                                    objArmazenagem.Periodo = layout.Periodo;
                                    objArmazenagem.QtdeDias = layout.QtdeDias;
                                    objArmazenagem.ValorExcesso = layout.ValorExcesso;
                                    objArmazenagem.ProRata = layout.ProRata;
                                    objArmazenagem.ValorAcrescimoPeso = layout.AdicionalPeso;
                                    objArmazenagem.PesoLimite = layout.PesoLimite;
                                    objArmazenagem.BaseExcesso = layout.BaseExcesso.ToName().ToUpper();
                                    objArmazenagem.BaseCalculo = layout.BaseCalculo.ToName().ToUpper();
                                    objArmazenagem.ValorAcrescimo = layout.AdicionalIMO;
                                    objArmazenagem.Exercito = layout.Exercito;
                                    objArmazenagem.ValorAnvisa = layout.ValorANVISA;
                                }
                            }
                        }

                        break;

                    case TipoRegistro.ARMAZENAGEM_MINIMO:

                        if (layout.LinhaReferencia > 0)
                        {
                            var VarianteLocal = ConverteMargemIPA.MargemIPA(layout.Margem);

                            var armazenagem = _tabelaDAO.ObterServicoPeriodoPorLinha(layout.LinhaReferencia, layout.OportunidadeId);

                            if (armazenagem != null)
                            {
                                foreach (var arm_minimo in armazenagem)
                                {

                                    if (layout.TipoCarga == TipoCarga.CONTEINER)
                                {
                                    foreach (var tipoCarga in tiposCargas)
                                    {
                                        if (tipoCarga == "SVAR20")
                                        {
                                            _tabelaDAO.AtualizarPrecoMinimo(layout.ValorMinimo20, layout.LimiteBls, arm_minimo.ServicoFixoVariavelId, tipoCarga, layout.BaseCalculo.ToName().ToUpper() ,layout.OportunidadeId, VarianteLocal);
                                        }
                                        else if (tipoCarga == "SVAR40")
                                        {
                                            _tabelaDAO.AtualizarPrecoMinimo(layout.ValorMinimo40, layout.LimiteBls, arm_minimo.ServicoFixoVariavelId, tipoCarga, layout.BaseCalculo.ToName().ToUpper(),layout.OportunidadeId, VarianteLocal);
                                        }
                                    }
                                }

                                if (layout.TipoCarga == TipoCarga.MUDANCA_REGIME)
                                {
                                    if ((layout.ValorMinimo20 != layout.ValorMinimo40) || (_temminimo == true))
                                    {
                                        _temminimo = true;
                                        foreach (var tipoCarga in tiposCargas)
                                        {
                                            var ids = _tabelaDAO.ObterServicosArmazenagemAnteriores(arm_minimo.Periodo, "CPIER", arm_minimo.BaseCalculo, tabelaCobranca, layout.Linha);

                                            if (tipoCarga == "SVAR20")
                                                _tabelaDAO.CadastrarFaixaMinimo(layout.ValorMinimo20, tipoCarga, 99, ids.ToArray());
                                            else
                                                _tabelaDAO.CadastrarFaixaMinimo(layout.ValorMinimo40, tipoCarga, 99, ids.ToArray());
                                        }
                                    }
                                    else
                                    {
                                        _tabelaDAO.AtualizarPrecoMinimo(layout.ValorMinimo20, layout.LimiteBls, arm_minimo.ServicoFixoVariavelId, "CPIER", layout.BaseCalculo.ToName().ToUpper(), layout.OportunidadeId, VarianteLocal);
                                    }
                                }

                                if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                {
                                    var ids = _tabelaDAO.ObterServicosArmazenagemAnteriores(
                                        arm_minimo.Periodo, arm_minimo.TipoCarga, arm_minimo.BaseCalculo, tabelaCobranca, layout.Linha);
                                    if ((layout.ValorMinimo == 0) & (layout.LimiteBls == 0))
                                    {
                                        layout.LimiteBls = 9999;
                                    }
                                    if (layout.LimiteBls > 0)
                                    {
                                        foreach (var tipoCarga in tiposCargas)
                                        {

                                            if (tipoCarga == "SVAR20")
                                            {
                                                if (layout.ValorMinimo20 == 0) { layout.ValorMinimo20 = layout.ValorMinimo; }
                                                _tabelaDAO.CadastrarFaixaMinimo(layout.ValorMinimo20, tipoCarga, layout.LimiteBls, ids.ToArray());
                                            }
                                            else
                                            {
                                                if (layout.ValorMinimo40 == 0) { layout.ValorMinimo40 = layout.ValorMinimo; }
                                                _tabelaDAO.CadastrarFaixaMinimo(layout.ValorMinimo40, tipoCarga, layout.LimiteBls, ids.ToArray());
                                            }
                                        }
                                    }
                                    else
                                    {
                                            if (layout.TipoCarga == TipoCarga.CARGA_SOLTA)
                                            {
                                                _tabelaDAO.AtualizarPrecoMinimo(layout.ValorMinimo, layout.LimiteBls, arm_minimo.ServicoFixoVariavelId, "CRGST", layout.BaseCalculo.ToName().ToUpper(),layout.OportunidadeId, VarianteLocal);
                                            }
                                            if (layout.TipoCarga == TipoCarga.CARGA_BBK)
                                            {
                                                _tabelaDAO.AtualizarPrecoMinimo(layout.ValorMinimo, layout.LimiteBls, arm_minimo.ServicoFixoVariavelId, "BBK", layout.BaseCalculo.ToName().ToUpper(),layout.OportunidadeId, VarianteLocal);
                                            }
                                            if (layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                            {
                                                _tabelaDAO.AtualizarPrecoMinimo(layout.ValorMinimo, layout.LimiteBls, arm_minimo.ServicoFixoVariavelId, "VEIC", layout.BaseCalculo.ToName().ToUpper(),layout.OportunidadeId, VarianteLocal);
                                            }

                                    }
                                }
                            }
                            }
                        }

                        break;

                    case TipoRegistro.SERVIÇO_PARA_MARGEM:

                        var objServicoMargem = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            PesoMaximo = layout.PesoMaximo,
                            AdicionalPeso = layout.AdicionalPeso,
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            TipoDocumentoId = layout.TipoDocumentoId,
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                            BaseExcesso = layout.BaseExcesso.ToName().ToUpper(),
                            ValorExcesso = layout.ValorExcesso,
                            ProRata = layout.ProRata
                        };

                        if (layout.AdicionalIMO > 0)
                        {
                            objServicoMargem.ValorAcrescimo = layout.AdicionalIMO;
                        }
                        if (layout.Exercito > 0)
                        {
                            objServicoMargem.Exercito = layout.Exercito;
                        }

                        foreach (var variante in variantes)
                        {
                            objServicoMargem.VarianteLocal = variante;

                            if (variante == "MDIR")
                            {
                                objServicoMargem.PrecoUnitario = layout.ValorMargemDireita;
                            }
                            else if (variante == "MESQ")
                            {
                                objServicoMargem.PrecoUnitario = layout.ValorMargemEsquerda;
                            }
                            else
                            {
                                objServicoMargem.PrecoUnitario = layout.ValorEntreMargens;
                            }

                            foreach (var servico in servicosIPA)
                            {
                                objServicoMargem.ServicoId = servico.Id;

                                // Para o serviço "Adicional 25 Ton" existe regra diferenciada para cálculo. 
                                // O valor unitário deve ser preenchido no campo "Acresc. Peso", e inserido o peso (25000) no campo "Acima de (kg)". 
                                // O campo preço unitário deve ficar em branco.

                                if (objServicoMargem.ServicoId == 332)
                                {
                                    objServicoMargem.ValorAcrescimoPeso = objServicoMargem.PrecoUnitario;
                                    objServicoMargem.PrecoUnitario = 0;
                                    objServicoMargem.PesoLimite = 25000;
                                }

                                if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                {
                                    if (layout.TipoCarga == TipoCarga.CARGA_SOLTA)
                                    {
                                        objServicoMargem.TipoCarga = "CRGST";
                                    }
                                    if (layout.TipoCarga == TipoCarga.CARGA_BBK)
                                    {
                                        objServicoMargem.TipoCarga = "BBK";
                                    }
                                    if (layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                    {
                                        objServicoMargem.TipoCarga = "VEIC";
                                    }
                                    _tabelaDAO.GravarServicoFixo(objServicoMargem);
                                }
                                else
                                {
                                    objServicoMargem.TipoCarga = "SVAR";
                                    _tabelaDAO.GravarServicoFixo(objServicoMargem);
                                }

                            }
                        }

                        break;

                    case TipoRegistro.MINIMO_PARA_MARGEM:

                        if (layout.LinhaReferencia > 0)
                        {
                            var servicosMargem = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "");

                            foreach (var servicoMargem in servicosMargem)
                            {
                                if (servicoMargem.VarianteLocal == "MDIR")
                                {
                                    _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimoMargemDireita, servicoMargem.ServicoFixoVariavelId,"");
                                }
                                else if (servicoMargem.VarianteLocal == "MESQ")
                                {
                                    _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimoMargemEsquerda, servicoMargem.ServicoFixoVariavelId,"");
                                }
                                else
                                {
                                    _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimoEntreMargens, servicoMargem.ServicoFixoVariavelId,"");
                                }
                            }
                        }

                        break;

                    case TipoRegistro.SERVICO_MECANICA_MANUAL:

                        var objServicoMecManual = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                            PesoMaximo = layout.PesoMaximo,
                            AdicionalPeso = layout.AdicionalPeso,
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            TipoOperacao = ConverteTipoOperacaoIPA.TipoOperacaoIPA(layout.TipoTrabalho),
                            TipoCarga = "SVAR20",
                            VarianteLocal = "SVAR",
                        };

                        if (layout.AdicionalIMO > 0)
                        {
                            objServicoMecManual.ValorAcrescimo = layout.AdicionalIMO;
                        }
                        if (layout.Exercito > 0)
                        {
                            objServicoMecManual.Exercito = layout.Exercito;
                        }
                        objServicoMecManual.TipoCarga = "SVAR20";
                        GravarServicoFixo(layout, servicosIPA, objServicoMecManual);
                        break;

                    case TipoRegistro.MINIMO_MECANICA_MANUAL:

                        if (layout.LinhaReferencia > 0)
                        {
                            if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                            {
                                if (layout.TipoCarga == TipoCarga.CARGA_SOLTA)
                                {
                                    var servicosMecManualn = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "CRGST");
                                    foreach (var servicoMecManual in servicosMecManualn)
                                    {
                                        _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoMecManual.ServicoFixoVariavelId,"");
                                    }
                                }
                                if (layout.TipoCarga == TipoCarga.CARGA_BBK)
                                {
                                    var servicosMecManuabbk = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "BBK");
                                    foreach (var servicoMecManual in servicosMecManuabbk)
                                    {
                                        _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoMecManual.ServicoFixoVariavelId,"");
                                    }
                                }
                                if (layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                {
                                    var servicosMecManuaveic = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "VEIC");
                                    foreach (var servicoMecManual in servicosMecManuaveic)
                                    {
                                        _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoMecManual.ServicoFixoVariavelId,"");
                                    }
                                }

                            }
                            else
                            {
                                foreach (var tipoCarga in tiposCargas)
                                {
                                    if (tipoCarga == "SVAR20")
                                    {
                                        var servicosMecManualn = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "SVAR20");
                                        foreach (var servicoMecManual in servicosMecManualn)
                                        {
                                            _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoMecManual.ServicoFixoVariavelId,"");
                                        }
                                    }
                                    else if (tipoCarga == "SVAR40")
                                    {
                                        var servicosMecManualn = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "SVAR40");
                                        foreach (var servicoMecManual in servicosMecManualn)
                                        {
                                            _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo40, servicoMecManual.ServicoFixoVariavelId,"");
                                        }

                                    }
                                }
                                /*}
                                else
                                {
                                    _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoMecManual.ServicoFixoVariavelId);
                                } 
                            } */

                            }
                        }

                        break;

                    case TipoRegistro.SERVICO_LIBERACAO:

                        var objServicoLiberacao = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            GrupoAtracacaoId = layout.GrupoAtracacaoId,
                            TipoDocumentoId = layout.TipoDocumentoId,
                            VarianteLocal = ConverteMargemIPA.MargemIPA(layout.Margem)
                        };

                        if (layout.AdicionalIMO > 0)
                        {
                            objServicoLiberacao.ValorAcrescimo = layout.AdicionalIMO;
                        }

                        if (layout.Exercito > 0)
                        {
                            objServicoLiberacao.Exercito = layout.Exercito;
                        }
                        if (layout.TipoCarga == TipoCarga.CONTEINER)
                        {
                            GravarServicoFixo(layout, servicosIPA, objServicoLiberacao);
                        }
                        else
                        {
                            objServicoLiberacao.PrecoUnitario = layout.Valor;
                            GravarServicoFixo(layout, servicosIPA, objServicoLiberacao);
                        }

                        break;

                    case TipoRegistro.SERVICO_HUBPORT:

                        var objServicoHubPort = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            Origem = layout.Origem,
                            Destino = layout.Destino,
                            VarianteLocal = "SVAR"
                        };

                        if (layout.FormaPagamentoNVOCC > 0)
                        {
                            objServicoHubPort.FormaPagamentoNVOCC = ConverteFormaPagamentoIPA.FormaPagamentoIPA(layout.FormaPagamentoNVOCC);
                        }
                        else
                        {
                            objServicoHubPort.FormaPagamentoNVOCC = 0;
                        }

                        objServicoHubPort.TipoCarga = "CRGST";

                        foreach (var servico in servicosIPA)
                        {
                            objServicoHubPort.ServicoId = servico.Id;
                            objServicoHubPort.PrecoUnitario = layout.Valor;
                            objServicoHubPort.PrecoMinimo = layout.ValorMinimo;
                            if (layout.ValorMinimo==0)
                            { 
                                objServicoHubPort.PrecoMinimo = layout.Valor;
                            }
                            _tabelaDAO.GravarServicoFixo(objServicoHubPort);
                        }

                        break;

                    case TipoRegistro.GERAIS:

                        var objServicoGeral = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            TipoDocumentoId = layout.TipoDocumentoId,
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                            BaseExcesso = layout.BaseExcesso.ToName().ToUpper(),
                            ValorExcesso = layout.ValorExcesso,
                            FormaPagamentoNVOCC = layout.FormaPagamentoNVOCC,
                            VarianteLocal = "SVAR"
                        };

                        GravarServicoFixo(layout, servicosIPA, objServicoGeral);

                        break;

                    case TipoRegistro.MINIMO_GERAL:

                        if (layout.LinhaReferencia > 0)
                        {
                            var armazenagem_min = _tabelaDAO.ObterServicoPeriodoPorLinha(layout.LinhaReferencia, layout.OportunidadeId);

                            if (armazenagem_min.Count()>0)
                            {

                                foreach (var arm_minimo in armazenagem_min)
                                {
                                    if (layout.TipoCarga == TipoCarga.CONTEINER)
                                    {
                                        foreach (var tipoCarga in tiposCargas)
                                        {
                                            if (tipoCarga == "SVAR20")
                                            {
                                                _tabelaDAO.AtualizarPrecoMinimoVariavel(layout.ValorMinimo20, arm_minimo.ServicoFixoVariavelId);
                                            }
                                            else if (tipoCarga == "SVAR40")
                                            {
                                                _tabelaDAO.AtualizarPrecoMinimoVariavel(layout.ValorMinimo40, arm_minimo.ServicoFixoVariavelId);
                                            }
                                        }
                                    }

                                    if (layout.TipoCarga == TipoCarga.MUDANCA_REGIME)
                                    {
                                        if ((layout.ValorMinimo20 != layout.ValorMinimo40))
                                        {
                                            _tabelaDAO.AtualizarPrecoMinimoVariavel(layout.ValorMinimo20, arm_minimo.ServicoFixoVariavelId);
                                        }
                                    }

                                    if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                                    {
                                        _tabelaDAO.AtualizarPrecoMinimoVariavel(layout.ValorMinimo, arm_minimo.ServicoFixoVariavelId);
                                    }

                                }
                            }
                            else
                            {
                                var servicosGerais = _tabelaDAO.ObterServicoFixoPorLinha(layout.LinhaReferencia, layout.OportunidadeId, "");

                                foreach (var servicoGeral in servicosGerais)
                                {
                                    if (layout.TipoCarga != TipoCarga.CARGA_SOLTA && layout.TipoCarga != TipoCarga.CARGA_BBK && layout.TipoCarga != TipoCarga.CARGA_VEICULO)
                                    {
                                        if (layout.ValorMinimo20 != layout.ValorMinimo40)
                                        {
                                            foreach (var tipoCarga in tiposCargas)
                                            {
                                                if (tipoCarga == "SVAR20")
                                                {
                                                    _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoGeral.ServicoFixoVariavelId, layout.BaseCalculo.ToName().ToUpper());
                                                }
                                                else if (tipoCarga == "SVAR40")
                                                {
                                                    _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo40, servicoGeral.ServicoFixoVariavelId, layout.BaseCalculo.ToName().ToUpper());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo20, servicoGeral.ServicoFixoVariavelId, layout.BaseCalculo.ToName().ToUpper());
                                        }
                                    }
                                    else
                                    {
                                        _tabelaDAO.AtualizarPrecoMinimoFixo(layout.ValorMinimo, servicoGeral.ServicoFixoVariavelId,  layout.BaseCalculo.ToName().ToUpper());
                                    }
                                }
                            }
                        }
                        break;
                    
                    case TipoRegistro.ARMAZENAGEM_MINIMO_CIF:
                        if (layout.LinhaReferencia > 0)
                        {
                        }
                        break;

                    case TipoRegistro.PERIODO_PADRAO:

                        var objPeriodoPadrao = new ServicoFixoVariavel
                        {
                            Lista = tabelaCobranca,
                            OportunidadeId = layout.OportunidadeId,
                            Linha = layout.Linha,
                            Periodo = layout.Periodo,
                            QtdeDias = layout.QtdeDias,
                            VarianteLocal = "SVAR",
                            Moeda = ConverteMoedaIPA.MoedaIPA(layout.Moeda),
                            BaseCalculo = layout.BaseCalculo.ToName().ToUpper(),
                        };

                        if (layout.TipoCarga == TipoCarga.CONTEINER)
                        {
                            if (layout.Valor20 == layout.Valor40)
                            {
                                objPeriodoPadrao.TipoCarga = "SVAR";
                                objPeriodoPadrao.Linha = layout.Linha;

                                foreach (var servico in servicosIPA)
                                {
                                    objPeriodoPadrao.ServicoId = servico.Id;
                                    objPeriodoPadrao.PrecoUnitario = layout.Valor20;

                                    _tabelaDAO.GravarServicoVariavel(objPeriodoPadrao,0);
                                }
                            }
                            else
                            {
                                foreach (var tipoCarga in tiposCargas)
                                {
                                    objPeriodoPadrao.TipoCarga = tipoCarga;
                                    objPeriodoPadrao.Linha = layout.Linha;

                                    foreach (var servico in servicosIPA)
                                    {
                                        objPeriodoPadrao.ServicoId = servico.Id;

                                        if (tipoCarga == "SVAR20")
                                        {
                                            objPeriodoPadrao.PrecoUnitario = layout.Valor20;
                                        }
                                        else
                                        {
                                            objPeriodoPadrao.PrecoUnitario = layout.Valor40;
                                        }

                                        _tabelaDAO.GravarServicoVariavel(objPeriodoPadrao,0);
                                    }
                                }
                            }
                        }


                        if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                        {
                            objPeriodoPadrao.PrecoUnitario = layout.Valor;
                            if (layout.TipoCarga == TipoCarga.CARGA_SOLTA)
                            {
                                objPeriodoPadrao.TipoCarga = "CRGST";
                            }
                            if (layout.TipoCarga == TipoCarga.CARGA_BBK)
                            {
                                objPeriodoPadrao.TipoCarga = "BBK";
                            }
                            if (layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                            {
                                objPeriodoPadrao.TipoCarga = "VEIC";
                            }

                            foreach (var servico in servicosIPA)
                            {
                                objPeriodoPadrao.ServicoId = servico.Id;
                                _tabelaDAO.GravarServicoVariavel(objPeriodoPadrao,0);
                            }
                        }

                        if (layout.TipoCarga == TipoCarga.MUDANCA_REGIME)
                        {
                            objPeriodoPadrao.PrecoUnitario = layout.Valor;
                            objPeriodoPadrao.TipoCarga = "CPIER";
                            if ((layout.Valor > 0) || (layout.Valor20 + layout.Valor40 == 0))
                            {
                                objPeriodoPadrao.PrecoUnitario = layout.Valor;
                                foreach (var servico in servicosIPA)
                                {
                                    objPeriodoPadrao.ServicoId = servico.Id;
                                    _tabelaDAO.GravarServicoVariavel(objPeriodoPadrao,0);
                                }
                            }
                            if (layout.Valor20 > 0)
                            {
                                objPeriodoPadrao.PrecoUnitario = layout.Valor20;
                                foreach (var servico in servicosIPA)
                                {
                                    objPeriodoPadrao.ServicoId = servico.Id;
                                    _tabelaDAO.GravarServicoVariavel(objPeriodoPadrao,0);
                                }
                            }
                            if (layout.Valor40 > 0)
                            {
                                objPeriodoPadrao.PrecoUnitario = layout.Valor40;
                                foreach (var servico in servicosIPA)
                                {
                                    objPeriodoPadrao.ServicoId = servico.Id;
                                    _tabelaDAO.GravarServicoVariavel(objPeriodoPadrao,0);
                                }
                            }
                        }

                        break;
                }
            }

            if (!_crm)
            {
                _tabelaDAO.AcertaReefer(tabelaCobranca);
                _tabelaDAO.CorrigeFaixasCIF(_oportunidadeId);
                _tabelaDAO.CorrigeFaixasCIFMinimo(_oportunidadeId);
                _tabelaDAO.RemoverDuplicidadesServicosVariaveis(tabelaCobranca);
                _tabelaDAO.RemoverDuplicidadesServicosFixos(tabelaCobranca);
                _tabelaDAO.ApagarReembolsosInexistentes(tabelaCobranca);
                _tabelaDAO.GravarServicoLiberacao(tabelaCobranca, _oportunidadeId);
                _tabelaDAO.ExcluirServicosEntreMargem(tabelaCobranca);
                _tabelaDAO.RemoverDuplicidadesMargem(tabelaCobranca);
                _tabelaDAO.RemoverDuplicidadesMargemfixo(tabelaCobranca, 1);
                _tabelaDAO.RemoverDuplicidadesMargem(tabelaCobranca);
                _tabelaDAO.RemoverDuplicidadesMargemfixo(tabelaCobranca, 2);
                _tabelaDAO.CorrigeGrupoAtracacao(tabelaCobranca);
                _tabelaDAO.CorrigeMonitoramentoReeferCPIER(_oportunidadeId);
            }
            else
            {
                _tabelaDAO.CorrigeFaixasCIF(_oportunidadeId);
               _tabelaDAO.CorrigeFaixasCIFMinimo(_oportunidadeId);
                _tabelaDAO.RemoverDuplicidadesServicosFixos(tabelaCobranca);

            }
        }

        private void GravarServicoFixo(LayoutDTO layout, IEnumerable<ServicoIPA> servicosIPA, ServicoFixoVariavel obj)
        {
            if (layout.TipoCarga == TipoCarga.CARGA_SOLTA || layout.TipoCarga == TipoCarga.CARGA_BBK || layout.TipoCarga == TipoCarga.CARGA_VEICULO)
            {
                if (layout.TipoCarga == TipoCarga.CARGA_SOLTA)
                {
                    obj.TipoCarga = "CRGST";
                }
                if (layout.TipoCarga == TipoCarga.CARGA_BBK)
                {
                    obj.TipoCarga = "BBK";
                }
                if (layout.TipoCarga == TipoCarga.CARGA_VEICULO)
                {
                    obj.TipoCarga = "VEIC";
                }


                foreach (var servico in servicosIPA)
                {
                    obj.ServicoId = servico.Id;                    

                    if (obj.ServicoId == 332)
                    {
                        obj.ValorAcrescimoPeso = obj.PrecoUnitario;
                        obj.PrecoUnitario = 0;
                        obj.PesoLimite = 25000;
                    }
                    else
                    {
                        obj.PrecoUnitario = layout.Valor;
                    }

                    _tabelaDAO.GravarServicoFixo(obj);
                }
            }
            else
            {
                foreach (var tipoCarga in tiposCargas)
                {
                    obj.TipoCarga = tipoCarga;

                    foreach (var servico in servicosIPA)
                    {
                        obj.ServicoId = servico.Id;

                        if (obj.ServicoId == 332)
                        {
                            obj.ValorAcrescimoPeso = obj.PrecoUnitario;
                            obj.PrecoUnitario = 0;
                            obj.PesoLimite = 25000;
                        }
                        else
                        {
                            obj.PrecoUnitario = tipoCarga == "SVAR20" ? layout.Valor20 : layout.Valor40;
                        }
                         
                        _tabelaDAO.GravarServicoFixo(obj);
                    }
                }
            }
        }
    }
}