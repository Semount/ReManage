using System;

namespace ReManage.ViewModels
{
    public class RefrigeratorViewModel : ProductViewModel
    {
        public TimeSpan UnfreezeTime { get; set; }
        public string UnfreezeTimeFormatted => $"{UnfreezeTime.Hours:D2}:{UnfreezeTime.Minutes:D2}:{UnfreezeTime.Seconds:D2}";
    }
}