using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace FindToolForm
{
    public partial class frmFormTool : Form
    {
        XElement xldocToolName;
        XElement xldocProgramNames;
        Boolean bWhatUpAvail = false;
        Boolean bFindFSPProjAvail = false;

        public frmFormTool()
        {
            InitializeComponent();
            //xldocToolName = XElement.Load(@"C:\Users\steven chavez\Documents\dbwork\Tool_list.xml");
            //xldocProgramNames = XElement.Load(@"C:\Users\steven chavez\Documents\dbwork\FSPTOOL.XML");

            xldocToolName = XElement.Load("Tool_list.xml");
            xldocProgramNames = XElement.Load("FSPTOOL.XML");

            FileInfo fiWhatupTest = new FileInfo("Whatup.exe");
            if (fiWhatupTest.Exists)
            {
                bWhatUpAvail = true;
            }
            FileInfo fiFindProjTest = new FileInfo("FindFSPProj.exe");
            if (fiFindProjTest.Exists)
            {
                bFindFSPProjAvail = true;
            }

        }

        private void btnLookup_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Length == 0)
            {
                MessageBox.Show("Search text cannot be null", "FindTool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            //Search for tool description

            var vToolNameList =
                from CStool in xldocToolName.Elements("TOOLDESC")
                where CStool.Element("CAPTION").Value.ToUpper().Contains(txtSearch.Text.ToUpper()) || CStool.Element("TOOLNAME").Value.ToUpper().Contains(txtSearch.Text.ToUpper())
                orderby CStool.Element("TOOLNAME").Value ascending
                select new
                {
                    name = CStool.Element("TOOLNAME").Value,
                    cap = CStool.Element("CAPTION").Value
                };

            if (vToolNameList.Count() == 0)
            {
                MessageBox.Show("Search text " + txtSearch.Text + " not Found","FindTool", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            else
            {

                tv.Nodes.Clear();
                tv.BeginUpdate();
                foreach (var vTinfo in vToolNameList)
                {
                    string sHead = string.Format("{0}-{1}", (string)vTinfo.name, (string)vTinfo.cap);

                    TreeNode tnToolNode = new TreeNode(sHead);
                    tnToolNode.ForeColor = Color.Blue;
                    tnToolNode.Name = (string)vTinfo.name;

                    var vToolProgramList =
                        from Tool in xldocProgramNames.Elements("TOOL")
                        where Tool.Element("ToolName").Value.Equals(vTinfo.name)
                        orderby Tool.Element("HostProgram").Value ascending
                        select new
                        {
                            pgmlist = Tool.Element("HostProgram").Value,
                            vbProgram = Tool.Element("CallProgram").Value
                        };


                    foreach (var vProginfo in vToolProgramList)
                    {
                        tnToolNode.Nodes.Add(new TreeNode(vProginfo.pgmlist));
                    }

                    tv.Nodes.Add(tnToolNode);
                }
                tv.EndUpdate();
            }
        }

        private void tv_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 1 && bWhatUpAvail)
            {
                string strProgram = e.Node.Text;
                //            MessageBox.Show(txtnothing, "FindTool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Process pWhatif = new Process();
                //pWhatif.StartInfo.FileName = @"C:\Users\steven chavez\Documents\Visual Studio 2010\Projects\WhatUp\WhatUp\bin\Release\Whatup.exe";
                pWhatif.StartInfo.FileName = "Whatup.exe";
                pWhatif.StartInfo.Arguments = strProgram;
                pWhatif.Start();
            }
           
        }

        private void tv_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0 && bFindFSPProjAvail)
            {
                String sToolName = e.Node.Name;
                //MessageBox.Show(sToolName, "FindTool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Process pFindProj = new Process();

                pFindProj.StartInfo.FileName = "FindFSPProj.exe";
                pFindProj.StartInfo.Arguments = sToolName;
                pFindProj.Start();

            }
        }
    }

}

