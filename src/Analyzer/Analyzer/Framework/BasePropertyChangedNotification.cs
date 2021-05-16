using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Analyzer.Framework
{
    public class BasePropertyChangedNotification : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetAndRaisePropertyChanged<TRef>(
            ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            this.RaisePropertyChanged(propertyName);
        }

        protected void SetAndRaisePropertyChangedIfDifferentValues<TRef>(
            ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
            where TRef : class
        {
            if (field == null || !field.Equals(value))
            {
                SetAndRaisePropertyChanged(ref field, value, propertyName);
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var listners = (MulticastDelegate)PropertyChanged;

            if (listners == null)
                return;

            foreach (var listner in listners.GetInvocationList())
            {
                if (listner.Target == null) { continue; }

                try
                {
                    listner.DynamicInvoke(this, new PropertyChangedEventArgs(propertyName));
                }
                catch (Exception ex)
                {
                    var test = ex;
                }
            }
        }
    }
}
