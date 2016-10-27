using System;
using System.Reflection;
using Omu.ValueInjecter.Injections;

namespace Auction.Presentation.Infrastructure
{
    public class NoNullsInjection : LoopInjection
    {
        /* protected override bool MatchTypes(Type source, Type target)
         {
             return source.Name == target.Name && ((source.Name == "Login" && target.Name == "Username") || (source.Name == "Username" && target.Name == "Login"));
         }*/

        protected override void SetValue(object source, object target, PropertyInfo sp, PropertyInfo tp)
        {
            if (sp.GetValue(source) == null || sp.GetValue(source).Equals(Guid.Empty) || sp.GetValue(source).Equals(TimeSpan.Zero))
            {
                return;
            } 

                base.SetValue(source, target, sp, tp);
        }
    }
}