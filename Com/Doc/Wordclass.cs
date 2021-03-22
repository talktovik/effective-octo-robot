using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;

namespace QM.Com.Doc
{
    class Wordclass
    {
        public void documentMaker() {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Writeln("This Is the test !");
            builder.InsertBreak(BreakType.PageBreak);
            doc.Save("Document.docx");

        
        }
    }
}
