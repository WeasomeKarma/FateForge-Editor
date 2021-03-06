﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FateForge.Managers;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using FateForge.Managers.IO;
using FateForge.DataTypes;

namespace FateForge
{
    public partial class DeletionFieldContainer : UserControl, IIndependentResize, IXmlSerializable, IReferenceTableEntry
    {
        public EventHandler Deletion;
        public EventHandler Clicked;

        /// <summary>
        /// Returns the ControlCollection which is managed with resize.
        /// </summary>
        //public ControlCollection ManagedControls { get => panel1.Controls; }

        private bool _testAllClicks = false;
        public ControlCollection Contents { get => panel1.Controls; }

        public DeletionFieldContainer( bool _testAllClicks = false, params Control[] _addedControls)
        {
            InitializeComponent();

            this._testAllClicks = _testAllClicks;

            panel1.ControlAdded += (s, e) => CollapseManager.ResizeChilds(panel1);
            panel1.ControlRemoved += (s, e) => CollapseManager.ResizeChilds(panel1);

            foreach (Control c in _addedControls)
            {
                panel1.Controls.Add(c);
                if (_testAllClicks)
                    c.Click += (s, o) => { Clicked(s, o); };

            }

            if (_testAllClicks)
            {
                panel1.Click += (s, o) => { Clicked?.Invoke(s, o); };
                Click += (s, o) => { Clicked?.Invoke(s, o); };
            }

            ParentChanged += (s, e) => { if (Parent != null) CollapseManager.IndependentResize(this, panel1); };
        }

        public void ForceDeletion()
        {
            button1_Click(this, null);
        }

        public void IndependentResize()
        {
            if (Parent != null) CollapseManager.IndependentResize(this, panel1);
        }

        public void AddControl(Control c)
        {
            panel1.Controls.Add(c);
        }

        public void RemoveControl(Control c)
        {
            panel1.Controls.Remove(c);
        }

        //private void Panel1_ControlAdded(object sender, ControlEventArgs e)
        //{
        //    int _newHeight = 0;
        //    foreach (Control c in panel1.Controls)
        //    {
        //        c.Location = new Point(0, _newHeight);
        //        _newHeight += c.Height + 4;
        //    }

        //    Size = new Size(216, _newHeight);
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            Deletion?.Invoke(this, new EventArgs());
            Parent.Controls.Remove(this);
        }

        public void IndependentCollapse(bool collapseState)
        {

        }

        public int GetDesiredSize()
        {
            return MinimumSize.Height;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            panel1.Controls.AddRange(ImportManager.GetControlListFromXml(this, reader).ToArray());
        }

        public void WriteXml(XmlWriter writer)
        {
            ExportManager.ExportControlList(writer, panel1.Controls);
        }

        public void Reference()
        {
            foreach (IReferenceTableEntry refe in panel1.Controls)
                refe.Reference();
        }
    }
}
