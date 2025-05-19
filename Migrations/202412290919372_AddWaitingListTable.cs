namespace eBookLibrary3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWaitingListTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WaitingLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        AddedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WaitingLists", "UserId", "dbo.Users");
            DropForeignKey("dbo.WaitingLists", "BookId", "dbo.Books");
            DropIndex("dbo.WaitingLists", new[] { "UserId" });
            DropIndex("dbo.WaitingLists", new[] { "BookId" });
            DropTable("dbo.WaitingLists");
        }
    }
}
