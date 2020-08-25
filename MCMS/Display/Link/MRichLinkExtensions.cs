namespace MCMS.Display.Link
{
    public static class MRichLinkExtensions
    {
        public static T AsButton<T>(this T value, string cssClasses) where T : MRichLink
        {
            value.CssClasses = "btn btn-" + cssClasses;
            return value;
        }

        public static T ToggleModal<T>(this T value, bool isWith) where T : MRichLink
        {
            value.AsModal = isWith;
            return value;
        }

        public static T WithModal<T>(this T value, string backdrop = "static",
            ModalSuccessAction modalSuccessAction = ModalSuccessAction.RunCallback) where T : MRichLink
        {
            value.ModalBackdrop = backdrop;
            value.AsModal = true;
            value.ModalSuccessAction = modalSuccessAction;
            return value;
        }

        public static T WithValues<T>(this T value, object values) where T : MRichLink
        {
            value.Values = values;
            return value;
        }
    }
}