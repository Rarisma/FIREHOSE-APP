using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirehoseApp.Controls;

namespace FirehoseApp.Views.Models;

class ShellVM : ObservableObject
{

    public LoadableCommand LoadPublicationDataCommand { get; }
    public LoadableCommand LoadContent1Command { get; }
}
