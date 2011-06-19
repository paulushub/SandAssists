using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestLibrary
{
    public abstract class DependencyObjects : DependencyObject
    {
        public enum Bouyancy
        {
            Floats,
            Sinks,
            Drifts
        }
        public static readonly DependencyProperty BouyancyProperty = DependencyProperty.RegisterAttached(
          "Bouyancy",
          typeof(Bouyancy),
          typeof(DependencyObjects),
          new FrameworkPropertyMetadata(Bouyancy.Floats, FrameworkPropertyMetadataOptions.AffectsArrange),
          new ValidateValueCallback(ValidateBouyancy)
        );
        public static void SetBouyancy(UIElement element, Bouyancy value)
        {
            element.SetValue(BouyancyProperty, value);
        }
        public static Bouyancy GetBouyancy(UIElement element)
        {
            return (Bouyancy)element.GetValue(BouyancyProperty);
        }
        private static bool ValidateBouyancy(object value)
        {
            Bouyancy bTest = (Bouyancy)value;
            return (bTest == Bouyancy.Floats || bTest == Bouyancy.Drifts || bTest == Bouyancy.Sinks);
        }
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
          "IsDirty",
          typeof(Boolean),
          typeof(DependencyObjects)
        );

        public Boolean State
        {
            get { return (Boolean)this.GetValue(StateProperty); }
            set { this.SetValue(StateProperty, value); }
        }
        
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
          "State", typeof(Boolean), typeof(DependencyObjects), new PropertyMetadata(false));

    }

}
