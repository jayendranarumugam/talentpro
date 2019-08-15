using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentClassification
{
    class DocumentExtraction
    {
        public static string GetTextFromPDF(PdfReader reader)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            }
            return text.ToString();
        }
        private static List<Image> GetImagesFromPdfDict(PdfDictionary dict, PdfReader doc)
        {
            List<Image> images = new List<Image>();
            PdfDictionary res = (PdfDictionary)(PdfReader.GetPdfObject(dict.Get(PdfName.RESOURCES)));
            PdfDictionary xobj = (PdfDictionary)(PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT)));

            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)(PdfReader.GetPdfObject(obj));
                        PdfName subtype = (PdfName)(PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE)));
                        if (PdfName.IMAGE.Equals(subtype))
                        {
                            int xrefIdx = ((PRIndirectReference)obj).Number;
                            PdfObject pdfObj = doc.GetPdfObject(xrefIdx);
                            PdfStream str = (PdfStream)(pdfObj);

                            PdfImageObject pdfImage =
                                new iTextSharp.text.pdf.parser.PdfImageObject((PRStream)str);
                            System.Drawing.Image img = pdfImage.GetDrawingImage();

                            images.Add(img);
                        }
                        else if (PdfName.FORM.Equals(subtype) || PdfName.GROUP.Equals(subtype))
                        {
                            images.AddRange(GetImagesFromPdfDict(tg, doc));
                        }
                    }
                }
            }

            return images;
        }
        public static List<CloudBlockBlob> ExtractImageUploadToAzure(PdfReader pdfReader, Stream blob, TraceWriter log, string name, ResumeDocModel resumeDocModel)
        {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(blob);
            List<CloudBlockBlob> cloudBlockBlobs = new List<CloudBlockBlob>();
            AzureStorageModel azureStorageModel = AzStorage.GetBlobDirectoryList(name);

            if (azureStorageModel.IblobList.Count() > 0)
            {
                foreach (IListBlobItem listBlobItem in azureStorageModel.IblobList)
                {
                    CloudBlockBlob cloudBlob = listBlobItem as CloudBlockBlob;
                    cloudBlob.DeleteIfExists();
                }

            }
            try
            {
                for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdfReader.GetPageN(pageNumber);
                    List<Image> IlistImages = GetImagesFromPdfDict(pg, pdfReader);
                    EncoderParameters parms = new EncoderParameters(1);
                    parms.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                    var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    foreach (Image image in IlistImages)
                    {
                        MemoryStream ms = new MemoryStream();
                        CloudBlockBlob cloudBlob = azureStorageModel.Container.GetBlockBlobReference(name + "/" + Guid.NewGuid() + ".jpeg");
                        image.Save(ms, encoder, parms);
                        ms.Seek(0, SeekOrigin.Begin); // otherwise you'll get zero byte files
                        cloudBlob.UploadFromStream(ms);
                        cloudBlockBlobs.Add(cloudBlob);
                        resumeDocModel.imageDetails.imageURLList.Add(cloudBlob.Uri.ToString());
                    }


                }
            }
            catch
            {
                throw;
            }
            finally
            {
                pdfReader.Close();
            }
            return cloudBlockBlobs;
        }
    }
}
