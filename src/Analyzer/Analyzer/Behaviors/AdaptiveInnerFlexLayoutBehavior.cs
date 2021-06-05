using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Reflection;

namespace Analyzer.Behaviors
{
    public static class AdaptiveInnerFlexLayoutBehavior
    {
        public static readonly BindableProperty AdjustDimensionsProperty =
            BindableProperty.CreateAttached(
                "AdjustDimensions",
                typeof(bool),
                typeof(AdaptiveInnerFlexLayoutBehavior),
                false,
                propertyChanged: OnAdjustDimensionsChanged);

        public static bool GetAdjustDimensions(BindableObject view)
        {
            return (bool)view.GetValue(AdjustDimensionsProperty);
        }

        public static void SetAdjustDimensions(BindableObject view, bool value)
        {
            view.SetValue(AdjustDimensionsProperty, value);
        }

        static void OnAdjustDimensionsChanged(BindableObject view, object oldValue, object newValue)
        {
            var layout = view as FlexLayout;
            if (layout == null)
            {
                return;
            }

            bool attachBehavior = (bool)newValue;
            if (attachBehavior)
            {
                ComputeDimensions();
                if(layout.HeightRequest < 1)
                {
                    layout.HeightRequest = 2;
                }

                layout.PropertyChanged += Layout_PropertyChanged;
            }
            else
            {
                layout.PropertyChanged += Layout_PropertyChanged;
            }
        }

        private static void Layout_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var  layout = sender as FlexLayout;
            var propertyName = e.PropertyName.ToLowerInvariant();

            if ( propertyName != "height" && propertyName != "widthrequest") return;

            var childrenCount = layout.Children.Count;
            var minHeight = -1;
            var xLocations = new Dictionary<double, int>();
            var minHeightMultiplicator = 1;


            for(var i =0; i < childrenCount; i++)
            {
                var view = layout.Children[i];
                Size request = view.Measure(Math.Abs(layout.MinimumHeightRequest), layout.WidthRequest).Request;
                var xLocation = view.X;

                if((request.Height < minHeight && request.Height > 1 ) || minHeight < 1)
                {
                    minHeight = Convert.ToInt32(request.Height);
                }

                if(xLocations.ContainsKey(xLocation))
                {
                    xLocations[xLocation] += 1;
                }
                else
                {
                    xLocations.Add(xLocation, 1);
                }
            }

            foreach(var locationCount in xLocations.Where(kvp => kvp.Value > 1).Select(kvp => kvp.Value).ToArray())
            {
                if (minHeightMultiplicator >= locationCount) continue;

                minHeightMultiplicator = locationCount;
            }

            var heightRequest = minHeight * minHeightMultiplicator;

            if (layout.HeightRequest != heightRequest)
            {
                layout.HeightRequest = heightRequest;
            }
        }

        private static void ComputeDimensions()
        {
        }

        private static void Parent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }
    }
}
