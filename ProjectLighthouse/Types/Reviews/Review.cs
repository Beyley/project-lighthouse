#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using LBPUnion.ProjectLighthouse.Serialization;
using LBPUnion.ProjectLighthouse.Types.Levels;

namespace LBPUnion.ProjectLighthouse.Types.Reviews
{
    [XmlRoot("review")]
    [XmlType("review")]
    public class Review
    {
        // ReSharper disable once UnusedMember.Global
        [Key]
        public int ReviewId { get; set; }

        [XmlIgnore]
        public int ReviewerId { get; set; }

        [ForeignKey(nameof(ReviewerId))]
        public User Reviewer { get; set; }

        [XmlElement("slot_id")]
        public int SlotId { get; set; }

        [ForeignKey(nameof(SlotId))]
        public Slot Slot { get; set; }

        [XmlElement("timestamp")]
        public long Timestamp { get; set; }

        [XmlElement("labels")]
        public string LabelCollection { get; set; }

        [NotMapped]
        [XmlIgnore]
        public string[] Labels {
            get => this.LabelCollection.Split(",");
            set => this.LabelCollection = string.Join(',', value);
        }

        [XmlElement("deleted")]
        public bool Deleted { get; set; }

        [XmlElement("deleted_by")]
        public DeletedBy DeletedBy { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("thumb")]
        public int Thumb { get; set; }

        [XmlElement("thumbsup")]
        public int ThumbsUp { get; set; }

        [XmlElement("thumbsdown")]
        public int ThumbsDown { get; set; }

        public string Serialize
            (RatedLevel? yourLevelRating = null, RatedReview? yourRatingStats = null)
            => this.Serialize("review", yourLevelRating, yourRatingStats);

        public string Serialize(string elementOverride, RatedLevel? yourLevelRating = null, RatedReview? yourRatingStats = null)
        {

            XmlWriterSettings settings = new();
            settings.OmitXmlDeclaration = true;

            XmlSerializer serializer = new(typeof(DeletedBy));
            StringWriter stringWriter = new();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings)) serializer.Serialize(xmlWriter, this.DeletedBy);
            string deletedBy = stringWriter.ToString();

            string reviewData = LbpSerializer.TaggedStringElement("slot_id", this.SlotId, "type", this.Slot.Type) +
                                LbpSerializer.StringElement("reviewer", this.Reviewer.Username) +
                                LbpSerializer.StringElement("thumb", this.Thumb) +
                                LbpSerializer.StringElement("timestamp", this.Timestamp) +
                                LbpSerializer.StringElement("labels", this.LabelCollection) +
                                LbpSerializer.StringElement("deleted", this.Deleted) +
                                deletedBy +
                                LbpSerializer.StringElement("text", this.Text) +
                                LbpSerializer.StringElement("thumbsup", this.ThumbsUp) +
                                LbpSerializer.StringElement("thumbsdown", this.ThumbsDown) +
                                LbpSerializer.StringElement("yourthumb", yourRatingStats?.Thumb == null ? 0 : yourRatingStats?.Thumb);

            return LbpSerializer.TaggedStringElement(elementOverride, reviewData, "id", this.SlotId + "." + this.Reviewer.Username);
        }
    }

}