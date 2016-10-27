using System;
using System.Reflection;
using Omu.ValueInjecter.Injections;

namespace Auction.BussinessLogic.Infrastructure
{
    public class NoNullsInjection : LoopInjection
    {
        protected override void SetValue(object source, object target, PropertyInfo sp, PropertyInfo tp)
        {
            if (sp.GetValue(source) == null || sp.GetValue(source).Equals(Guid.Empty) || sp.GetValue(source).Equals(TimeSpan.Zero) || sp.GetValue(source).Equals(DateTime.MinValue) || sp.GetValue(source).Equals(default(int)))
            {
                return;
            } 

            base.SetValue(source, target, sp, tp);
        }
    }
}
