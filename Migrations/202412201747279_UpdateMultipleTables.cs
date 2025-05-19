namespace eBookLibrary3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMultipleTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "PurchaseId", "dbo.Purchases");
            DropIndex("dbo.Payments", new[] { "PurchaseId" });
            DropPrimaryKey("dbo.Payments");
            AddColumn("dbo.Payments", "PaymentId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Payments", "UserId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Payments", "PaymentId");
            CreateIndex("dbo.Payments", "UserId");
            AddForeignKey("dbo.Payments", "UserId", "dbo.Users", "UserId", cascadeDelete: true);
            DropColumn("dbo.Payments", "PurchaseId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "PurchaseId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Payments", "UserId", "dbo.Users");
            DropIndex("dbo.Payments", new[] { "UserId" });
            DropPrimaryKey("dbo.Payments");
            DropColumn("dbo.Payments", "UserId");
            DropColumn("dbo.Payments", "PaymentId");
            AddPrimaryKey("dbo.Payments", "PurchaseId");
            CreateIndex("dbo.Payments", "PurchaseId");
            AddForeignKey("dbo.Payments", "PurchaseId", "dbo.Purchases", "PurchaseId");
        }
    }
}
