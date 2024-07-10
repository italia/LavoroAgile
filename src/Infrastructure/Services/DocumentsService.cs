using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Domain.Model;
using Domain.Model.Utilities;
using Domain.Settings;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Infrastructure.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<Accordo, Guid> _repositoryAccordo;
        private readonly IStrutturaService _strutturaService;
        private readonly DocumentsServiceSettings _documentsServiceSettings;
        private readonly IPersonalDataProviderService _personalDataProviderService;
        private readonly IAccordoService _accordoService;
        private readonly ILogger<DocumentsService> _logger;

        public DocumentsService(IRazorViewEngine razorViewEngine, 
            ITempDataProvider tempDataProvider, 
            IServiceProvider serviceProvider, 
            IRepository<Accordo, Guid> repositoryAccordo,
            IStrutturaService strutturaService,
            IOptions<DocumentsServiceSettings> documentsServiceSettings, 
            IPersonalDataProviderService personalDataProviderService,
            IAccordoService accordoService,
            ILogger<DocumentsService> logger)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _repositoryAccordo = repositoryAccordo;
            _strutturaService = strutturaService;
            _documentsServiceSettings = documentsServiceSettings?.Value;
            _personalDataProviderService = personalDataProviderService;
            _accordoService = accordoService;
            _logger = logger;
        }

        /// <summary>
        /// //Generazione di una stringa a partire dal ViewModel valorizzato con il suo corrispondendete Model
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Generazione di un pdf a partire da un id accordo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileStreamResult> GeneratePdfAsync(Guid id, CancellationToken cancellationToken)
        {
            AccordoForPdf accordoForPdf = new AccordoForPdf();
            accordoForPdf.Accordo = await _repositoryAccordo.GetAsync(id, cancellationToken);
            accordoForPdf.Struttura = await _strutturaService.GetStrutturaAsync(Guid.Parse(accordoForPdf.Accordo.UidStrutturaUfficioServizio), cancellationToken);

            //Generazione della stringa a partire dal viewmodel e suo corrispondente model
            string html = await this.RenderToStringAsync("AccordoPdf/AccordoPdf", accordoForPdf);

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(html);
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Evento per l'aggiunta di Header e Footer
            string testoHeader = string.Empty;
            if (accordoForPdf.Accordo.Stato != StatoAccordo.Sottoscritto)
                testoHeader = EnumExtensions.ToDescriptionString(accordoForPdf.Accordo.Stato);

            writer.PageEvent = new HeaderFooterAdd(testoHeader);

            HtmlWorker htmlWorker = new HtmlWorker(doc);
            doc.Open();
            htmlWorker.StartDocument();

            //Aggiunta immagine
            try
            {
                Image png;
                png = Image.GetInstance(new Uri(_documentsServiceSettings.BaseUrl + "/img/logopresidenza.png"));
                png.ScalePercent(36f);
                png.Alignment = Image.TEXTWRAP;
                doc.Add(png);
            }catch(Exception ex)
            {
                //Va avanti la generazione del pdf anche senza l'immagine del logo in intestazione
            }

            htmlWorker.Parse(txtReader);
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            //Restituzione del filestreamresult
            var contentType = "application/pdf";
            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, contentType);            
        }

        public async Task<FileStreamResult> GeneratePdfValutazioneAsync(Guid id, CancellationToken cancellationToken)
        {
            AccordoForPdf accordoForPdf = new AccordoForPdf();
            accordoForPdf.Accordo = await _repositoryAccordo.GetAsync(id, cancellationToken);
            accordoForPdf.Struttura = await _strutturaService.GetStrutturaAsync(Guid.Parse(accordoForPdf.Accordo.UidStrutturaUfficioServizio), cancellationToken);
            
            //Generazione della stringa a partire dal viewmodel e suo corrispondente model
            string html = await this.RenderToStringAsync("ValutazionePdf/ValutazionePdf", accordoForPdf);

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(html);
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            HtmlWorker htmlWorker = new HtmlWorker(doc);
            doc.Open();
            htmlWorker.StartDocument();

            //Aggiunta immagine
            try
            {
                Image png;
                png = Image.GetInstance(new Uri(_documentsServiceSettings.BaseUrl + "/img/logopresidenza.png"));
                png.ScalePercent(36f);
                png.Alignment = Image.TEXTWRAP;
                doc.Add(png);
            }
            catch (Exception ex)
            {
                //Va avanti la generazione del pdf anche senza l'immagine del logo in intestazione
            }

            htmlWorker.Parse(txtReader);
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            //Restituzione del filestreamresult
            var contentType = "application/pdf";
            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, contentType);
        }
    }
}

    //Classe per gestire Header e Footer del documento PDF
public class HeaderFooterAdd : PdfPageEventHelper
{
    private string _headerText;
    public HeaderFooterAdd(string headerText) 
    {
        _headerText = headerText;
    }

    //Aggiunge un testo all'header di ogni pagina e la numerazione delle pagine nel footer
    public override void OnEndPage(PdfWriter writer, Document document)
    {
        //Watermark
        float fontSize = 50;
        float xPosition = 300;
        float yPosition = 400;
        float angle = 45;

        PdfContentByte under = writer.DirectContentUnder;
        BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
        under.BeginText();
        under.SetColorFill(BaseColor.LightGray);
        under.SetFontAndSize(baseFont, fontSize);
        under.ShowTextAligned(PdfContentByte.ALIGN_CENTER, _headerText, xPosition, yPosition, angle);
        under.EndText();

        //Footer
        PdfContentByte cb = writer.DirectContent;
        Font ffont = new Font(Font.BOLD, 10);
        Phrase footer = new Phrase(document.PageNumber.ToString(), ffont);

        ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
                footer,
                (document.Right - document.Left) / 2 + document.LeftMargin,
                document.Bottom - 10, 0);
    }
}    

