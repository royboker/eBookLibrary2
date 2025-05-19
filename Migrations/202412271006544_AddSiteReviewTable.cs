namespace eBookLibrary3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteReviewTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteReviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(),
                        Content = c.String(),
                        Rating = c.Single(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteReviews", "UserId", "dbo.Users");
            DropIndex("dbo.SiteReviews", new[] { "UserId" });
            DropTable("dbo.SiteReviews");
        }
    }
}
