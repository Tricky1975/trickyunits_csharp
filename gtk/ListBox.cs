// Lic:
//   ListBox.cs
//   
//   version: 18.09.19
//   Copyright (C) 2018 Jeroen P. Broks
//   This software is provided 'as-is', without any express or implied
//   warranty.  In no event will the authors be held liable for any damages
//   arising from the use of this software.
//   Permission is granted to anyone to use this software for any purpose,
//   including commercial applications, and to alter it and redistribute it
//   freely, subject to the following restrictions:
//   1. The origin of this software must not be misrepresented; you must not
//      claim that you wrote the original software. If you use this software
//      in a product, an acknowledgment in the product documentation would be
//      appreciated but is not required.
//   2. Altered source versions must be plainly marked as such, and must not be
//      misrepresented as being the original software.
//   3. This notice may not be removed or altered from any source distribution.
// EndLic
using Gtk;
using System.Collections.Generic;

namespace TrickyUnits.GTK
{
    /// <summary>
    /// Creates a listbox (acutually the TreeView widget will be used for that, but it's use is heavily simplefied)
    /// </summary>

    class ListBox {
        /// <summary>
        /// The true treeview gadget, but it's advised only to call this for features that really require to point to the gadget itself.
        /// </summary>
        readonly public TreeView Gadget;

        readonly List<string> items = new List<string>();

        void Update(){
            var lst = new ListStore(typeof(string));
            foreach (string k in items)
                lst.AppendValues(k);
            Gadget.Model = lst;
        }

        public void SetSizeRequest(int w, int h) => Gadget.SetSizeRequest(w, h);

        /// <summary>
        /// Adds the listbox to a parent. It can only be added to a box at the current time.
        /// </summary>
        /// <param name="parent">Parent.</param>
        public void AddTo(Box parent) { parent.Add(Gadget); }

        public void AddItem(string itemtext){
            items.Add(itemtext);
            Update();
        }

        /// <summary>
        /// Remove all items from the listbox
        /// </summary>
        public void Clear() { items.Clear(); Update(); }

        public string ItemText { 
            get{
                TreeSelection selection = Gadget.Selection;
                TreeModel model;
                TreeIter iter;
                if (selection.GetSelected(out model, out iter))
                {
                    var rec = (model.GetValue(iter, 0) as string);
                    return rec;
                }
                return "";
            }
        }

        public ListBox(){
            Gadget = new TreeView();
            var tvc = new TreeViewColumn();
            var NameCell = new CellRendererText();
            tvc.Title = "Items";
            tvc.PackStart(NameCell, true);
            tvc.AddAttribute(NameCell, "text", 0);
            //ListRecords.HeightRequest = 800 - 390;
            Gadget.AppendColumn(tvc);
            ListBox.Hello();
        }

        static public void Hello(){
            MKL.Version("Tricky Units for C# - ListBox.cs","18.09.19");
            MKL.Lic    ("Tricky Units for C# - ListBox.cs","ZLib License");
        }
    }
}
