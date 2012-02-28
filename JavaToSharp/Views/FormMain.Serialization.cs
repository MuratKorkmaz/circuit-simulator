using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class FormMain
    {
        private string _startSircuitPath;

        private void SerializeMenu(ToolStripMenuItem ownerMenu)
        {
            const string path = "setuplist.txt";
            var data = new List<string>(readUrlData(path));
            SerializeSubmenu(data, ownerMenu);
        }

        private string[] readUrlData(string url)
        {
            string[] data = null;
            if (!File.Exists(url))
            {
                throw new Exception(string.Format("File {0} is no exists", url));
            }
            try
            {
                data = File.ReadAllLines(url, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
            }
            return data;
        }

        private int SerializeSubmenu(List<string> data, ToolStripMenuItem ownerMenu)
        {
            var menuItems = new List<ToolStripMenuItem>();
            int counter = 0;
            try
            {
                int len = data.Count;
                for (int i = 0; i < len; i++)
                {
                    string line = data[i];
                    if (line[0] == '#')
                        continue;
                    if (line[0] == '+')
                    {
                        var subMenuItem = new ToolStripMenuItem(line.Substring(1));
                        var submenuData = data.GetRange(i + 1, len - i - 1);
                        i = i + 1 + SerializeSubmenu(submenuData, subMenuItem);
                        menuItems.Add(subMenuItem);
                    }
                    else if (line[0] == '-')
                    {
                        counter = i;
                        break;
                    }
                    else
                    {
                        bool isFirst = (line[0] == '>');
                        int start = isFirst ? 1 : 0;
                        int firstSpaceIndex = line.IndexOf(' ');
                        string filePath = line.Substring(start, firstSpaceIndex);
                        string title = line.Substring(firstSpaceIndex + 1);
                        var item = new ToolStripMenuItem(title) {Tag = filePath};
                        item.Click += tsmiScheme_Click;
                        menuItems.Add(item);
                        if (isFirst)
                        {
                            _startSircuitPath = filePath;
                        }
                    }
                }
                ownerMenu.DropDownItems.AddRange(menuItems.ToArray());
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
                _simController.stop("Can't read setuplist.txt!", null);
            }
            return counter;
        }

        private void SerializeScheme(string path)
        {
            _simController.t = 0;
            try
            {
                const string folder = "circuits/";
                string url = folder + path;
                string[] data = readUrlData(url);
                readSetup(data);
            }
            catch (Exception e)
            {
                UserMessageView.Instance.ShowError(e.StackTrace);
                _simController.stop("Unable to read " + path + "!", null);
            }
            //titleLabel.Text = title;
            _simController.needAnalyze();
        }

        private void readSetup(string[] lines)
        {
            StringTokenizer st = new StringTokenizer(lines);
            while (st.hasMoreTokens())
            {
                string type = st.nextToken();
                int tint = type[0];
                try
                {
                    if (tint == 'o')
                    {
                        Scope sc = new Scope(_simController);
                        sc.position = _simController.scopeCount;
                        sc.undump(st);
                        _simController.scopes[_simController.scopeCount++] = sc;
                        continue;
                    }
                    if (tint == 'h')
                    {
                        readHint(st);
                        continue;
                    }
                    if (tint == '$')
                    {
                        readOptions(st);
                        continue;
                    }
                    if (tint == '%' || tint == '?' || tint == 'B')
                    {
                        // ignore afilter-specific stuff
                        continue;
                    }
                    if (tint >= '0' && tint <= '9')
                        tint = int.Parse(type);
                    int x1 = int.Parse(st.nextToken());
                    int y1 = int.Parse(st.nextToken());
                    int x2 = int.Parse(st.nextToken());
                    int y2 = int.Parse(st.nextToken());
                    int f = int.Parse(st.nextToken());
                    Type cls = _simController.dumpTypes[tint];
                    if (cls == null)
                    {
                        Console.WriteLine("unrecognized dump type: " + type);
                        break;
                    }
                    // find element class
                    var carr = new Type[6];
                    //carr[0] = getClass();
                    carr[0] = carr[1] = carr[2] = carr[3] = carr[4] = typeof(int);
                    carr[5] = typeof(StringTokenizer);
                    ConstructorInfo cstr = cls.GetConstructor(carr);

                    // invoke constructor with starting coordinates
                    var oarr = new object[6];
                    //oarr[0] = this;
                    oarr[0] = x1;
                    oarr[1] = y1;
                    oarr[2] = x2;
                    oarr[3] = y2;
                    oarr[4] = f;
                    oarr[5] = st;
                    var ce = (CircuitElm)cstr.Invoke(oarr);
                    ce.setPoints();
                    _simController.elmList.Add(ce);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    UserMessageView.Instance.ShowError(ex.InnerException.StackTrace);
                    break;
                }
                catch (Exception ex)
                {
                    UserMessageView.Instance.ShowError(ex.StackTrace);
                    break;
                }                
            }
        }

        private void readHint(StringTokenizer st)
        {
            _simController.hintType = int.Parse(st.nextToken());
            _simController.hintItem1 = int.Parse(st.nextToken());
            _simController.hintItem2 = int.Parse(st.nextToken());
        }

        private void readOptions(StringTokenizer st)
        {
            int flags = int.Parse(st.nextToken());
            string ssp = st.nextToken();
            _simController.timeStep = double.Parse(ssp);
            double sp = double.Parse(st.nextToken());
            int sp2 = (int)(Math.Log(10 * sp) * 24 + 61.5);
            //int sp2 = (int) (Math.log(sp)*24+1.5);
            _ucSimulationParameters.SimulationSpeed = sp2;
            _ucSimulationParameters.CurrentSpeed = int.Parse(st.nextToken());
            CircuitElm.voltageRange = double.Parse(st.nextToken());
            try
            {
                _ucSimulationParameters.PowerLight = int.Parse(st.nextToken());
            }
            catch (Exception e)
            {
            }
            _simController.setGrid();
        }
    }
}