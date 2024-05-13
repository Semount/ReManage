using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using NodaTime.Text;

namespace ReManage.Core
{
    public class PeriodConverter : ValueConverter<Period, string>
    {
        public PeriodConverter() : base(
        v => v.ToString(), // Convert `Period` to string
        v => PeriodPattern.Roundtrip.Parse(v).Value // Convert string back to `Period`
        )
        {
        }
    }
}
