/*using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class ImportDialog 
    {
        internal CirSim cframe;
        internal Button importButton, closeButton;
       // internal HtmlHelper text;
        internal bool isURL;
        

        internal ImportDialog(CirSim f, string str, bool url) //: base(f, (str.Length > 0) ? "Export" : "Import", false)
        {
            isURL = url;
            cframe = f;
        //    Layout = new ImportDialogLayout();
         //   add(text = new TextArea(str, 10, 60, TextArea.SCROLLBARS_BOTH));
           // importButton = new Button("Import");
         //   if (!isURL)
          //      add(importButton);
            //importButton.addActionListener(this);
         //   add(closeButton = new Button("Close"));
           // closeButton.addActionListener(this);
            Point x = new Point(0, 0);
          //  resize(400, 300);
           // Dimension d = Size;
           // setLocation(x.X + (cframe.winSize.Width-d.width)/2, x.Y + (cframe.winSize.Height-d.height)/2);
           // show();
            //if (str.Length > 0)
             //   text.selectAll();
        }

        public virtual void actionPerformed(ActionEvent e)
        {
            int i;
            object src = e.Source;
            if (src == importButton)
            {
                cframe.readSetup(text.Text);
                Visible = false;
            }
            if (src == closeButton)
                Visible = false;
        }

        public virtual bool handleEvent(Event ev)
        {
            if (ev.id == Event.WINDOW_DESTROY)
            {
                //CirSim.main.requestFocus();
                Visible = false;
                cframe.impDialog = null;
                return true;
            }
            return base.handleEvent(ev);
        }
    }
}*/
