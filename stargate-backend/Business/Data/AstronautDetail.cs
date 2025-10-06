using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("AstronautDetail")]
    public class AstronautDetail
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        public virtual Person Person { get; set; }
    }

    public class AstronautDetailConfiguration : IEntityTypeConfiguration<AstronautDetail>
    {
        public void Configure(EntityTypeBuilder<AstronautDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Rule: A Person will only ever hold one current Astronaut Duty Title, Start Date, and Rank at a time
            // This is enforced by the one-to-one relationship with Person
            
            builder.Property(x => x.CurrentRank).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CurrentDutyTitle).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CareerStartDate).IsRequired();
            builder.Property(x => x.CareerEndDate);
        }
    }
}
