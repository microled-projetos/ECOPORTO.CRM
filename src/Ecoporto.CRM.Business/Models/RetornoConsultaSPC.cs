using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ecoporto.CRM.Business.Models.SPC
{
	[XmlRoot(ElementName = "protocolo")]
	public class Protocolo
	{
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
		[XmlAttribute(AttributeName = "digito")]
		public string Digito { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "operador")]
	public class Operador
	{
		[XmlAttribute(AttributeName = "codigo")]
		public string Codigo { get; set; }
		[XmlAttribute(AttributeName = "nome")]
		public string Nome { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "mensagem-base-externa")]
	public class Mensagembaseexterna
	{
		[XmlAttribute(AttributeName = "origem-base-externa")]
		public string Origembaseexterna { get; set; }
		[XmlAttribute(AttributeName = "mensagem-base-externa")]
		public string _mensagembaseexterna { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "base-inoperante")]
	public class Baseinoperante
	{
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "cpf")]
	public class Cpf
	{
		[XmlAttribute(AttributeName = "regiao-origem")]
		public string Regiaoorigem { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "situacao-cpf")]
	public class Situacaocpf
	{
		[XmlAttribute(AttributeName = "descricao-situacao")]
		public string Descricaosituacao { get; set; }
		[XmlAttribute(AttributeName = "data-situacao")]
		public string Datasituacao { get; set; }
		[XmlAttribute(AttributeName = "uf")]
		public string Uf { get; set; }
	}

	[XmlRoot(ElementName = "estado-rg")]
	public class Estadorg
	{
		[XmlAttribute(AttributeName = "sigla-uf")]
		public string Siglauf { get; set; }
	}

	[XmlRoot(ElementName = "estado")]
	public class Estado
	{
		[XmlAttribute(AttributeName = "sigla-uf")]
		public string Siglauf { get; set; }
	}

	[XmlRoot(ElementName = "cidade")]
	public class Cidade
	{
		[XmlElement(ElementName = "estado")]
		public Estado Estado { get; set; }
		[XmlAttribute(AttributeName = "nome")]
		public string Nome { get; set; }
	}

	[XmlRoot(ElementName = "endereco")]
	public class Endereco
	{
		[XmlElement(ElementName = "cidade")]
		public Cidade Cidade { get; set; }
		[XmlAttribute(AttributeName = "logradouro")]
		public string Logradouro { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
		[XmlAttribute(AttributeName = "complemento")]
		public string Complemento { get; set; }
		[XmlAttribute(AttributeName = "bairro")]
		public string Bairro { get; set; }
		[XmlAttribute(AttributeName = "cep")]
		public string Cep { get; set; }
	}

	[XmlRoot(ElementName = "telefone-residencial")]
	public class Telefoneresidencial
	{
		[XmlAttribute(AttributeName = "numero-ddd")]
		public string Numeroddd { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "telefone-celular")]
	public class Telefonecelular
	{
		[XmlAttribute(AttributeName = "numero-ddd")]
		public string Numeroddd { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "telefone-comercial")]
	public class Telefonecomercial
	{
		[XmlAttribute(AttributeName = "numero-ddd")]
		public string Numeroddd { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "fax")]
	public class Fax
	{
		[XmlAttribute(AttributeName = "numero-ddd")]
		public string Numeroddd { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "nacionalidade")]
	public class Nacionalidade
	{
		[XmlAttribute(AttributeName = "nome")]
		public string Nome { get; set; }
	}

	[XmlRoot(ElementName = "consumidor-pessoa-fisica")]
	public class Consumidorpessoafisica
	{
		[XmlElement(ElementName = "cpf")]
		public Cpf Cpf { get; set; }
		[XmlElement(ElementName = "situacao-cpf")]
		public Situacaocpf Situacaocpf { get; set; }
		[XmlElement(ElementName = "estado-rg")]
		public Estadorg Estadorg { get; set; }
		[XmlElement(ElementName = "endereco")]
		public Endereco Endereco { get; set; }
		[XmlElement(ElementName = "telefone-residencial")]
		public Telefoneresidencial Telefoneresidencial { get; set; }
		[XmlElement(ElementName = "telefone-celular")]
		public Telefonecelular Telefonecelular { get; set; }
		[XmlElement(ElementName = "telefone-comercial")]
		public Telefonecomercial Telefonecomercial { get; set; }
		[XmlElement(ElementName = "fax")]
		public Fax Fax { get; set; }
		[XmlElement(ElementName = "nacionalidade")]
		public Nacionalidade Nacionalidade { get; set; }
		[XmlAttribute(AttributeName = "data-expedicao-rg")]
		public string Dataexpedicaorg { get; set; }
		[XmlAttribute(AttributeName = "data-nascimento")]
		public string Datanascimento { get; set; }
		[XmlAttribute(AttributeName = "email")]
		public string Email { get; set; }
		[XmlAttribute(AttributeName = "estado-civil")]
		public string Estadocivil { get; set; }
		[XmlAttribute(AttributeName = "idade")]
		public string Idade { get; set; }
		[XmlAttribute(AttributeName = "nome")]
		public string Nome { get; set; }
		[XmlAttribute(AttributeName = "nome-mae")]
		public string Nomemae { get; set; }
		[XmlAttribute(AttributeName = "nome-pai")]
		public string Nomepai { get; set; }
		[XmlAttribute(AttributeName = "nome-social")]
		public string Nomesocial { get; set; }
		[XmlAttribute(AttributeName = "numero-rg")]
		public string Numerorg { get; set; }
		[XmlAttribute(AttributeName = "numero-titulo-eleitor")]
		public string Numerotituloeleitor { get; set; }
		[XmlAttribute(AttributeName = "pessoa-estrangeira")]
		public string Pessoaestrangeira { get; set; }
		[XmlAttribute(AttributeName = "sexo")]
		public string Sexo { get; set; }
		[XmlAttribute(AttributeName = "signo")]
		public string Signo { get; set; }
	}

	[XmlRoot(ElementName = "cnpj")]
	public class Cnpj
	{
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "situacao-cnpj")]
	public class Situacaocnpj
	{
		[XmlAttribute(AttributeName = "descricao-situacao")]
		public string Descricaosituacao { get; set; }
		[XmlAttribute(AttributeName = "data-situacao")]
		public string Datasituacao { get; set; }
		[XmlAttribute(AttributeName = "uf")]
		public string Uf { get; set; }
	}

	[XmlRoot(ElementName = "situacao-inscricao-estadual")]
	public class Situacaoinscricaoestadual
	{
		[XmlAttribute(AttributeName = "descricao-situacao")]
		public string Descricaosituacao { get; set; }
		[XmlAttribute(AttributeName = "data-situacao")]
		public string Datasituacao { get; set; }
		[XmlAttribute(AttributeName = "uf")]
		public string Uf { get; set; }
	}

	[XmlRoot(ElementName = "telefone")]
	public class Telefone
	{
		[XmlAttribute(AttributeName = "numero-ddd")]
		public string Numeroddd { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "natureza-juridica")]
	public class Naturezajuridica
	{
		[XmlAttribute(AttributeName = "descricao")]
		public string Descricao { get; set; }
		[XmlAttribute(AttributeName = "codigo")]
		public string Codigo { get; set; }
	}

	[XmlRoot(ElementName = "atividade-economica-principal")]
	public class Atividadeeconomicaprincipal
	{
		[XmlAttribute(AttributeName = "descricao")]
		public string Descricao { get; set; }
		[XmlAttribute(AttributeName = "codigo")]
		public string Codigo { get; set; }
	}

	[XmlRoot(ElementName = "atividade-economica-secundaria")]
	public class Atividadeeconomicasecundaria
	{
		[XmlAttribute(AttributeName = "descricao")]
		public string Descricao { get; set; }
		[XmlAttribute(AttributeName = "codigo")]
		public string Codigo { get; set; }
	}

	[XmlRoot(ElementName = "consumidor-pessoa-juridica")]
	public class Consumidorpessoajuridica
	{
		[XmlElement(ElementName = "cnpj")]
		public Cnpj Cnpj { get; set; }
		[XmlElement(ElementName = "situacao-cnpj")]
		public Situacaocnpj Situacaocnpj { get; set; }
		[XmlElement(ElementName = "situacao-inscricao-estadual")]
		public Situacaoinscricaoestadual Situacaoinscricaoestadual { get; set; }
		[XmlElement(ElementName = "endereco")]
		public Endereco Endereco { get; set; }
		[XmlElement(ElementName = "telefone")]
		public Telefone Telefone { get; set; }
		[XmlElement(ElementName = "fax")]
		public Fax Fax { get; set; }
		[XmlElement(ElementName = "natureza-juridica")]
		public Naturezajuridica Naturezajuridica { get; set; }
		[XmlElement(ElementName = "atividade-economica-principal")]
		public Atividadeeconomicaprincipal Atividadeeconomicaprincipal { get; set; }
		[XmlElement(ElementName = "atividade-economica-secundaria")]
		public List<Atividadeeconomicasecundaria> Atividadeeconomicasecundaria { get; set; }
		[XmlAttribute(AttributeName = "data-fundacao")]
		public string Datafundacao { get; set; }
		[XmlAttribute(AttributeName = "email")]
		public string Email { get; set; }
		[XmlAttribute(AttributeName = "home-page")]
		public string Homepage { get; set; }
		[XmlAttribute(AttributeName = "inscricao-estadual")]
		public string Inscricaoestadual { get; set; }
		[XmlAttribute(AttributeName = "nome-comercial")]
		public string Nomecomercial { get; set; }
		[XmlAttribute(AttributeName = "numero-NIRE-NIRC")]
		public string NumeroNIRENIRC { get; set; }
		[XmlAttribute(AttributeName = "razao-social")]
		public string Razaosocial { get; set; }
		[XmlAttribute(AttributeName = "razao-social-anterior")]
		public string Razaosocialanterior { get; set; }
		[XmlAttribute(AttributeName = "valor-capital-social")]
		public string Valorcapitalsocial { get; set; }
	}

	[XmlRoot(ElementName = "consumidor")]
	public class Consumidor
	{
		[XmlElement(ElementName = "consumidor-pessoa-fisica")]
		public Consumidorpessoafisica Consumidorpessoafisica { get; set; }
		[XmlElement(ElementName = "consumidor-pessoa-juridica")]
		public Consumidorpessoajuridica Consumidorpessoajuridica { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "resumo")]
	public class Resumo
	{
		[XmlAttribute(AttributeName = "quantidade-total")]
		public string Quantidadetotal { get; set; }
		[XmlAttribute(AttributeName = "mesano-primeira-ocorrencia")]
		public string Mesanoprimeiraocorrencia { get; set; }
		[XmlAttribute(AttributeName = "mesano-ultima-ocorrencia")]
		public string Mesanoultimaocorrencia { get; set; }
		[XmlAttribute(AttributeName = "data-primeira-ocorrencia")]
		public string Dataprimeiraocorrencia { get; set; }
		[XmlAttribute(AttributeName = "data-ultima-ocorrencia")]
		public string Dataultimaocorrencia { get; set; }
		[XmlAttribute(AttributeName = "valor-total")]
		public decimal Valortotal { get; set; }
		[XmlAttribute(AttributeName = "valor-ultima-ocorrencia")]
		public string Valorultimaocorrencia { get; set; }
	}

	[XmlRoot(ElementName = "detalhe-grafia-pj")]
	public class Detalhegrafiapj
	{
		[XmlAttribute(AttributeName = "razao-social")]
		public string Razaosocial { get; set; }
	}

	[XmlRoot(ElementName = "cidade-associado")]
	public class Cidadeassociado
	{
		[XmlElement(ElementName = "estado")]
		public Estado Estado { get; set; }
		[XmlAttribute(AttributeName = "nome")]
		public string Nome { get; set; }
	}

	[XmlRoot(ElementName = "telefone-associado")]
	public class Telefoneassociado
	{
		[XmlAttribute(AttributeName = "numero-ddd")]
		public string Numeroddd { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
	}

	[XmlRoot(ElementName = "detalhe-spc")]
	public class Detalhespc
	{
		[XmlElement(ElementName = "cidade-associado")]
		public Cidadeassociado Cidadeassociado { get; set; }
		[XmlElement(ElementName = "telefone-associado")]
		public Telefoneassociado Telefoneassociado { get; set; }
		[XmlAttribute(AttributeName = "nome-associado")]
		public string Nomeassociado { get; set; }
		[XmlAttribute(AttributeName = "codigo-entidade")]
		public string Codigoentidade { get; set; }
		[XmlAttribute(AttributeName = "conta-contrato")]
		public string Contacontrato { get; set; }
		[XmlAttribute(AttributeName = "data-inclusao")]
		public string Datainclusao { get; set; }
		[XmlAttribute(AttributeName = "data-vencimento")]
		public string Datavencimento { get; set; }
		[XmlAttribute(AttributeName = "reservado")]
		public string Reservado { get; set; }
		[XmlAttribute(AttributeName = "nome-entidade")]
		public string Nomeentidade { get; set; }
		[XmlAttribute(AttributeName = "contrato")]
		public string Contrato { get; set; }
		[XmlAttribute(AttributeName = "registro-instituicao-financeira")]
		public string Registroinstituicaofinanceira { get; set; }
		[XmlAttribute(AttributeName = "registro-relevante")]
		public string Registrorelevante { get; set; }
		[XmlAttribute(AttributeName = "comprador-fiador-avalista")]
		public string Compradorfiadoravalista { get; set; }
		[XmlAttribute(AttributeName = "valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName = "alinea")]
	public class Alinea
	{
		[XmlAttribute(AttributeName = "codigo")]
		public string Codigo { get; set; }
		[XmlAttribute(AttributeName = "descricao")]
		public string Descricao { get; set; }
	}

	[XmlRoot(ElementName = "banco")]
	public class Banco
	{
		[XmlAttribute(AttributeName = "codigo")]
		public string Codigo { get; set; }
		[XmlAttribute(AttributeName = "nome")]
		public string Nome { get; set; }
	}

	[XmlRoot(ElementName = "dados-bancarios")]
	public class Dadosbancarios
	{
		[XmlElement(ElementName = "endereco")]
		public Endereco Endereco { get; set; }
		[XmlElement(ElementName = "banco")]
		public Banco Banco { get; set; }
		[XmlElement(ElementName = "telefone")]
		public Telefone Telefone { get; set; }
		[XmlElement(ElementName = "fax")]
		public Fax Fax { get; set; }
		[XmlAttribute(AttributeName = "numero-agencia")]
		public string Numeroagencia { get; set; }
		[XmlAttribute(AttributeName = "nome-agencia")]
		public string Nomeagencia { get; set; }
		[XmlAttribute(AttributeName = "numero-conta-corrente")]
		public string Numerocontacorrente { get; set; }
		[XmlAttribute(AttributeName = "digito-conta-corrente")]
		public string Digitocontacorrente { get; set; }
	}

	[XmlRoot(ElementName = "cheque-inicial")]
	public class Chequeinicial
	{
		[XmlElement(ElementName = "dados-bancarios")]
		public Dadosbancarios Dadosbancarios { get; set; }
		[XmlAttribute(AttributeName = "data-emissao")]
		public string Dataemissao { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
		[XmlAttribute(AttributeName = "digito")]
		public string Digito { get; set; }
		[XmlAttribute(AttributeName = "valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName = "cheque-final")]
	public class Chequefinal
	{
		[XmlElement(ElementName = "dados-bancarios")]
		public Dadosbancarios Dadosbancarios { get; set; }
		[XmlAttribute(AttributeName = "numero")]
		public string Numero { get; set; }
		[XmlAttribute(AttributeName = "digito")]
		public string Digito { get; set; }
		[XmlAttribute(AttributeName = "valor")]
		public string Valor { get; set; }
	}

	[XmlRoot(ElementName = "detalhe-cheque-lojista")]
	public class Detalhechequelojista
	{
		[XmlElement(ElementName = "alinea")]
		public Alinea Alinea { get; set; }
		[XmlElement(ElementName = "cheque-inicial")]
		public Chequeinicial Chequeinicial { get; set; }
		[XmlElement(ElementName = "cheque-final")]
		public Chequefinal Chequefinal { get; set; }
		[XmlElement(ElementName = "cidade-associado")]
		public Cidadeassociado Cidadeassociado { get; set; }
		[XmlElement(ElementName = "telefone-associado")]
		public Telefoneassociado Telefoneassociado { get; set; }
		[XmlAttribute(AttributeName = "nome-associado")]
		public string Nomeassociado { get; set; }
		[XmlAttribute(AttributeName = "codigo-entidade")]
		public string Codigoentidade { get; set; }
		[XmlAttribute(AttributeName = "data-inclusao")]
		public string Datainclusao { get; set; }
		[XmlAttribute(AttributeName = "nome-entidade")]
		public string Nomeentidade { get; set; }
		[XmlAttribute(AttributeName = "informante")]
		public string Informante { get; set; }
		[XmlAttribute(AttributeName = "origem")]
		public string Origem { get; set; }
	}

	[XmlRoot(ElementName = "grafia-pj")]
	public class Grafiapj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "endereco-cep-consultado")]
	public class Enderecocepconsultado
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "telefone-consultado")]
	public class Telefoneconsultado
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "telefone-vinculado-consumidor")]
	public class Telefonevinculadoconsumidor
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "telefone-vinculado-assinante-consultado")]
	public class Telefonevinculadoassinanteconsultado
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "ultimo-telefone-informado")]
	public class Ultimotelefoneinformado
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "ultimo-endereco-informado")]
	public class Ultimoenderecoinformado
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "participacao-empresa")]
	public class Participacaoempresa
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "socio")]
	public class Socio
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "administrador")]
	public class Administrador
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "alerta-documento")]
	public class Alertadocumento
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "spc")]
	public class Spc
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public List<Detalhespc> Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "cheque-lojista")]
	public class Chequelojista
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "ccf")]
	public class Ccf
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contra-ordem-documento-diferente")]
	public class Contraordemdocumentodiferente
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "quantidade-taloes-contra-ordenados")]
		public string Quantidadetaloescontraordenados { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contra-ordem-agencia-diferente")]
	public class Contraordemagenciadiferente
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "quantidade-taloes-contra-ordenados")]
		public string Quantidadetaloescontraordenados { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contra-ordem-agencia-conta-diferente")]
	public class Contraordemagenciacontadiferente
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "quantidade-taloes-contra-ordenados")]
		public string Quantidadetaloescontraordenados { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "acao")]
	public class Acao
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "protesto")]
	public class Protesto
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contra-ordem")]
	public class Contraordem
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "quantidade-taloes-contra-ordenados")]
		public string Quantidadetaloescontraordenados { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contumacia")]
	public class Contumacia
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "data-final-periodo-consultado")]
		public string Datafinalperiodoconsultado { get; set; }
		[XmlAttribute(AttributeName = "data-inicial-periodo-consultado")]
		public string Datainicialperiodoconsultado { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "credito-concedido")]
	public class Creditoconcedido
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "banco-agencia-conta-documento-diferente")]
	public class Bancoagenciacontadocumentodiferente
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "dados-agencia-bancaria")]
	public class Dadosagenciabancaria
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "consulta-realizada")]
	public class Consultarealizada
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "quantidade-dias-consultados")]
		public string Quantidadediasconsultados { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "informacao-poder-judiciario")]
	public class Informacaopoderjudiciario
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "riskscoring-6-meses")]
	public class Riskscoring6meses
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "riskscoring-12-meses")]
	public class Riskscoring12meses
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "cheque-sem-fundo-achei")]
	public class Chequesemfundoachei
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "cheque-sem-fundo-achei-ccf")]
	public class Chequesemfundoacheiccf
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contumacia-srs")]
	public class Contumaciasrs
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "participacao-falencia")]
	public class Participacaofalencia
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "restricao-financeira")]
	public class Restricaofinanceira
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "pendencia-financeira")]
	public class Pendenciafinanceira
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "cheque-sem-fundo-varejo")]
	public class Chequesemfundovarejo
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contra-ordem-srs")]
	public class Contraordemsrs
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "contra-ordem-documento-diferente-srs")]
	public class Contraordemdocumentodiferentesrs
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "cheque-outras-ocorrencias-srs")]
	public class Chequeoutrasocorrenciassrs
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "cheque-consulta-online-srs")]
	public class Chequeconsultaonlinesrs
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "consulta-realizada-cheque")]
	public class Consultarealizadacheque
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "referenciais-negocios")]
	public class Referenciaisnegocios
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "historico-pagamento")]
	public class Historicopagamento
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "relacionamento-mais-antigo-com-fornecedores")]
	public class Relacionamentomaisantigocomfornecedores
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "registro-consulta")]
	public class Registroconsulta
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "ultimas-consultas")]
	public class Ultimasconsultas
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "capital-social")]
	public class Capitalsocial
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "antecessora")]
	public class Antecessora
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "atividade-empresa")]
	public class Atividadeempresa
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "filial")]
	public class Filial
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "informacoes-adicionais")]
	public class Informacoesadicionais
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "incorporacao-fusao-cisao")]
	public class Incorporacaofusaocisao
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "principais-produtos")]
	public class Principaisprodutos
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "mensagem-complementar")]
	public class Mensagemcomplementar
	{
		[XmlElement(ElementName = "mensagem")]
		public List<string> Mensagem { get; set; }
		[XmlAttribute(AttributeName = "origem")]
		public string Origem { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "spc-score-3-meses")]
	public class Spcscore3meses
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "spc-score-12-meses")]
	public class Spcscore12meses
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "spc-obito")]
	public class Spcobito
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "confirmacao-rg")]
	public class Confirmacaorg
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "renavam-federal")]
	public class Renavamfederal
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "roubo-furto")]
	public class Roubofurto
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "dpvat")]
	public class Dpvat
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "gravame")]
	public class Gravame
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "renavam-estadual")]
	public class Renavamestadual
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "localizaVeiculo")]
	public class LocalizaVeiculo
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "renda-presumida-spc")]
	public class Rendapresumidaspc
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "limite-credito-sugerido")]
	public class Limitecreditosugerido
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "ocupacao")]
	public class Ocupacao
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "informacoes-complementares")]
	public class Informacoescomplementares
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "telefone-alternativo")]
	public class Telefonealternativo
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "alerta-obito")]
	public class Alertaobito
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "alerta-identidade")]
	public class Alertaidentidade
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "faturamento-presumido")]
	public class Faturamentopresumido
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "limite-credito-pj")]
	public class Limitecreditopj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "gasto-estimado-pj")]
	public class Gastoestimadopj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "quadro-social-mais-completo-pj")]
	public class Quadrosocialmaiscompletopj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "risco-credito-pj")]
	public class Riscocreditopj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "perfil-financeiro-pj")]
	public class Perfilfinanceiropj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "gasto-estimado-pf")]
	public class Gastoestimadopf
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "indice-relacionamento-mercado-pf")]
	public class Indicerelacionamentomercadopf
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "indice-relacionamento-mercado-pj")]
	public class Indicerelacionamentomercadopj
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "limite-credito-sugerido-sar")]
	public class Limitecreditosugeridosar
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "comprometimento-renda-mensal-pf")]
	public class Comprometimentorendamensalpf
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "collection-score-plus")]
	public class Collectionscoreplus
	{
		[XmlElement(ElementName = "resumo")]
		public Resumo Resumo { get; set; }
		[XmlElement(ElementName = "detalhe-grafia-pj")]
		public Detalhegrafiapj Detalhegrafiapj { get; set; }
		[XmlElement(ElementName = "detalhe-spc")]
		public Detalhespc Detalhespc { get; set; }
		[XmlElement(ElementName = "detalhe-cheque-lojista")]
		public Detalhechequelojista Detalhechequelojista { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "resultado", Namespace = "http://webservice.consulta.spcjava.spcbrasil.org/")]
	public class ResultadoSPC
	{
		[XmlElement(ElementName = "protocolo")]
		public Protocolo Protocolo { get; set; }
		[XmlElement(ElementName = "operador")]
		public Operador Operador { get; set; }
		[XmlElement(ElementName = "mensagem-base-externa")]
		public List<Mensagembaseexterna> Mensagembaseexterna { get; set; }
		[XmlElement(ElementName = "base-inoperante")]
		public List<Baseinoperante> Baseinoperante { get; set; }
		[XmlElement(ElementName = "consumidor")]
		public Consumidor Consumidor { get; set; }
		[XmlElement(ElementName = "grafia-pj")]
		public Grafiapj Grafiapj { get; set; }
		[XmlElement(ElementName = "endereco-cep-consultado")]
		public Enderecocepconsultado Enderecocepconsultado { get; set; }
		[XmlElement(ElementName = "telefone-consultado")]
		public Telefoneconsultado Telefoneconsultado { get; set; }
		[XmlElement(ElementName = "telefone-vinculado-consumidor")]
		public Telefonevinculadoconsumidor Telefonevinculadoconsumidor { get; set; }
		[XmlElement(ElementName = "telefone-vinculado-assinante-consultado")]
		public Telefonevinculadoassinanteconsultado Telefonevinculadoassinanteconsultado { get; set; }
		[XmlElement(ElementName = "ultimo-telefone-informado")]
		public Ultimotelefoneinformado Ultimotelefoneinformado { get; set; }
		[XmlElement(ElementName = "ultimo-endereco-informado")]
		public Ultimoenderecoinformado Ultimoenderecoinformado { get; set; }
		[XmlElement(ElementName = "participacao-empresa")]
		public Participacaoempresa Participacaoempresa { get; set; }
		[XmlElement(ElementName = "socio")]
		public Socio Socio { get; set; }
		[XmlElement(ElementName = "administrador")]
		public Administrador Administrador { get; set; }
		[XmlElement(ElementName = "alerta-documento")]
		public Alertadocumento Alertadocumento { get; set; }
		[XmlElement(ElementName = "spc")]
		public Spc Spc { get; set; }
		[XmlElement(ElementName = "cheque-lojista")]
		public Chequelojista Chequelojista { get; set; }
		[XmlElement(ElementName = "ccf")]
		public Ccf Ccf { get; set; }
		[XmlElement(ElementName = "contra-ordem-documento-diferente")]
		public Contraordemdocumentodiferente Contraordemdocumentodiferente { get; set; }
		[XmlElement(ElementName = "contra-ordem-agencia-diferente")]
		public Contraordemagenciadiferente Contraordemagenciadiferente { get; set; }
		[XmlElement(ElementName = "contra-ordem-agencia-conta-diferente")]
		public Contraordemagenciacontadiferente Contraordemagenciacontadiferente { get; set; }
		[XmlElement(ElementName = "acao")]
		public Acao Acao { get; set; }
		[XmlElement(ElementName = "protesto")]
		public Protesto Protesto { get; set; }
		[XmlElement(ElementName = "contra-ordem")]
		public Contraordem Contraordem { get; set; }
		[XmlElement(ElementName = "contumacia")]
		public Contumacia Contumacia { get; set; }
		[XmlElement(ElementName = "credito-concedido")]
		public Creditoconcedido Creditoconcedido { get; set; }
		[XmlElement(ElementName = "banco-agencia-conta-documento-diferente")]
		public Bancoagenciacontadocumentodiferente Bancoagenciacontadocumentodiferente { get; set; }
		[XmlElement(ElementName = "dados-agencia-bancaria")]
		public Dadosagenciabancaria Dadosagenciabancaria { get; set; }
		[XmlElement(ElementName = "consulta-realizada")]
		public Consultarealizada Consultarealizada { get; set; }
		[XmlElement(ElementName = "informacao-poder-judiciario")]
		public Informacaopoderjudiciario Informacaopoderjudiciario { get; set; }
		[XmlElement(ElementName = "riskscoring-6-meses")]
		public Riskscoring6meses Riskscoring6meses { get; set; }
		[XmlElement(ElementName = "riskscoring-12-meses")]
		public Riskscoring12meses Riskscoring12meses { get; set; }
		[XmlElement(ElementName = "cheque-sem-fundo-achei")]
		public Chequesemfundoachei Chequesemfundoachei { get; set; }
		[XmlElement(ElementName = "cheque-sem-fundo-achei-ccf")]
		public Chequesemfundoacheiccf Chequesemfundoacheiccf { get; set; }
		[XmlElement(ElementName = "contumacia-srs")]
		public Contumaciasrs Contumaciasrs { get; set; }
		[XmlElement(ElementName = "participacao-falencia")]
		public Participacaofalencia Participacaofalencia { get; set; }
		[XmlElement(ElementName = "restricao-financeira")]
		public Restricaofinanceira Restricaofinanceira { get; set; }
		[XmlElement(ElementName = "pendencia-financeira")]
		public Pendenciafinanceira Pendenciafinanceira { get; set; }
		[XmlElement(ElementName = "cheque-sem-fundo-varejo")]
		public Chequesemfundovarejo Chequesemfundovarejo { get; set; }
		[XmlElement(ElementName = "contra-ordem-srs")]
		public Contraordemsrs Contraordemsrs { get; set; }
		[XmlElement(ElementName = "contra-ordem-documento-diferente-srs")]
		public Contraordemdocumentodiferentesrs Contraordemdocumentodiferentesrs { get; set; }
		[XmlElement(ElementName = "cheque-outras-ocorrencias-srs")]
		public Chequeoutrasocorrenciassrs Chequeoutrasocorrenciassrs { get; set; }
		[XmlElement(ElementName = "cheque-consulta-online-srs")]
		public Chequeconsultaonlinesrs Chequeconsultaonlinesrs { get; set; }
		[XmlElement(ElementName = "consulta-realizada-cheque")]
		public Consultarealizadacheque Consultarealizadacheque { get; set; }
		[XmlElement(ElementName = "referenciais-negocios")]
		public Referenciaisnegocios Referenciaisnegocios { get; set; }
		[XmlElement(ElementName = "historico-pagamento")]
		public Historicopagamento Historicopagamento { get; set; }
		[XmlElement(ElementName = "relacionamento-mais-antigo-com-fornecedores")]
		public Relacionamentomaisantigocomfornecedores Relacionamentomaisantigocomfornecedores { get; set; }
		[XmlElement(ElementName = "registro-consulta")]
		public Registroconsulta Registroconsulta { get; set; }
		[XmlElement(ElementName = "ultimas-consultas")]
		public Ultimasconsultas Ultimasconsultas { get; set; }
		[XmlElement(ElementName = "capital-social")]
		public Capitalsocial Capitalsocial { get; set; }
		[XmlElement(ElementName = "antecessora")]
		public Antecessora Antecessora { get; set; }
		[XmlElement(ElementName = "atividade-empresa")]
		public Atividadeempresa Atividadeempresa { get; set; }
		[XmlElement(ElementName = "filial")]
		public Filial Filial { get; set; }
		[XmlElement(ElementName = "informacoes-adicionais")]
		public Informacoesadicionais Informacoesadicionais { get; set; }
		[XmlElement(ElementName = "incorporacao-fusao-cisao")]
		public Incorporacaofusaocisao Incorporacaofusaocisao { get; set; }
		[XmlElement(ElementName = "principais-produtos")]
		public Principaisprodutos Principaisprodutos { get; set; }
		[XmlElement(ElementName = "mensagem-complementar")]
		public List<Mensagemcomplementar> Mensagemcomplementar { get; set; }
		[XmlElement(ElementName = "spc-score-3-meses")]
		public Spcscore3meses Spcscore3meses { get; set; }
		[XmlElement(ElementName = "spc-score-12-meses")]
		public Spcscore12meses Spcscore12meses { get; set; }
		[XmlElement(ElementName = "spc-obito")]
		public Spcobito Spcobito { get; set; }
		[XmlElement(ElementName = "confirmacao-rg")]
		public Confirmacaorg Confirmacaorg { get; set; }
		[XmlElement(ElementName = "renavam-federal")]
		public Renavamfederal Renavamfederal { get; set; }
		[XmlElement(ElementName = "roubo-furto")]
		public Roubofurto Roubofurto { get; set; }
		[XmlElement(ElementName = "dpvat")]
		public Dpvat Dpvat { get; set; }
		[XmlElement(ElementName = "gravame")]
		public Gravame Gravame { get; set; }
		[XmlElement(ElementName = "renavam-estadual")]
		public Renavamestadual Renavamestadual { get; set; }
		[XmlElement(ElementName = "localizaVeiculo")]
		public LocalizaVeiculo LocalizaVeiculo { get; set; }
		[XmlElement(ElementName = "renda-presumida-spc")]
		public Rendapresumidaspc Rendapresumidaspc { get; set; }
		[XmlElement(ElementName = "limite-credito-sugerido")]
		public Limitecreditosugerido Limitecreditosugerido { get; set; }
		[XmlElement(ElementName = "ocupacao")]
		public Ocupacao Ocupacao { get; set; }
		[XmlElement(ElementName = "informacoes-complementares")]
		public Informacoescomplementares Informacoescomplementares { get; set; }
		[XmlElement(ElementName = "telefone-alternativo")]
		public Telefonealternativo Telefonealternativo { get; set; }
		[XmlElement(ElementName = "alerta-obito")]
		public Alertaobito Alertaobito { get; set; }
		[XmlElement(ElementName = "alerta-identidade")]
		public Alertaidentidade Alertaidentidade { get; set; }
		[XmlElement(ElementName = "faturamento-presumido")]
		public Faturamentopresumido Faturamentopresumido { get; set; }
		[XmlElement(ElementName = "limite-credito-pj")]
		public Limitecreditopj Limitecreditopj { get; set; }
		[XmlElement(ElementName = "gasto-estimado-pj")]
		public Gastoestimadopj Gastoestimadopj { get; set; }
		[XmlElement(ElementName = "quadro-social-mais-completo-pj")]
		public Quadrosocialmaiscompletopj Quadrosocialmaiscompletopj { get; set; }
		[XmlElement(ElementName = "risco-credito-pj")]
		public Riscocreditopj Riscocreditopj { get; set; }
		[XmlElement(ElementName = "perfil-financeiro-pj")]
		public Perfilfinanceiropj Perfilfinanceiropj { get; set; }
		[XmlElement(ElementName = "gasto-estimado-pf")]
		public Gastoestimadopf Gastoestimadopf { get; set; }
		[XmlElement(ElementName = "indice-relacionamento-mercado-pf")]
		public Indicerelacionamentomercadopf Indicerelacionamentomercadopf { get; set; }
		[XmlElement(ElementName = "indice-relacionamento-mercado-pj")]
		public Indicerelacionamentomercadopj Indicerelacionamentomercadopj { get; set; }
		[XmlElement(ElementName = "limite-credito-sugerido-sar")]
		public Limitecreditosugeridosar Limitecreditosugeridosar { get; set; }
		[XmlElement(ElementName = "comprometimento-renda-mensal-pf")]
		public Comprometimentorendamensalpf Comprometimentorendamensalpf { get; set; }
		[XmlElement(ElementName = "collection-score-plus")]
		public Collectionscoreplus Collectionscoreplus { get; set; }
		[XmlAttribute(AttributeName = "restricao")]
		public string Restricao { get; set; }
		[XmlAttribute(AttributeName = "data")]
		public string Data { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlIgnore]
		public string XML { get; set; }
	}
}
