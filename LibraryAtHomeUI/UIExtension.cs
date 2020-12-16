using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibraryAtHomeUI
{

    public static class UIExtension
    {
        public static bool UiElementExistsByName(this Panel panel, string uiElementName)
        {
            var res = from elem in panel.Children.OfType<Control>()
                where elem.Name == uiElementName
                select elem;

            if (res.Count() != 0)
                return true;
            return false;
        }

        public static UIElement GetUiElement(this Panel panel, string uiElementName)
        {
            foreach (Control c in panel.Children)
                if (c.Name == uiElementName)
                    return (UIElement)c;
            return null;
        }


        
    }
}

