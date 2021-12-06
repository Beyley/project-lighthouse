#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Serialization;
using LBPUnion.ProjectLighthouse.Types.Profiles;

namespace LBPUnion.ProjectLighthouse.Types.Levels
{
    /// <summary>
    ///     A LittleBigPlanet level.
    /// </summary>
    [XmlRoot("slot")]
    [XmlType("slot")]
    [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
    public class Slot
    {
        [XmlAttribute("type")]
        [NotMapped]
        public string Type { get; set; } = "user";

        [Key]
        [XmlElement("id")]
        public int SlotId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; } = "";

        [XmlElement("description")]
        public string Description { get; set; } = "";

        [XmlElement("icon")]
        public string IconHash { get; set; } = "";

        [XmlElement("rootLevel")]
        public string RootLevel { get; set; } = "";

        [XmlIgnore]
        public string ResourceCollection { get; set; } = "";

        [NotMapped]
        [XmlElement("resource")]
        public string[] Resources {
            get => this.ResourceCollection.Split(",");
            set => this.ResourceCollection = string.Join(',', value);
        }

        [XmlIgnore]
        public int LocationId { get; set; }

        [XmlIgnore]
        public int CreatorId { get; set; }

        [XmlIgnore]
        [ForeignKey(nameof(CreatorId))]
        public User? Creator { get; set; }

        [NotMapped]
        [XmlElement("npHandle")]
        public string? CreatorUsername {
            get => this.Creator?.Username;
            set {}
        }

        /// <summary>
        ///     The location of the level on the creator's earth
        /// </summary>
        [XmlElement("location")]
        [ForeignKey(nameof(LocationId))]
        public Location? Location { get; set; }

        [XmlElement("initiallyLocked")]
        public bool InitiallyLocked { get; set; }

        [XmlElement("isSubLevel")]
        public bool SubLevel { get; set; }

        [XmlElement("isLBP1Only")]
        public bool Lbp1Only { get; set; }

        [XmlElement("shareable")]
        public int Shareable { get; set; }

        [XmlElement("authorLabels")]
        public string AuthorLabels { get; set; } = "";

        [XmlElement("background")]
        public string BackgroundHash { get; set; } = "";

        [XmlElement("minPlayers")]
        public int MinimumPlayers { get; set; }

        [XmlElement("maxPlayers")]
        public int MaximumPlayers { get; set; }

        [XmlElement("moveRequired")]
        public bool MoveRequired { get; set; }

        [XmlElement("firstPublished")]
        public long FirstUploaded { get; set; }

        [XmlElement("lastUpdated")]
        public long LastUpdated { get; set; }

        [XmlElement("mmpick")]
        public bool TeamPick { get; set; }

        [XmlElement("game")]
        public GameVersion GameVersion { get; set; }

        [NotMapped]
        [XmlElement("heartCount")]
        public int Hearts {
            get {
                using Database database = new();

                return database.HeartedLevels.Count(s => s.SlotId == this.SlotId);
            }
            set {}
        }

        [NotMapped]
        [XmlElement("playCount")]
        public int Plays {
            get => this.PlaysLBP1 + this.PlaysLBP2 + this.PlaysLBP3 + this.PlaysLBPVita;
            set {}
        }

        [NotMapped]
        [XmlElement("uniquePlaycount")]
        public int PlaysUnique {
            get => this.PlaysLBP1Unique + this.PlaysLBP2Unique + this.PlaysLBP3Unique + this.PlaysLBPVitaUnique;
            set {}
        }

        [NotMapped]
        [XmlElement("completionCount")]
        public int PlaysComplete {
            get => this.PlaysLBP1Complete + this.PlaysLBP2Complete + this.PlaysLBP3Complete + this.PlaysLBPVitaComplete;
            set {}
        }

        [XmlIgnore]
        public int PlaysLBP1 { get; set; }

        [XmlIgnore]
        public int PlaysLBP1Complete { get; set; }

        [XmlIgnore]
        public int PlaysLBP1Unique { get; set; }

        [XmlIgnore]
        public int PlaysLBP2 { get; set; }

        [XmlIgnore]
        public int PlaysLBP2Complete { get; set; }

        [XmlIgnore]
        public int PlaysLBP2Unique { get; set; }

        [XmlIgnore]
        public int PlaysLBP3 { get; set; }

        [XmlIgnore]
        public int PlaysLBP3Complete { get; set; }

        [XmlIgnore]
        public int PlaysLBP3Unique { get; set; }

        [XmlIgnore]
        public int PlaysLBPVita { get; set; }

        [XmlIgnore]
        public int PlaysLBPVitaComplete { get; set; }

        [XmlIgnore]
        public int PlaysLBPVitaUnique { get; set; }

        [NotMapped]
        [XmlElement("thumbsup")]
        public int Thumbsup {
            get {
                using Database database = new();

                return database.RatedLevels.Count(r => r.SlotId == this.SlotId && r.Rating == 1);
            }
            set {}
        }

        [NotMapped]
        [XmlElement("thumbsdown")]
        public int Thumbsdown {
            get {
                using Database database = new();

                return database.RatedLevels.Count(r => r.SlotId == this.SlotId && r.Rating == -1);
            }
            set {}
        }

        [NotMapped]
        [XmlElement("averageRating")]
        public double RatingLBP1 {
            get {
                using Database database = new();

                IQueryable<RatedLevel> ratedLevels = database.RatedLevels.Where(r => r.SlotId == this.SlotId && r.RatingLBP1 > 0);
                if (!ratedLevels.Any()) return 3.0;

                return Enumerable.Average(ratedLevels, r => r.RatingLBP1);
            }
            set {}
        }

        [XmlElement("leveltype")]
        public string LevelType { get; set; } = "";

        [NotMapped]
        [XmlElement("sizeOfResources")]
        public int Size {
            get => this.Resources.Sum(FileHelper.ResourceSize);
            set {}
        }

        public string Serialize(RatedLevel? yourRatingStats = null, VisitedLevel? yourVisitedStats = null)
        {
            string slotData = LbpSerializer.StringElement("name", this.Name) +
                              LbpSerializer.StringElement("lbp1PlayCount", this.PlaysLBP1) +
                              LbpSerializer.StringElement("lbp1CompletionCount", this.PlaysLBP1Complete) +
                              LbpSerializer.StringElement("lbp1UniquePlayCount", this.PlaysLBP1Unique) +
                              LbpSerializer.StringElement("lbp2PlayCount", this.PlaysLBP2) +
                              LbpSerializer.StringElement("lbp2CompletionCount", this.PlaysLBP2Complete) +
                              LbpSerializer.StringElement("lbp2UniquePlayCount", this.PlaysLBP2Unique) + // not actually used ingame, as per above comment
                              LbpSerializer.StringElement("lbp3PlayCount", this.PlaysLBP3) +
                              LbpSerializer.StringElement("lbp3CompletionCount", this.PlaysLBP3Complete) +
                              LbpSerializer.StringElement("lbp3UniquePlayCount", this.PlaysLBP3Unique) +
                              LbpSerializer.StringElement("lbpvitaPlayCount", this.PlaysLBPVita) +
                              LbpSerializer.StringElement("lbpvitaCompletionCount", this.PlaysLBPVitaComplete) +
                              LbpSerializer.StringElement("lbpvitaUniquePlayCount", this.PlaysLBPVitaUnique) +
                              LbpSerializer.StringElement("yourRating", yourRatingStats?.RatingLBP1) +
                              LbpSerializer.StringElement("yourDPadRating", yourRatingStats?.Rating) +
                              LbpSerializer.StringElement("yourLBP1PlayCount", yourVisitedStats?.PlaysLBP1) +
                              LbpSerializer.StringElement("yourLBP2PlayCount", yourVisitedStats?.PlaysLBP2) +
                              LbpSerializer.StringElement("yourLBP3PlayCount", yourVisitedStats?.PlaysLBP3) +
                              LbpSerializer.StringElement
                                  ("yourLBPVitaPlayCount", yourVisitedStats?.PlaysLBPVita); // i doubt this is the right name but we'll go with it

            return LbpSerializer.TaggedStringElement("slot", slotData, "type", "user");
        }
    }
}