using System;

namespace Sym17.Web.Models.Contact
{
    public class ImageProcessingContact
    {
        public ImageProcessingContact()
        {
            Created = DateTime.UtcNow;
        }

        public string Id { get; set; }

        public DateTime Created { get; set; }

        public byte[] Image { get; set; }
        public string Photo { get; set; }
        public string Photo48 { get; set; }

        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public int Age { get; set; }

        public string ParsedText { get; set; }

        public double Smile { get; set; }
        public float Happiness { get; set; }
        public float Anger { get; set; }
        public float Contempt { get; set; }
        public float Disgust { get; set; }
        public float Fear { get; set; }
        public float Neutral { get; set; }
        public float Sadness { get; set; }
        public float Surprise { get; set; }
        public string Glasses { get; set; }

        protected bool Equals(ImageProcessingContact other)
        {
            return string.Equals(this.Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImageProcessingContact)obj);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public static bool operator ==(ImageProcessingContact left, ImageProcessingContact right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ImageProcessingContact left, ImageProcessingContact right)
        {
            return !Equals(left, right);
        }
    }
}