﻿using System;
using System.Collections.ObjectModel;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public interface ILocalSourceControlViewModel
    {
        ObservableCollection<IPackageViewModel> Packages { get; set; }

        string SearchQuery { get; set; }
        bool MatchWord { get; set; }

        string SortColumn { get; set; }
        bool SortDescending { get; set; }

        void Loaded(object sender, EventArgs e);
    }
}
