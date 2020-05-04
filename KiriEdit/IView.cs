/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriProj;
using System.Windows.Forms;

namespace KiriEdit
{
    internal interface IView
    {
        IShell Shell { get; set; }
        Project Project { get; set; }
        object ProjectItem { get; set; }

        Form Form { get; }
        string DocumentTitle { get; }
        bool IsDirty { get; }
        HistoryBuffer HistoryBuffer { get; }

        bool Save();
        void CloseView( bool force );
    }
}
