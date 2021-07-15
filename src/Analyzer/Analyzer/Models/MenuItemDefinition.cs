using Analyzer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Models
{
    public class MenuItemDefinition : IMenuItemDefinition, IEquatable<IMenuItemDefinition>
    {
        public MenuItemDefinition(int order, string nameTranslationKey, string image, string descriptionTranslationKey, Type viewType)
        {
            Order = order;
            NameTranslationKey = nameTranslationKey;
            DescriptionTranslationKey = descriptionTranslationKey;
            Image = image;
            ViewType = viewType;
        }
        public int Order { get; }

        public string NameTranslationKey { get; }

        public string DescriptionTranslationKey { get; }

        public string Image { get; }

        public Type ViewType { get; }

        public bool Equals(IMenuItemDefinition other)
        {
            if (other == null) return false;

            return this.Order == other.Order &&
                   this.NameTranslationKey == other.NameTranslationKey &&
                   this.DescriptionTranslationKey == other.DescriptionTranslationKey &&
                   this.Image == other.Image &&
                   this.ViewType.FullName == other.ViewType.FullName;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(NameTranslationKey);
        }
    }
}
