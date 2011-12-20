using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace ShiroChan
{
    public enum EditType
    {
        Output,
        Console
    }
    public class EditPanel
    {
        public EditType Type = EditType.Console;
        public string Text = "";
        public string FileName = "";

        public EditPanel(EditType type, string txt)
        {
            Type = type;
            Text = txt;
        }
        public EditPanel(EditType type, string txt, string file)
        {
            Type = type;
            Text = txt;
            FileName = file;
        }
    }
    
    public class EditFrame
    {
        public EditPanel[] Panels = new EditPanel[2];
        public int DividerScale = 225;

        public EditFrame(EditType left, EditType right)
        {
            Panels[0] = new EditPanel(left, "");
            Panels[1] = new EditPanel(right, "");
        }

        internal void SetActive(RichTextBox LeftText, ScintillaNet.Scintilla RightText, System.Windows.Forms.Label RightTitle, SplitContainer split)
        {
            if (EditType.Output == Panels[0].Type)
                LeftText.BackColor = Color.LightYellow;
            else
                LeftText.BackColor = Color.White;
            LeftText.Text = Panels[0].Text;

            RightTitle.Text = Panels[1].Type.ToString();
            if (EditType.Output == Panels[1].Type)
                RightText.BackColor = Color.LightYellow;
            else
                RightText.BackColor = Color.White;
            RightText.Text = Panels[1].Text;

            split.SplitterDistance = DividerScale;
            RightText.Focus();
        }
    }
}
