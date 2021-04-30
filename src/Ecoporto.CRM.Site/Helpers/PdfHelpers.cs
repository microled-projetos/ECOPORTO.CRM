using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Ecoporto.CRM.Site.Helpers
{
    public class PdfHelpers : PdfPageEventHelper
    {
        private readonly string _proposta;

        public PdfHelpers(string proposta)
        {
            _proposta = proposta;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            PdfPTable tabFot = new PdfPTable(1);
            PdfPCell cell;
            tabFot.TotalWidth = 600;

            Image logoTipo = Image.GetInstance(@"http://op.ecoportosantos.com.br/CRM/content/img/header-pdf.png");
            logoTipo.ScaleAbsolute(580f, 90f);

            tabFot.AddCell(new PdfPCell(logoTipo)
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                BorderWidthLeft = 0f,
                BorderWidthRight = 0f,
                BorderWidthTop = 0f,
                BorderWidthBottom = 0f,
                Colspan = 2
            });

            var verdana = FontFactory.GetFont("Verdana", 9f, Font.BOLD);

            tabFot.AddCell(new PdfPCell(new Phrase(_proposta, verdana))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                BorderWidthLeft = 0f,
                BorderWidthRight = 0f,
                BorderWidthTop = 0f,
                BorderWidthBottom = 0f,
                PaddingTop=3,
                PaddingBottom=0,
                PaddingLeft = 36
            });

            tabFot.WriteSelectedRows(0, -1, 0, 844, writer.DirectContent);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            PdfPTable tabFot = new PdfPTable(1)
            {
                TotalWidth = 600
            };

            tabFot.DefaultCell.Border = Rectangle.NO_BORDER;

            tabFot.AddCell(new PdfPCell(new Phrase($"{writer.PageNumber}"))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BorderWidthLeft = 0f,
                BorderWidthRight = 0f,
                BorderWidthTop = 0f,
                BorderWidthBottom = 0f
            });

            tabFot.WriteSelectedRows(0, -1, -40, document.Bottom, writer.DirectContent);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}