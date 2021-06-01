//using unoidl.com.sun.star.lang;
//using unoidl.com.sun.star.uno;
//using unoidl.com.sun.star.container;
//using unoidl.com.sun.star.frame;
//using unoidl.com.sun.star.beans;
//using unoidl.com.sun.star.view;
//using System.Collections.Generic;
//using System.IO;
//using System;
//using System.Diagnostics;

//namespace Ecoporto.CRM.Site.Helpers
//{
//    public enum AppType
//    {
//        Writer,
//        Calc,
//        Impress,
//        Draw,
//        Math
//    }

//    public enum ExportFilter
//    {
//        Word97,
//        WriterPDF,
//        CalcPDF,
//        DrawPDF,
//        ImpressPDF,
//        MathPDF
//    }

//    public class OpenOffice
//    {
//        public static void ConvertToPDF(string inputFile, string outputFile)
//        {
//            if (ConvertExtensionToFilterType(Path.GetExtension(inputFile)) == null)
//                throw new InvalidProgramException("Unknown file type for OpenOffice. File = " + inputFile);

//            StartOpenOffice();

//            //Get a ComponentContext
//            unoidl.com.sun.star.uno.XComponentContext xLocalContext =
//               uno.util.Bootstrap.bootstrap();
//            //Get MultiServiceFactory
//            unoidl.com.sun.star.lang.XMultiServiceFactory xRemoteFactory =
//               (unoidl.com.sun.star.lang.XMultiServiceFactory)
//               xLocalContext.getServiceManager();
//            //Get a CompontLoader
//            XComponentLoader aLoader =
//               (XComponentLoader)xRemoteFactory.createInstance("com.sun.star.frame.Desktop");
//            //Load the sourcefile

//            XComponent xComponent = null;
//            try
//            {
//                xComponent = initDocument(aLoader,
//                   PathConverter(inputFile), "_blank");
//                //Wait for loading
//                while (xComponent == null)
//                {
//                    System.Threading.Thread.Sleep(1000);
//                }

//                // save/export the document
//                saveDocument(xComponent, inputFile, PathConverter(outputFile));

//            }
//            catch { throw; }
//            finally { xComponent.dispose(); }

//        }

//        private static void StartOpenOffice()
//        {
//            Process[] ps = Process.GetProcessesByName("soffice.exe");
//            if (ps != null)
//            {
//                if (ps.Length > 0)
//                    return;
//                else
//                {
//                    Process p = new Process();
//                    p.StartInfo.Arguments = "-headless -nofirststartwizard";
//                    p.StartInfo.FileName = "soffice.exe";
//                    p.StartInfo.CreateNoWindow = true;
//                    bool result = p.Start();
//                    if (result == false)
//                        throw new InvalidProgramException("OpenOffice failed to start.");
//                }
//            }
//            else
//            {
//                throw new InvalidProgramException("OpenOffice not found.  Is OpenOffice installed?");
//            }
//        }

//        private static XComponent initDocument(XComponentLoader aLoader, string file, string target)
//        {
//            PropertyValue[] openProps = new PropertyValue[1];
//            openProps[0] = new PropertyValue();
//            openProps[0].Name = "Hidden";
//            openProps[0].Value = new uno.Any(true);


//            XComponent xComponent = aLoader.loadComponentFromURL(
//               file, target, 0,
//               openProps);

//            return xComponent;
//        }


//        private static void saveDocument(XComponent xComponent, string sourceFile, string destinationFile)
//        {
//            PropertyValue[] propertyValues = new PropertyValue[2];
//            propertyValues = new PropertyValue[2];
//            // Setting the flag for overwriting 
//            propertyValues[1] = new PropertyValue();
//            propertyValues[1].Name = "Overwrite";
//            propertyValues[1].Value = new uno.Any(true);
//            //// Setting the filter name 
//            propertyValues[0] = new PropertyValue();
//            propertyValues[0].Name = "FilterName";
//            propertyValues[0].Value = new uno.Any(ConvertExtensionToFilterType(Path.GetExtension(sourceFile)));
//            ((XStorable)xComponent).storeToURL(destinationFile, propertyValues);

//        }


//        private static string PathConverter(string file)
//        {
//            if (file == null || file.Length == 0)
//                throw new NullReferenceException("Null or empty path passed to OpenOffice");

//            return String.Format("file:///{0}", file.Replace(@"\", "/"));

//        }

//        public static string ConvertExtensionToFilterType(string extension)
//        {
//            switch (extension)
//            {
//                case ".doc":
//                case ".docx":
//                case ".txt":
//                case ".rtf":
//                case ".html":
//                case ".htm":
//                case ".xml":
//                case ".odt":
//                case ".wps":
//                case ".wpd":
//                    return "writer_pdf_Export";
//                case ".xls":
//                case ".xlsb":
//                case ".ods":
//                    return "calc_pdf_Export";
//                case ".ppt":
//                case ".pptx":
//                case ".odp":
//                    return "impress_pdf_Export";

//                default: return null;
//            }
//        }


//    }

//    //private XComponentContext context;
//    //private XMultiServiceFactory service;
//    //private XComponentLoader component;
//    //private XComponent doc;

//    //private List<string> filters = new List<string>();

//    //#region Constructors
//    //public OpenOffice()
//    //{
//    //    /// This will start a new instance of OpenOffice.org if it is not running, 
//    //    /// or it will obtain an existing instance if it is already open.
//    //    context = uno.util.Bootstrap.bootstrap();

//    //    /// The next step is to create a new OpenOffice.org service manager
//    //    service = (XMultiServiceFactory)context.getServiceManager();

//    //    /// Create a new Desktop instance using our service manager
//    //    component = (XComponentLoader)service.createInstance("com.sun.star.frame.Desktop");

//    //    // Getting filters
//    //    XNameContainer filters = (XNameContainer)service.createInstance("com.sun.star.document.FilterFactory");
//    //    foreach (string filter in filters.getElementNames())
//    //        this.filters.Add(filter);
//    //}

//    //~OpenOffice()
//    //{
//    //    if (doc != null)
//    //        doc.dispose();
//    //    doc = null;
//    //}
//    //#endregion

//    //#region Private methods
//    //private string FilterToString(ExportFilter filter)
//    //{
//    //    switch (filter)
//    //    {
//    //        case ExportFilter.Word97: return "MS Word 97";
//    //        case ExportFilter.WriterPDF: return "writer_pdf_Export";
//    //        case ExportFilter.CalcPDF: return "calc_pdf_Export";
//    //        case ExportFilter.DrawPDF: return "draw_pdf_Export";
//    //        case ExportFilter.ImpressPDF: return "impress_pdf_Export";
//    //        case ExportFilter.MathPDF: return "math_pdf_Export";
//    //    }
//    //    return "";
//    //}
//    //#endregion

//    //#region Public methods
//    //public bool Load(string filename, bool hidden)
//    //{
//    //    return Load(filename, hidden, "", "");
//    //}
//    //public bool Load(string filename, bool hidden, int filter_index, string filter_options)
//    //{
//    //    return Load(filename, hidden, filters[filter_index], filter_options);
//    //}
//    //public bool Load(string filename, bool hidden, string filter_name, string filter_options)
//    //{
//    //    List<PropertyValue> pv = new List<PropertyValue>();
//    //    pv.Add(new PropertyValue("Hidden", 0, new uno.Any(hidden), PropertyState.DIRECT_VALUE));
//    //    if (filter_name != "")
//    //    {
//    //        pv.Add(new PropertyValue("FilterName", 0, new uno.Any(filter_name), PropertyState.DIRECT_VALUE));
//    //        pv.Add(new PropertyValue("FilterOptions", 0, new uno.Any(filter_options), PropertyState.DIRECT_VALUE));
//    //    }

//    //    try
//    //    {
//    //        doc = component.loadComponentFromURL(
//    //            "file:///" + filename.Replace('\\', '/'), "_blank",
//    //            0, pv.ToArray());
//    //        return true;
//    //    }
//    //    catch
//    //    {
//    //        doc = null;
//    //        return false;
//    //    }
//    //}
//    //public bool Print()
//    //{
//    //    return Print(1, "");
//    //}
//    //public bool Print(int copies, string pages)
//    //{
//    //    List<PropertyValue> pv = new List<PropertyValue>();
//    //    pv.Add(new PropertyValue("CopyCount", 0, new uno.Any(copies), PropertyState.DIRECT_VALUE));
//    //    if (pages != "")
//    //        pv.Add(new PropertyValue("Pages", 0, new uno.Any(pages), PropertyState.DIRECT_VALUE));
//    //    //if (doc is XPrintable)
//    //    try
//    //    {
//    //        ((XPrintable)doc).print(pv.ToArray());
//    //        return true;
//    //    }
//    //    catch { return false; }
//    //}
//    //public bool Save(string filename, ExportFilter filter)
//    //{
//    //    return Save(filename, FilterToString(filter));
//    //}
//    //public bool Save(string filename, string filter)
//    //{
//    //    List<PropertyValue> pv = new List<PropertyValue>();
//    //    pv.Add(new PropertyValue("FilterName", 0, new uno.Any(filter), PropertyState.DIRECT_VALUE));
//    //    pv.Add(new PropertyValue("Overwrite", 0, new uno.Any(true), PropertyState.DIRECT_VALUE));
//    //    try
//    //    {
//    //        filename = filename.Replace("\\", "/");
//    //        ((XStorable)doc).storeToURL("file:///" + filename, pv.ToArray());
//    //        return true;
//    //    }
//    //    catch(Exception ex)
//    //    {
//    //        return false;
//    //    }
//    //}
//    //public bool ExportToPdf(string filename)
//    //{
//    //    filename = Path.ChangeExtension(filename, ".pdf");
//    //    bool ret = Save(filename, "writer_pdf_Export");
//    //    if (!ret) ret = Save(filename, "impress_pdf_Export");
//    //    if (!ret) ret = Save(filename, "calc_pdf_Export");
//    //    if (!ret) ret = Save(filename, "draw_pdf_Export");
//    //    if (!ret) ret = Save(filename, "impress_pdf_Export");
//    //    if (!ret) ret = Save(filename, "math_pdf_Export");
//    //    return ret;
//    //}
//    //public void Close()
//    //{
//    //    doc.dispose();
//    //    doc = null;
//    //}

//    //public bool New(AppType app, bool hidden)
//    //{
//    //    try
//    //    {
//    //        string sapp = "private:factory/";
//    //        switch (app)
//    //        {
//    //            case AppType.Writer:
//    //                sapp += "swriter";
//    //                break;
//    //            case AppType.Calc:
//    //                sapp += "scalc";
//    //                break;
//    //            case AppType.Impress:
//    //                sapp += "simpress";
//    //                break;
//    //            case AppType.Draw:
//    //                sapp += "sdraw";
//    //                break;
//    //            case AppType.Math:
//    //                sapp += "smath";
//    //                break;
//    //        }
//    //        PropertyValue pv = new PropertyValue("Hidden", 0, new uno.Any(hidden), PropertyState.DIRECT_VALUE);
//    //        doc = component.loadComponentFromURL(sapp, "_blank", 0, new PropertyValue[1] { pv });
//    //        return true;
//    //    }
//    //    catch
//    //    {
//    //        doc = null;
//    //        return false;
//    //    }
//    //}
//    //#endregion


//    //#region Properties
//    //public List<string> Filters
//    //{
//    //    get { return filters; }
//    //}

//}