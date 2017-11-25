namespace Sym17.Web
{
    using System;

    using Models.Contact;

    public delegate void ActiveVisitorChanged(object sender, ActiveVisitorChangedEventArgs e);

    public static class ActiveVisitor
    {
        public static event ActiveVisitorChanged VisitorChanged;

        public static event ActiveVisitorChanged VisitorUpdated;

        private static ImageProcessingContact contact;

        public static ImageProcessingContact Get()
        {
            return contact;
        }

        public static void Set(ImageProcessingContact imageProcessingContact)
        {
            bool changed = contact != imageProcessingContact;

            contact = imageProcessingContact;

            if (changed)
            {
                if (VisitorChanged != null)
                {
                    VisitorChanged("", new ActiveVisitorChangedEventArgs(contact));
                }
            }

            if (VisitorUpdated != null)
            {
                VisitorUpdated("", new ActiveVisitorChangedEventArgs(contact));
            }
        }
    }

    public class ActiveVisitorChangedEventArgs : EventArgs
    {
        public ActiveVisitorChangedEventArgs(ImageProcessingContact imageProcessingContact)
        {
            this.ImageProcessingContact = imageProcessingContact;
        }

        public ActiveVisitorChangedEventArgs()
        {
        }

        public ImageProcessingContact ImageProcessingContact { get; set; }
    }
}