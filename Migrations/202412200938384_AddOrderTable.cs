namespace eBookLibrary3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderTable : DbMigration
    {
        public override void Up()
        {      
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentStatus = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
   
            
        }
        
        public override void Down()
        {
            
        }
    }
}
